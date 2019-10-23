using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using TMPro;

public class NewGameDialog : MonoBehaviour
{
    [HideInInspector] public int CurrentSlot = -1;
    private TextMeshProUGUI _newGameDialogTitle;
    private TMP_InputField _playerNameInput;
    private Toggle _tutorialToggle;
    private CanvasGroup _playPanelCanvas;
    private Button _createButton;

    void Awake()
    {
        if (!_playPanelCanvas)
            _playPanelCanvas = GameObject.Find("PlayPanel").GetComponent<CanvasGroup>();
        Assert.IsNotNull(_playPanelCanvas, "Play panel canvas group not found!");

        if (!_newGameDialogTitle)
            _newGameDialogTitle = this.gameObject.transform.Find("NewGameDialogTitle").GetComponent<TextMeshProUGUI>();
        Assert.IsNotNull(_newGameDialogTitle, "New game title text GameObject not found!");

        if (!_playerNameInput)
            _playerNameInput = this.gameObject.transform.Find("PlayerNameInputfield").GetComponent<TMP_InputField>();
        Assert.IsNotNull(_playerNameInput, "New player name input field not found!");

        if (!_tutorialToggle)
            _tutorialToggle = this.gameObject.transform.Find("Bottom").transform.Find("Bottom_left").transform.Find("SkipTutorial_Checkbox").GetComponent<Toggle>();
        Assert.IsNotNull(_tutorialToggle, "Tutorial skip toggle button not found!");
        
        if (!_createButton)
            _createButton = this.gameObject.transform.Find("Bottom").transform.Find("Bottom_right").transform.Find("NewGameCreate_Button").GetComponent<Button>();
        Assert.IsNotNull(_createButton, "New game create button not found!");

        _newGameDialogTitle.text = "New Game";
        _createButton.interactable = false;
        _tutorialToggle.isOn = false;
    }

    public void Enable(int iSlot)
    {
        if (iSlot >= 0 && iSlot<= 2)
        {
            this.gameObject.SetActive(true);
            _playPanelCanvas.interactable = false;
            _playPanelCanvas.alpha = 0.2f;
            CurrentSlot = iSlot;
            _newGameDialogTitle.text = "New Game #" + (CurrentSlot + 1);
        }
        else
            Debug.LogError("New Game Dialog Error: wrong slot id -> " + iSlot);
    }

    public void BackButtonPress()
    {
        // add clean input field etc... or put it in the OnDisable() function
        _playPanelCanvas.interactable = true;
        _playPanelCanvas.alpha = 1f;
        _playerNameInput.text = "";
        _tutorialToggle.isOn = false;
        CurrentSlot = -1;
        this.gameObject.SetActive(false);
    }

    public void CreateButtonPress()
    {
        // DEBUG
        Debug.Log("New game dialog CREATE button press!");
        Debug.Log("Current slot is #" + CurrentSlot);
        Debug.Log("Player name input is [" + _playerNameInput.text + "]");
        Debug.Log("Tutorial skip is " + _tutorialToggle.isOn);

        _playerNameInput.text = _playerNameInput.text.Trim();
        GameManager.gm.CreateNewPlayer(CurrentSlot, _playerNameInput.text, !(_tutorialToggle.isOn));
        BackButtonPress();
    }

    public void PlayerNameInputValueChange()
    {
        _createButton.interactable = CheckPlayerName();
    }

    private bool CheckPlayerName()
    {
        if (_playerNameInput.text == "")
            return false;
        return true;
    }
}
