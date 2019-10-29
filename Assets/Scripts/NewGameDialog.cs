using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using TMPro;

public class NewGameDialog : MonoBehaviour
{
    [HideInInspector] public int CurrentSlot = -1;

    private TextMeshProUGUI _NewGameDialogTitle;
    private TMP_InputField _PlayerNameInput;
    private Toggle _TutorialToggle;
    private CanvasGroup _PlayPanelCanvas;
    private Button _CreateButton;

    void Awake()
    {
        if (!_PlayPanelCanvas)
            _PlayPanelCanvas = GameObject.Find("PlayPanel").GetComponent<CanvasGroup>();
        Assert.IsNotNull(_PlayPanelCanvas, "Play panel canvas group not found!");

        if (!_NewGameDialogTitle)
            _NewGameDialogTitle = this.gameObject.transform.Find("NewGameDialogTitle").GetComponent<TextMeshProUGUI>();
        Assert.IsNotNull(_NewGameDialogTitle, "New game title text GameObject not found!");

        if (!_PlayerNameInput)
            _PlayerNameInput = this.gameObject.transform.Find("PlayerNameInputfield").GetComponent<TMP_InputField>();
        Assert.IsNotNull(_PlayerNameInput, "New player name input field not found!");

        if (!_TutorialToggle)
            _TutorialToggle = this.gameObject.transform.Find("Bottom").transform.Find("Bottom_left").transform.Find("SkipTutorial_Checkbox").GetComponent<Toggle>();
        Assert.IsNotNull(_TutorialToggle, "Tutorial skip toggle button not found!");
        
        if (!_CreateButton)
            _CreateButton = this.gameObject.transform.Find("Bottom").transform.Find("Bottom_right").transform.Find("NewGameCreate_Button").GetComponent<Button>();
        Assert.IsNotNull(_CreateButton, "New game create button not found!");

        _NewGameDialogTitle.text = "New Game";
        _CreateButton.interactable = false;
        _TutorialToggle.isOn = false;
    }

    public void Enable(int iSlot)
    {
        if (iSlot >= 0 && iSlot<= 2)
        {
            this.gameObject.SetActive(true);
            _PlayPanelCanvas.interactable = false;
            _PlayPanelCanvas.alpha = 0.2f;
            CurrentSlot = iSlot;
            _NewGameDialogTitle.text = "New Game #" + (CurrentSlot + 1);
        }
        else
            Debug.LogError("New Game Dialog Error: wrong slot id -> " + iSlot);
    }

    public void BackButtonPress()
    {
        // add clean input field etc... or put it in the OnDisable() function
        _PlayPanelCanvas.interactable = true;
        _PlayPanelCanvas.alpha = 1f;
        _PlayerNameInput.text = "";
        _TutorialToggle.isOn = false;
        CurrentSlot = -1;
        this.gameObject.SetActive(false);
    }

    public void CreateButtonPress()
    {
        // DEBUG
        Debug.Log("New game dialog CREATE button press!");
        Debug.Log("Current slot is #" + CurrentSlot);
        Debug.Log("Player name input is [" + _PlayerNameInput.text + "]");
        Debug.Log("Tutorial skip is " + _TutorialToggle.isOn);

        _PlayerNameInput.text = _PlayerNameInput.text.Trim();
        GameManager.GM.CreateNewPlayer(CurrentSlot, _PlayerNameInput.text, !(_TutorialToggle.isOn));
        BackButtonPress();
    }

    public void PlayerNameInputValueChange()
    {
        _CreateButton.interactable = CheckPlayerName();
    }

    private bool CheckPlayerName()
    {
        if (_PlayerNameInput.text.Trim() == "")
            return false;
        return true;
    }
}
