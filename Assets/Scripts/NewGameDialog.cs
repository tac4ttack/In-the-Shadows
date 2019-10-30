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
        if (!NewGameDialogTitle)
            NewGameDialogTitle = this.gameObject.transform.Find("NewGameDialogTitle").GetComponent<TextMeshProUGUI>();
        if (!PlayerNameInput)
            PlayerNameInput = this.gameObject.transform.Find("PlayerNameInputfield").GetComponent<TMP_InputField>();
        if (!TutorialToggle)
            TutorialToggle = this.gameObject.transform.Find("Bottom").transform.Find("Bottom_left").transform.Find("SkipTutorial_Checkbox").GetComponent<Toggle>();
        if (!CreateButton)
            CreateButton = this.gameObject.transform.Find("Bottom").transform.Find("Bottom_right").transform.Find("NewGameCreate_Button").GetComponent<Button>();

        Assert.IsNotNull(PlayPanelCanvas, "Play panel canvas group not found!");
        Assert.IsNotNull(NewGameDialogTitle, "New game title text GameObject not found!");
        Assert.IsNotNull(PlayerNameInput, "New player name input field not found!");
        Assert.IsNotNull(TutorialToggle, "Tutorial skip toggle button not found!");
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
        // DEBUG
        // Debug.Log("New game dialog CREATE button press!");
        // Debug.Log("Current slot is #" + CurrentSlot);
        // Debug.Log("Player name input is [" + PlayerNameInput.text + "]");
        // Debug.Log("Tutorial skip is " + TutorialToggle.isOn);

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
