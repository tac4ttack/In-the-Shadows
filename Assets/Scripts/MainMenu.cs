using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class MainMenu : MonoBehaviour
{
    public StateMachine MainMenuStateMachine = new StateMachine();
    public enum MainMenuStates { Inactive = 0, TitleScreen, Main, Credits, Settings, Play, NewPlayerPrompt };
    public MainMenuStates CurrentState;

    [Header("Elements of Main Menu UI")]
    public CanvasGroup BackgroundPanel_CG;
    public CanvasGroup MainPanel_CG;
    public CanvasGroup MainButtonsPanel_CG;
    public CanvasGroup TitleScreenPanel_CG;
    public CanvasGroup PlayPanel_CG;
    public CanvasGroup SettingsPanel_CG;
    public CanvasGroup CreditsPanel_CG;
    public CanvasGroup NewPlayerPrompt_CG;

    public GameObject SpaceSystem_GO;
    public Toggle DebugModeCheckBox;

    void Awake()
    {
        if (BackgroundPanel_CG == null)
            BackgroundPanel_CG = this.gameObject.transform.Find("Background_Panel").GetComponent<CanvasGroup>();
        if (MainPanel_CG == null)
            MainPanel_CG = this.gameObject.transform.Find("Main_Panel").GetComponent<CanvasGroup>();
        if (TitleScreenPanel_CG == null)
            TitleScreenPanel_CG = this.gameObject.transform.Find("TitleScreen_Panel").GetComponent<CanvasGroup>();
        if (MainButtonsPanel_CG == null)
            MainButtonsPanel_CG = this.gameObject.transform.Find("MainButtons_Panel").GetComponent<CanvasGroup>();
        if (PlayPanel_CG == null)
            PlayPanel_CG = this.gameObject.transform.Find("Play_Panel").GetComponent<CanvasGroup>();
        if (SettingsPanel_CG == null)
            SettingsPanel_CG = this.gameObject.transform.Find("Settings_Panel").GetComponent<CanvasGroup>();
        if (CreditsPanel_CG == null)
            CreditsPanel_CG = this.gameObject.transform.Find("Credits_Panel").GetComponent<CanvasGroup>();
        if (NewPlayerPrompt_CG == null)
            NewPlayerPrompt_CG = this.gameObject.transform.Find("NewGame_Panel").GetComponent<CanvasGroup>();
        if (SpaceSystem_GO == null)
            SpaceSystem_GO = this.gameObject.transform.Find("Main_Panel").Find("SpaceSystem").gameObject;
        if (DebugModeCheckBox == null)
            DebugModeCheckBox = this.gameObject.transform.Find("Play_Panel").Find("Buttons").Find("Buttons_group_left").Find("DebugMode_Checkbox").GetComponent<Toggle>();

        Assert.IsNotNull(BackgroundPanel_CG, "Background Canvas group not found!");
        Assert.IsNotNull(MainPanel_CG, "Main Menu Canvas group not found!");
        Assert.IsNotNull(TitleScreenPanel_CG, "Title Screen Canvas group not found!");
        Assert.IsNotNull(MainButtonsPanel_CG, "Main Buttons Canvas group not found!");
        Assert.IsNotNull(PlayPanel_CG, "Play Panel Canvas group not found!");
        Assert.IsNotNull(SettingsPanel_CG, "Settings Canvas group not found!");
        Assert.IsNotNull(CreditsPanel_CG, "Credits Canvas group not found!");
        Assert.IsNotNull(NewPlayerPrompt_CG, "New Player Prompt Canvas group not found!");
        Assert.IsNotNull(SpaceSystem_GO, "Space System GameObject not found!");
        Assert.IsNotNull(DebugModeCheckBox, "Debug Mode Toggle not found!");
    }

    void Start() => MainMenuStateMachine.ChangeState(new TitleScreen_MainMenuState(this, TitleScreenPanel_CG.gameObject.transform.Find("PressAnyKey_Text").GetComponent<CanvasGroup>()));
    void Update() => MainMenuStateMachine.ExecuteState();

    #region Buttons logic

    public void PlayButtonPress()
    {
        MainMenuStateMachine.ChangeState(new Play_MainMenuState(this));
    }

    public void SettingsButtonPress()
    {
        MainMenuStateMachine.ChangeState(new Settings_MainMenuState(this));
    }

    public void CreditsButtonPress()
    {
        MainMenuStateMachine.ChangeState(new Credits_MainMenuState(this));
    }

    public void ExitButtonPress()
    {
        // DEBUG
        Debug.Log("App should exit!");
        //App exit!
    }

    public void BackButtonPress()
    {
        MainMenuStateMachine.GoBackToPreviousState();
    }

    public void PlayPanelBackButtonPress()
    {
        MainMenuStateMachine.ChangeState(new Main_MainMenuState(this));
    }

    public void DebugModeToggleCheck()
    {
        GameManager.GM.DebugMode = DebugModeCheckBox.isOn;
    }

    #endregion
}

#region Main Menu States

public class TitleScreen_MainMenuState : IState
{
    private MainMenu _MainMenu;
    private CanvasGroup _TitleScreenText_CG;

    public TitleScreen_MainMenuState(MainMenu iMainMenu, CanvasGroup iTitleScreenText_CG)
    {
        _MainMenu = iMainMenu;
        _TitleScreenText_CG = iTitleScreenText_CG;
    }

    public void Enter()
    {
        GameManager.GM.StartCoroutine(Utility.PopOutCanvasGroup(_MainMenu.BackgroundPanel_CG, 1f, Utility.TransitionSpeed));
        GameManager.GM.StartCoroutine(Utility.PopInCanvasGroup(_MainMenu.MainPanel_CG, 1f, Utility.TransitionSpeed));
        GameManager.GM.StartCoroutine(Utility.PopInCanvasGroup(_MainMenu.TitleScreenPanel_CG, 1f, Utility.TransitionSpeed));
        _MainMenu.CurrentState = MainMenu.MainMenuStates.TitleScreen;
    }

    public void Execute()
    {
        if (Input.anyKey)
            _MainMenu.MainMenuStateMachine.ChangeState(new Main_MainMenuState(_MainMenu));
        if (_MainMenu.CurrentState == MainMenu.MainMenuStates.TitleScreen)
            _TitleScreenText_CG.alpha = Mathf.Abs(Mathf.Sin(Time.time));
    }

    public void Exit()
    {
        GameManager.GM.StartCoroutine(Utility.PopOutCanvasGroup(_MainMenu.TitleScreenPanel_CG, 1f, Utility.TransitionSpeed));
    }
}

public class Main_MainMenuState : IState
{
    private MainMenu _MainMenu;

    public Main_MainMenuState(MainMenu iMainMenu)
    {
        _MainMenu = iMainMenu;
    }

    public void Enter()
    {
        GameManager.GM.StartCoroutine(Utility.PopInCanvasGroup(_MainMenu.MainPanel_CG, 1f, Utility.TransitionSpeed));
        GameManager.GM.StartCoroutine(Utility.PopInCanvasGroup(_MainMenu.MainButtonsPanel_CG, 1f, Utility.TransitionSpeed));
        _MainMenu.SpaceSystem_GO.SetActive(true);
        _MainMenu.CurrentState = MainMenu.MainMenuStates.Main;
    }

    public void Execute() { }

    public void Exit()
    {
        GameManager.GM.StartCoroutine(Utility.PopOutCanvasGroup(_MainMenu.MainPanel_CG, 1f, Utility.TransitionSpeed));
        GameManager.GM.StartCoroutine(Utility.PopOutCanvasGroup(_MainMenu.MainButtonsPanel_CG, 1f, Utility.TransitionSpeed));
        _MainMenu.SpaceSystem_GO.SetActive(false);
    }
}

public class Credits_MainMenuState : IState
{
    private MainMenu _MainMenu;

    public Credits_MainMenuState(MainMenu iMainMenu)
    {
        _MainMenu = iMainMenu;
    }

    public void Enter()
    {
        GameManager.GM.StartCoroutine(Utility.PopInCanvasGroup(_MainMenu.CreditsPanel_CG, 1f, Utility.TransitionSpeed));
        _MainMenu.CurrentState = MainMenu.MainMenuStates.Credits;
    }

    public void Execute() { }

    public void Exit()
    {
        GameManager.GM.StartCoroutine(Utility.PopOutCanvasGroup(_MainMenu.CreditsPanel_CG, 1f, Utility.TransitionSpeed));
    }
}

public class Settings_MainMenuState : IState
{
    private MainMenu _MainMenu;

    public Settings_MainMenuState(MainMenu iMainMenu)
    {
        _MainMenu = iMainMenu;
    }

    public void Enter()
    {
        GameManager.GM.StartCoroutine(Utility.PopInCanvasGroup(_MainMenu.SettingsPanel_CG, 1f, Utility.TransitionSpeed));
        _MainMenu.CurrentState = MainMenu.MainMenuStates.Settings;
    }

    public void Execute() { }

    public void Exit()
    {
        SaveSystem.SaveSettings(GameManager.GM.Settings);
        GameManager.GM.StartCoroutine(Utility.PopOutCanvasGroup(_MainMenu.SettingsPanel_CG, 1f, Utility.TransitionSpeed));
    }
}

public class Play_MainMenuState : IState
{
    private MainMenu _MainMenu;

    public Play_MainMenuState(MainMenu iMainMenu)
    {
        _MainMenu = iMainMenu;
    }

    public void Enter()
    {
        GameManager.GM.StartCoroutine(Utility.PopInCanvasGroup(_MainMenu.PlayPanel_CG, 1f, Utility.TransitionSpeed));
        _MainMenu.CurrentState = MainMenu.MainMenuStates.Play;
    }

    public void Execute() { }

    public void Exit()
    {
        GameManager.GM.StartCoroutine(Utility.PopOutCanvasGroup(_MainMenu.PlayPanel_CG, 1f, Utility.TransitionSpeed));
    }
}

public class NewPlayerPrompt_MainMenuState : IState
{
    private MainMenu _MainMenu;
    private int _SlotID;
    private NewGameDialog _NewGameDialog;

    public NewPlayerPrompt_MainMenuState(MainMenu iMainMenu, int iSlotID)
    {
        _MainMenu = iMainMenu;
        _SlotID = iSlotID;
        _NewGameDialog = _MainMenu.NewPlayerPrompt_CG.gameObject.GetComponent<NewGameDialog>();
    }

    public void Enter()
    {
        GameManager.GM.StartCoroutine(Utility.PopInCanvasGroup(_MainMenu.NewPlayerPrompt_CG, 1f, Utility.TransitionSpeed));
        _MainMenu.CurrentState = MainMenu.MainMenuStates.NewPlayerPrompt;
        _NewGameDialog.Enable(_SlotID);

        //  Scotchtape method to have the PlayPanel_CG at 0.2f alpha
        _MainMenu.PlayPanel_CG.alpha += 1f;
        // Debug, commented till the new version of pop functions are ready
        // GameManager.GM.StartCoroutine(Utility.PopOutCanvasGroup(_MainMenu.PlayPanel_CG, 1f, Utility.TransitionSpeed, 0.2f));
    }

    public void Execute() { }


    public void Exit()
    {
        _NewGameDialog.PlayerNameInput.text = "";
        _NewGameDialog.TutorialToggle.isOn = false;
        _NewGameDialog.CurrentSlot = -1;
        GameManager.GM.StartCoroutine(Utility.PopOutCanvasGroup(_MainMenu.NewPlayerPrompt_CG, 1f, Utility.TransitionSpeed));
    }
}

public class Inactive_MainMenuState : IState
{
    private MainMenu _MainMenu;

    public Inactive_MainMenuState(MainMenu iMainMenu)
    {
        _MainMenu = iMainMenu;
    }

    public void Enter()
    {
        _MainMenu.CurrentState = MainMenu.MainMenuStates.Inactive;
    }

    public void Execute() {}

    public void Exit() {}
}

#endregion