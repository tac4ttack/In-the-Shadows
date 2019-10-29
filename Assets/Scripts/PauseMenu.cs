using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;


public class PauseMenu : MonoBehaviour
{
    public StateMachine PauseMenuStateMachine = new StateMachine();
    public enum PauseMenuStates { Inactive = 0, Active, Settings};
    public PauseMenuStates CurrentState;
    public float TransitionSpeed = 0.1f;

    [Header("Canvas Groups of Pause Menu UI")]
    public CanvasGroup PauseCG;
    public CanvasGroup BackgroundCG;
    public CanvasGroup PauseMenuCG;
    public CanvasGroup SettingsCG;

    [Header("Main buttons of Pause Menu UI")]
    public Button ResumeButton;
    public Button RestartButton;
    public Button SettingsButton;
    public Button ExitButton;
    public Button AbortButton;

    void Awake()
    {
        if (PauseCG == null)
            PauseCG = this.gameObject.GetComponent<CanvasGroup>();
        if (BackgroundCG == null)
            BackgroundCG = this.gameObject.transform.Find("PauseMenu_Background").GetComponent<CanvasGroup>();
        if (PauseMenuCG == null)
            PauseMenuCG = this.gameObject.transform.Find("PauseMenu_Panel").GetComponent<CanvasGroup>();
        if (SettingsCG == null)
            SettingsCG = this.gameObject.transform.Find("Settings_Panel").GetComponent<CanvasGroup>();;

        Assert.IsNotNull(PauseCG, "Pause Menu UI Canvas group not found!");
        Assert.IsNotNull(BackgroundCG, "Pause Menu Background Canvas group not found!");
        Assert.IsNotNull(PauseMenuCG, "Pause Menu Panel Canvas group not found!");
        Assert.IsNotNull(SettingsCG, "Settings Panel Canvas group not found!");

        if (ResumeButton == null)
            ResumeButton = PauseMenuCG.gameObject.transform.Find("Resume_Button").GetComponent<Button>();
        if (RestartButton == null)
            RestartButton = PauseMenuCG.gameObject.transform.Find("Restart_Button").GetComponent<Button>();
        if (SettingsButton == null)
            SettingsButton = PauseMenuCG.gameObject.transform.Find("Settings_Button").GetComponent<Button>();
        if (AbortButton == null)
            AbortButton = PauseMenuCG.gameObject.transform.Find("Abort_Button").GetComponent<Button>();
        if (ExitButton == null)
            ExitButton = PauseMenuCG.gameObject.transform.Find("Exit_Button").GetComponent<Button>();

        Assert.IsNotNull(ResumeButton, "Pause menu Resume button not found!");
        Assert.IsNotNull(RestartButton, "Pause menu Restart button not found!");
        Assert.IsNotNull(SettingsButton, "Pause menu Settings button not found!");
        Assert.IsNotNull(ExitButton, "Pause menu Exit button not found!");
        Assert.IsNotNull(AbortButton, "Pause menu Abort button not found!");

        // DEBUG TODO!
        // When the main state machine will be done
        // if (levelselection)
        //      disable(restart button)
        //      disable(abort button)
    }

    void Start()
    {
        PauseMenuStateMachine.ChangeState(new Inactive_PauseMenuState(this.gameObject, this));
        CurrentState = PauseMenuStates.Inactive;
    }

    void Update()
    {
        PauseMenuStateMachine.ExecuteState();
    }

    /*
    **  Settings Panel buttons logic
    */
    public void SettingsButtonClick()
    {
        PauseMenuStateMachine.ChangeState(new Settings_PauseMenuState(this.gameObject, this));
    }

    public void SettingsBackButtonClick()
    {
        PauseMenuStateMachine.GoBackToPreviousState();
    }
}

public class Inactive_PauseMenuState : IState
{
    private GameObject _PauseMenuGO;
    private PauseMenu _PauseMenu;

    public Inactive_PauseMenuState(GameObject iPauseMenuGO, PauseMenu iPauseMenu)
    {
        _PauseMenuGO = iPauseMenuGO;
        _PauseMenu = iPauseMenu;
    }

    public void Enter()
    {
        GameManager.GM.StartCoroutine(Utility.PopOutCanvasGroup(_PauseMenu.PauseCG, 1f, _PauseMenu.TransitionSpeed));
        GameManager.GM.StartCoroutine(Utility.PopOutCanvasGroup(_PauseMenu.BackgroundCG, 1f, _PauseMenu.TransitionSpeed));
        GameManager.GM.StartCoroutine(Utility.PopOutCanvasGroup(_PauseMenu.PauseMenuCG, 1f, _PauseMenu.TransitionSpeed));
        _PauseMenu.CurrentState = PauseMenu.PauseMenuStates.Inactive;
    }

    public void Execute()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _PauseMenu.PauseMenuStateMachine.ChangeState(new Active_PauseMenuState(_PauseMenuGO, _PauseMenu));
        }
    }

    public void Exit()
    {
        Debug.Log("Inactive PM Exit");
    }
}

public class Active_PauseMenuState : IState
{
    private GameObject _PauseMenuGO;
    private PauseMenu _PauseMenu;

    public Active_PauseMenuState(GameObject iPauseMenuGO, PauseMenu iPauseMenu)
    {
        _PauseMenuGO = iPauseMenuGO;
        _PauseMenu = iPauseMenu;
    }

    public void Enter()
    {
        GameManager.GM.StartCoroutine(Utility.PopInCanvasGroup(_PauseMenu.PauseCG, 1f, _PauseMenu.TransitionSpeed));
        GameManager.GM.StartCoroutine(Utility.PopInCanvasGroup(_PauseMenu.BackgroundCG, 1f, _PauseMenu.TransitionSpeed));
        GameManager.GM.StartCoroutine(Utility.PopInCanvasGroup(_PauseMenu.PauseMenuCG, 1f, _PauseMenu.TransitionSpeed));
        _PauseMenu.CurrentState = PauseMenu.PauseMenuStates.Active;
    }

    public void Execute()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _PauseMenu.PauseMenuStateMachine.ChangeState(new Inactive_PauseMenuState(_PauseMenuGO, _PauseMenu));
        }
    }

    public void Exit()
    {
        Debug.Log("Active PM Exit");
    }
}

public class Settings_PauseMenuState : IState
{
    private GameObject _PauseMenuGO;
    private PauseMenu _PauseMenu;

    public Settings_PauseMenuState(GameObject iPauseMenuGO, PauseMenu iPauseMenu)
    {
        _PauseMenuGO = iPauseMenuGO;
        _PauseMenu = iPauseMenu;
    }

    public void Enter()
    {
        GameManager.GM.StartCoroutine(Utility.PopOutCanvasGroup(_PauseMenu.PauseMenuCG, 1f, _PauseMenu.TransitionSpeed));
        GameManager.GM.StartCoroutine(Utility.PopInCanvasGroup(_PauseMenu.SettingsCG, 1f, _PauseMenu.TransitionSpeed));
        _PauseMenu.CurrentState = PauseMenu.PauseMenuStates.Settings;
    }

    public void Execute()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _PauseMenu.PauseMenuStateMachine.GoBackToPreviousState();
        }
    }

    public void Exit()
    {
        SaveSystem.SaveSettings(GameManager.GM.Settings);
        GameManager.GM.StartCoroutine(Utility.PopOutCanvasGroup(_PauseMenu.SettingsCG, 1f, _PauseMenu.TransitionSpeed));
    }
}