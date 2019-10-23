using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using TMPro;

public class NewGameDialog : MonoBehaviour
{
    [HideInInspector] public int CurrentSlot = -1;
    private TextMeshProUGUI NewGameDialogTitleText;
    private TMP_InputField PlayerNameInput;
    private Toggle TutorialToggle;
    private CanvasGroup _playPanelCanvas;

    void Awake()
    {
        if (!_playPanelCanvas)
            _playPanelCanvas = GameObject.Find("PlayPanel").GetComponent<CanvasGroup>();
        Assert.IsNotNull(_playPanelCanvas, "Play panel canvas group not found!");

        if (!NewGameDialogTitleText)
            NewGameDialogTitleText = GameObject.Find("NewGameDialogTitle").GetComponent<TextMeshProUGUI>();
            // NewGameDialogTitleText = this.gameObject.transform.Find("NewGameDialogTitle").GetComponent<TextMeshProUGUI>();
        Assert.IsNotNull(NewGameDialogTitleText, "New game title text GameObject not found!");

        if (!PlayerNameInput)
            PlayerNameInput = GameObject.Find("PlayerNameInputfield").GetComponent<TMP_InputField>();
            // PlayerNameInput = this.gameObject.transform.Find("PlayerNameInputfield").GetComponent<TMP_InputField>();
        Assert.IsNotNull(PlayerNameInput, "New player name input field not found!");

        if (!TutorialToggle)
            TutorialToggle = GameObject.Find("SkipTutorial_Checkbox").GetComponent<Toggle>();
            // TutorialToggle = this.gameObject.transform.Find("SkipTutorial_Checkbox").GetComponent<Toggle>();
        Assert.IsNotNull(TutorialToggle, "Tutorial skip toggle button not found!");
        
        NewGameDialogTitleText.text = "New Game";
    }

    public void Enable(int iSlot)
    {
        if (iSlot >= 0 && iSlot<= 2)
        {
            this.gameObject.SetActive(true);
            _playPanelCanvas.interactable = false;
            _playPanelCanvas.alpha = 0.2f;
            CurrentSlot = iSlot;
            NewGameDialogTitleText.text = "New Game #" + (CurrentSlot + 1);
        }
        else
            Debug.LogError("New Game Dialog Error: wrong slot id -> " + iSlot);
    }

    public void BackButtonPress()
    {
        // add clean input field etc... or put it in the OnDisable() function
        _playPanelCanvas.interactable = true;
        _playPanelCanvas.alpha = 1f;
        PlayerNameInput.text = "";
        TutorialToggle.isOn = false;
        CurrentSlot = -1;
        this.gameObject.SetActive(false);
    }

    public void CreateButtonPress()
    {
        Debug.Log("New game dialog CREATE button press!");
        Debug.Log("Current slot is #" + CurrentSlot);
        Debug.Log("Player name input is [" + PlayerNameInput.text + "]");
        Debug.Log("Tutorial skip is " + TutorialToggle.isOn);
        GameManager.gm.CreateNewPlayer(CurrentSlot, PlayerNameInput.text, !(TutorialToggle.isOn));
        BackButtonPress();
    }
}
