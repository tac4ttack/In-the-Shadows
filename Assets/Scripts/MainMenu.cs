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
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        // DEBUG
        Debug.Log($"MAIN MENU - {this.name} - Awake()");
        #endif

        if (BackgroundPanel_CG == null)
            BackgroundPanel_CG = GameObject.FindGameObjectWithTag("Main Menu/Background").GetComponent<CanvasGroup>();
        Assert.IsNotNull(BackgroundPanel_CG, "Background Canvas group not found!");

        if (MainPanel_CG == null)
            MainPanel_CG = GameObject.FindGameObjectWithTag("Main Menu/Main Panel").GetComponent<CanvasGroup>();
        Assert.IsNotNull(MainPanel_CG, "Main Menu Canvas group not found!");

        if (TitleScreenPanel_CG == null)
            TitleScreenPanel_CG = GameObject.FindGameObjectWithTag("Main Menu/Title Screen Panel").GetComponent<CanvasGroup>();
        Assert.IsNotNull(TitleScreenPanel_CG, "Title Screen Canvas group not found!");

        if (MainButtonsPanel_CG == null)
            MainButtonsPanel_CG = GameObject.FindGameObjectWithTag("Main Menu/Main Buttons Panel").GetComponent<CanvasGroup>();
        Assert.IsNotNull(MainButtonsPanel_CG, "Main Buttons Canvas group not found!");

        if (PlayPanel_CG == null)
            PlayPanel_CG = GameObject.FindGameObjectWithTag("Main Menu/Play Panel").GetComponent<CanvasGroup>();
        Assert.IsNotNull(PlayPanel_CG, "Play Panel Canvas group not found!");

        if (SettingsPanel_CG == null)
            SettingsPanel_CG = GameObject.FindGameObjectWithTag("Settings/Panel").GetComponent<CanvasGroup>();
        Assert.IsNotNull(SettingsPanel_CG, "Settings Canvas group not found!");

        if (CreditsPanel_CG == null)
            CreditsPanel_CG = GameObject.FindGameObjectWithTag("Main Menu/Credits Panel").GetComponent<CanvasGroup>();
        Assert.IsNotNull(CreditsPanel_CG, "Credits Canvas group not found!");

        if (NewPlayerPrompt_CG == null)
            NewPlayerPrompt_CG = GameObject.FindGameObjectWithTag("Main Menu/New Game Dialog").GetComponent<CanvasGroup>();
        Assert.IsNotNull(NewPlayerPrompt_CG, "New Player Prompt Canvas group not found!");

        if (SpaceSystem_GO == null)
            SpaceSystem_GO = GameObject.FindGameObjectWithTag("Main Menu/Space System").gameObject;
        Assert.IsNotNull(SpaceSystem_GO, "Space System GameObject not found!");

        if (DebugModeCheckBox == null)
            DebugModeCheckBox = GameObject.FindGameObjectWithTag("Main Menu/Debug Mode Toggle").GetComponent<Toggle>();
        Assert.IsNotNull(DebugModeCheckBox, "Debug Mode Toggle not found!");
    }

    #if UNITY_EDITOR || DEVELOPMENT_BUILD
    // DEBUG
    void Start()
    {
        Debug.Log($"MAIN MENU - {this.name} - Start()");

        MainMenuStateMachine.ChangeState(new TitleScreen_MainMenuState(this, GameObject.FindGameObjectWithTag("Main Menu/Press Any Key Text").GetComponent<CanvasGroup>()));
    }
    #else
    void Start() => MainMenuStateMachine.ChangeState(new TitleScreen_MainMenuState(this, GameObject.FindGameObjectWithTag("Main Menu/Press Any Key Text").GetComponent<CanvasGroup>()));
    #endif

    void Update() => MainMenuStateMachine.ExecuteState();

    #region Buttons logic

    public void PlayButtonPress()
    {
        GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[1]);
        MainMenuStateMachine.ChangeState(new Play_MainMenuState(this));
    }

    public void SettingsButtonPress()
    {
        GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[1]);
        MainMenuStateMachine.ChangeState(new Settings_MainMenuState(this));
    }

    public void CreditsButtonPress()
    {
        GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[1]);
        MainMenuStateMachine.ChangeState(new Credits_MainMenuState(this));
    }

    public void ExitButtonPress()
    {
        GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[1]);
        SaveSystem.SavePlayers(GameManager.GM.Players);
        SaveSystem.SaveSettings(GameManager.GM.Settings);
        Application.Quit();
    }

    public void BackButtonPress()
    {
        GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[1]);
        MainMenuStateMachine.GoBackToPreviousState();
    }

    public void PlayPanelBackButtonPress()
    {
        GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[1]);
        MainMenuStateMachine.ChangeState(new Main_MainMenuState(this));
    }

    public void ClearAllButtonPress()
    {
        GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[1]);
        GameManager.GM.ClearAllPlayersData();
    }

    public void DebugModeToggleCheck()
    {
        GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[2]);        
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
        GameManager.GM.CurrentState = GameManager.GameStates.MainMenu;
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
        _NewGameDialog.PlayerName.text = "";
        _NewGameDialog.TutorialSkip.isOn = false;
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

    public void Execute() { }

    public void Exit() { }
}

#endregion