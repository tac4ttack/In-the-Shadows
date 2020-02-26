using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using TMPro;

public class NewGameDialog : MonoBehaviour
{
    [HideInInspector] public int CurrentSlot = -1;

    public TMP_InputField PlayerName;
    public Toggle TutorialSkip;

    private MainMenu _MainMenuScript;
    private TextMeshProUGUI _NewGameDialogTitle;
    private Button _Create_BTN;
    private Button _Back_BTN;

    void Awake()
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        // DEBUG
        Debug.Log($"NEW GAME DIALOG - {this.name} - Awake()");
        #endif

        if (_MainMenuScript == null)
            _MainMenuScript = GameObject.FindGameObjectWithTag("Main Menu/UI").GetComponent<MainMenu>();
        Assert.IsNotNull(_MainMenuScript, "Main Menu script not found in scene!");

        if (_NewGameDialogTitle == null)
            _NewGameDialogTitle = GameObject.FindGameObjectWithTag("New Game Dialog/Title Text").GetComponent<TextMeshProUGUI>();
        Assert.IsNotNull(_NewGameDialogTitle, "New game title text GameObject not found!");

        if (PlayerName == null)
            PlayerName = GameObject.FindGameObjectWithTag("New Game Dialog/Player Name Input").GetComponent<TMP_InputField>();
        Assert.IsNotNull(PlayerName, "New player name input field not found!");
        PlayerName.onValueChanged.AddListener(delegate { PlayerNameInputValueChange(); });

        if (TutorialSkip == null)
            TutorialSkip = GameObject.FindGameObjectWithTag("New Game Dialog/Skip Tutorial Toggle").GetComponent<Toggle>();
        Assert.IsNotNull(TutorialSkip, "Tutorial skip toggle button not found!");

        if (_Create_BTN == null)
            _Create_BTN = GameObject.FindGameObjectWithTag("New Game Dialog/Create Button").GetComponent<Button>();
        Assert.IsNotNull(_Create_BTN, "New game create button not found!");
        _Create_BTN.onClick.AddListener(delegate { CreateButtonPress(); });
        _Create_BTN.onClick.AddListener(delegate { _MainMenuScript.BackButtonPress(); });

        if (_Back_BTN == null)
            _Back_BTN = GameObject.FindGameObjectWithTag("New Game Dialog/Back Button").GetComponent<Button>();
        Assert.IsNotNull(_Back_BTN, "New game back button not found!");
        _Back_BTN.onClick.AddListener(delegate { _MainMenuScript.BackButtonPress(); });
    }

    public void Enable(int iSlot)
    {
        if (iSlot >= 0 && iSlot <= 2)
        {
            PlayerNameInputValueChange();
            CurrentSlot = iSlot;
            _NewGameDialogTitle.text = "New Game #" + (CurrentSlot + 1);
        }
        else
        {
            // DEBUG
            #if UNITY_EDITOR
            Debug.LogError("New Game Dialog Error: wrong slot id -> " + iSlot);
            #endif
            
            ;
        }
    }

    private void CreateButtonPress()
    {
        GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[1]);
        PlayerName.text = PlayerName.text.Trim();
        GameManager.GM.CreateNewPlayer(CurrentSlot, PlayerName.text, !(TutorialSkip.isOn));
    }

    private void PlayerNameInputValueChange()
    {
        _Create_BTN.interactable = CheckPlayerName();
    }

    private bool CheckPlayerName()
    {
        if (PlayerName.text.Trim() == "")
            return false;
        return true;
    }
}
