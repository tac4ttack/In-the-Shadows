using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using TMPro;

public class NewGameDialog : MonoBehaviour
{
    [HideInInspector] public int CurrentSlot = -1;

    public TextMeshProUGUI NewGameDialogTitle;
    public TMP_InputField PlayerNameInput;
    public Toggle TutorialToggle;
    public CanvasGroup PlayPanelCanvas;
    public Button CreateButton;

    void Awake()
    {
        if (!PlayPanelCanvas)
            PlayPanelCanvas = GameObject.Find("Play_Panel").GetComponent<CanvasGroup>();
        Assert.IsNotNull(PlayPanelCanvas, "Play panel canvas group not found!");

        if (!NewGameDialogTitle)
            NewGameDialogTitle = this.gameObject.transform.Find("NewGameDialogTitle").GetComponent<TextMeshProUGUI>();
        Assert.IsNotNull(NewGameDialogTitle, "New game title text GameObject not found!");

        if (!PlayerNameInput)
            PlayerNameInput = this.gameObject.transform.Find("PlayerNameInputfield").GetComponent<TMP_InputField>();
        Assert.IsNotNull(PlayerNameInput, "New player name input field not found!");

        if (!TutorialToggle)
            TutorialToggle = this.gameObject.transform.Find("Bottom").Find("Bottom_left").Find("SkipTutorial_Checkbox").GetComponent<Toggle>();
        Assert.IsNotNull(TutorialToggle, "Tutorial skip toggle button not found!");

        if (!CreateButton)
            CreateButton = this.gameObject.transform.Find("Bottom").Find("Bottom_right").Find("NewGameCreate_Button").GetComponent<Button>();
        Assert.IsNotNull(CreateButton, "New game create button not found!");
    }

    public void Enable(int iSlot)
    {
        if (iSlot >= 0 && iSlot <= 2)
        {
            PlayerNameInputValueChange();
            CurrentSlot = iSlot;
            NewGameDialogTitle.text = "New Game #" + (CurrentSlot + 1);
        }
        else
            Debug.LogError("New Game Dialog Error: wrong slot id -> " + iSlot);
    }

    public void CreateButtonPress()
    {
        PlayerNameInput.text = PlayerNameInput.text.Trim();
        GameManager.GM.CreateNewPlayer(CurrentSlot, PlayerNameInput.text, !(TutorialToggle.isOn));
    }

    public void PlayerNameInputValueChange()
    {
        CreateButton.interactable = CheckPlayerName();
    }

    private bool CheckPlayerName()
    {
        if (PlayerNameInput.text.Trim() == "")
            return false;
        return true;
    }
}
