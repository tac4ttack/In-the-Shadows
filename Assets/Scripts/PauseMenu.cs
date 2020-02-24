using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public StateMachine PauseMenuStateMachine = new StateMachine();
    public enum PauseMenuStates { Inactive = 0, Active, Settings, ConfirmationPrompt };
    public PauseMenuStates CurrentState;

    public enum ConfirmationPromptTarget { None = 0, MainMenu, LevelSelection, Restart };
    public ConfirmationPromptTarget CurrentPromptTarget;

    [Header("Elements of Pause Menu UI")]
    public CanvasGroup Pause_CG;
    public CanvasGroup Background_CG;
    public CanvasGroup PauseMenu_CG;
    public CanvasGroup Settings_CG;
    public CanvasGroup ConfirmationPrompt_CG;
    private Button _Restart_BTN;
    private Button _Abort_BTN;
    
    private Puzzle _PuzzleScript;


    void Awake()
    {
        if (Pause_CG == null)
            Pause_CG = this.gameObject.GetComponent<CanvasGroup>();
        Assert.IsNotNull(Pause_CG, "Pause Menu UI Canvas group not found!");

        if (Background_CG == null)
            Background_CG = GameObject.FindGameObjectWithTag("PauseMenu_Background").GetComponent<CanvasGroup>();
        Assert.IsNotNull(Background_CG, "Pause Menu Background Canvas group not found!");

        if (PauseMenu_CG == null)
            PauseMenu_CG = GameObject.FindGameObjectWithTag("PauseMenu_Panel").GetComponent<CanvasGroup>();
        Assert.IsNotNull(PauseMenu_CG, "Pause Menu Canvas group not found!");

        if (Settings_CG == null)
            Settings_CG = GameObject.FindGameObjectWithTag("Settings_Panel").GetComponent<CanvasGroup>();
        Assert.IsNotNull(Settings_CG, "Settings Canvas group not found!");

        if (ConfirmationPrompt_CG == null)
            ConfirmationPrompt_CG = GameObject.FindGameObjectWithTag("PauseMenu_ConfirmationDialog").GetComponent<CanvasGroup>();
        Assert.IsNotNull(ConfirmationPrompt_CG, "Confirmation Prompt Canvas group not found!");

        if (_Restart_BTN == null)
            _Restart_BTN = GameObject.FindGameObjectWithTag("PauseMenu_RestartButton").GetComponent<Button>();
        Assert.IsNotNull(_Restart_BTN, "Restart button not found!");

        if (_Abort_BTN == null)
            _Abort_BTN = GameObject.FindGameObjectWithTag("PauseMenu_AbortButton").GetComponent<Button>();
        Assert.IsNotNull(_Abort_BTN, "Abort button not found!");
    }

    void Start()
    {
        if (GameManager.GM.CurrentState == GameManager.GameStates.InGame)
        {
            if (_PuzzleScript == null)
                _PuzzleScript = GameObject.FindGameObjectWithTag("InGame_PuzzleContainer").GetComponent<Puzzle>();
            Assert.IsNotNull(_PuzzleScript, "Puzzle script not found!");
        }

        PauseMenuStateMachine.ChangeState(new Inactive_PauseMenuState(this));
        _Restart_BTN.gameObject.SetActive(GameManager.GM.CurrentState == GameManager.GameStates.InGame);
        _Abort_BTN.gameObject.SetActive(GameManager.GM.CurrentState == GameManager.GameStates.InGame);
    }

    #region Buttons Logic

    public void ResumeButtonPress()
    {
        GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[1]);
        if (GameManager.GM.CurrentState == GameManager.GameStates.InGame)
            _PuzzleScript.PuzzleStateMachine.ChangeState(new Playing_PuzzleState(_PuzzleScript));
        PauseMenuStateMachine.ChangeState(new Inactive_PauseMenuState(this));
    }

    public void RestartButtonPress()
    {
        GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[1]);
        CurrentPromptTarget = PauseMenu.ConfirmationPromptTarget.Restart;
        PauseMenuStateMachine.ChangeState(new ConfirmationPrompt_PauseMenuState(this));
    }

    public void AbortButtonPress()
    {
        GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[1]);
        CurrentPromptTarget = PauseMenu.ConfirmationPromptTarget.LevelSelection;
        PauseMenuStateMachine.ChangeState(new ConfirmationPrompt_PauseMenuState(this));
    }

    public void ExitButtonPress()
    {
        GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[1]);
        CurrentPromptTarget = PauseMenu.ConfirmationPromptTarget.MainMenu;
        PauseMenuStateMachine.ChangeState(new ConfirmationPrompt_PauseMenuState(this));
    }

    public void SettingsButtonPress()
    {
        GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[1]);
        PauseMenuStateMachine.ChangeState(new Settings_PauseMenuState(this));
    }

    public void SettingsBackButtonPress()
    {
        GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[1]);
        PauseMenuStateMachine.GoBackToPreviousState();
    }

    public void ConfirmationYesButtonPress()
    {
        GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[1]);
        switch (CurrentPromptTarget)
        {
            case PauseMenu.ConfirmationPromptTarget.MainMenu:
                GameManager.GM.GameStateMachine.ChangeState(new InMainMenu_GameState());
                break;
            case PauseMenu.ConfirmationPromptTarget.Restart:
                GameManager.GM.GameStateMachine.ChangeState(new InGame_GameState(SceneManager.GetActiveScene().buildIndex));
                break;
            case PauseMenu.ConfirmationPromptTarget.LevelSelection:
                GameManager.GM.GameStateMachine.ChangeState(new LevelSelection_GameState());
                break;
            default:
                break;
        }
    }

    public void ConfirmationNoButtonPress()
    {
        GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[1]);
        PauseMenuStateMachine.GoBackToPreviousState();
    }

    public void PauseMenuButtonPress()
    {
        GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[1]);
        PauseMenuStateMachine.ChangeState(new Active_PauseMenuState(this));
    }

    #endregion
}

#region PauseMenu States

public class Inactive_PauseMenuState : IState
{
    private PauseMenu _PauseMenu;

    public Inactive_PauseMenuState(PauseMenu iPauseMenu) => _PauseMenu = iPauseMenu;

    public void Enter()
    {
        GameManager.GM.StartCoroutine(Utility.PopOutCanvasGroup(_PauseMenu.Pause_CG, 1f, Utility.TransitionSpeed));
        GameManager.GM.StartCoroutine(Utility.PopOutCanvasGroup(_PauseMenu.Background_CG, 1f, Utility.TransitionSpeed));
        GameManager.GM.StartCoroutine(Utility.PopOutCanvasGroup(_PauseMenu.PauseMenu_CG, 1f, Utility.TransitionSpeed));
        _PauseMenu.CurrentState = PauseMenu.PauseMenuStates.Inactive;
    }

    public void Execute() { }

    public void Exit() { }
}

public class Active_PauseMenuState : IState
{
    private PauseMenu _PauseMenu;

    public Active_PauseMenuState(PauseMenu iPauseMenu) => _PauseMenu = iPauseMenu;

    public void Enter()
    {
        GameManager.GM.StartCoroutine(Utility.PopInCanvasGroup(_PauseMenu.Pause_CG, 1f, Utility.TransitionSpeed));
        GameManager.GM.StartCoroutine(Utility.PopInCanvasGroup(_PauseMenu.Background_CG, 1f, Utility.TransitionSpeed));
        GameManager.GM.StartCoroutine(Utility.PopInCanvasGroup(_PauseMenu.PauseMenu_CG, 1f, Utility.TransitionSpeed));
        _PauseMenu.CurrentState = PauseMenu.PauseMenuStates.Active;
        Time.timeScale = 0f;
    }

    public void Execute() { }

    public void Exit()
    {
        GameManager.GM.StartCoroutine(Utility.PopOutCanvasGroup(_PauseMenu.PauseMenu_CG, 1f, Utility.TransitionSpeed));
        Time.timeScale = 1f;
    }
}

public class Settings_PauseMenuState : IState
{
    private PauseMenu _PauseMenu;

    public Settings_PauseMenuState(PauseMenu iPauseMenu) => _PauseMenu = iPauseMenu;

    public void Enter()
    {
        GameManager.GM.StartCoroutine(Utility.PopInCanvasGroup(_PauseMenu.Settings_CG, 1f, Utility.TransitionSpeed));
        _PauseMenu.CurrentState = PauseMenu.PauseMenuStates.Settings;
    }

    public void Execute() { }

    public void Exit()
    {
        SaveSystem.SaveSettings(GameManager.GM.Settings);
        GameManager.GM.StartCoroutine(Utility.PopOutCanvasGroup(_PauseMenu.Settings_CG, 1f, Utility.TransitionSpeed));
    }
}

public class ConfirmationPrompt_PauseMenuState : IState
{
    private PauseMenu _PauseMenu;

    public ConfirmationPrompt_PauseMenuState(PauseMenu iPauseMenu) => _PauseMenu = iPauseMenu;

    public void Enter()
    {
        _PauseMenu.CurrentState = PauseMenu.PauseMenuStates.ConfirmationPrompt;
        GameManager.GM.StartCoroutine(Utility.PopOutCanvasGroup(_PauseMenu.PauseMenu_CG, 1f, Utility.TransitionSpeed));
        GameManager.GM.StartCoroutine(Utility.PopInCanvasGroup(_PauseMenu.ConfirmationPrompt_CG, 1f, Utility.TransitionSpeed));
    }

    public void Execute() { }

    public void Exit()
    {
        _PauseMenu.CurrentPromptTarget = PauseMenu.ConfirmationPromptTarget.None;
        GameManager.GM.StartCoroutine(Utility.PopOutCanvasGroup(_PauseMenu.ConfirmationPrompt_CG, 1f, Utility.TransitionSpeed));
    }
}

#endregion