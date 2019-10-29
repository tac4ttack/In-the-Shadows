using UnityEngine;
using UnityEngine.Assertions;

public class PauseMenu : MonoBehaviour
{
    public StateMachine PauseMenuStateMachine = new StateMachine();
    public enum PauseMenuStates { Inactive = 0, Active, Settings, ConfirmationPrompt};
    public PauseMenuStates CurrentState;
    public float TransitionSpeed = 0.1f;

    [Header("Canvas Groups of Pause Menu UI")]
    public CanvasGroup PauseCG;
    public CanvasGroup BackgroundCG;
    public CanvasGroup PauseMenuCG;
    public CanvasGroup SettingsCG;
    public CanvasGroup ConfirmationPromptCG;

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
        if (ConfirmationPromptCG == null)
            ConfirmationPromptCG = this.gameObject.transform.Find("Confirmation_Panel").GetComponent<CanvasGroup>();

        Assert.IsNotNull(PauseCG, "Pause Menu UI Canvas group not found!");
        Assert.IsNotNull(BackgroundCG, "Pause Menu Background Canvas group not found!");
        Assert.IsNotNull(PauseMenuCG, "Pause Menu Panel Canvas group not found!");
        Assert.IsNotNull(SettingsCG, "Settings Panel Canvas group not found!");
        Assert.IsNotNull(ConfirmationPromptCG, "Confirmation Prompt Canvas group not found!");

        // DEBUG TODO!
        // When the main state machine will be done
        // if (levelselection)
        //      disable(restart button)
        //      disable(abort button)
    }

    void Start() => PauseMenuStateMachine.ChangeState(new Inactive_PauseMenuState(this));
    void Update() => PauseMenuStateMachine.ExecuteState();
    
    #region Buttons Logic
    public void ResumeButtonClick()
    {
        Debug.Log("Resume button click!");
        PauseMenuStateMachine.ChangeState(new Inactive_PauseMenuState(this));
    }
    
    public void RestartButtonClick()
    {
        Debug.Log("Restart button click!");
        PauseMenuStateMachine.ChangeState(new ConfirmationPrompt_PauseMenuState(this));
    }

    public void AbortButtonClick()
    {
        Debug.Log("Abort button click!");
        PauseMenuStateMachine.ChangeState(new ConfirmationPrompt_PauseMenuState(this));
    }

    public void ExitButtonClick()
    {
        Debug.Log("Exit button click!");
        PauseMenuStateMachine.ChangeState(new ConfirmationPrompt_PauseMenuState(this));
    }

    public void SettingsButtonClick()
    {
        PauseMenuStateMachine.ChangeState(new Settings_PauseMenuState(this));
    }

    public void SettingsBackButtonClick()
    {
        PauseMenuStateMachine.GoBackToPreviousState();
    }

    public void ConfirmationYesButtonClick()
    {
        Debug.Log("Confirmation yes button clicked!");

    }

    public void ConfirmationNoButtonClick()
    {
        Debug.Log("Confirmation no button clicked!");
        PauseMenuStateMachine.GoBackToPreviousState();
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
        GameManager.GM.StartCoroutine(Utility.PopOutCanvasGroup(_PauseMenu.PauseCG, 1f, _PauseMenu.TransitionSpeed));
        GameManager.GM.StartCoroutine(Utility.PopOutCanvasGroup(_PauseMenu.BackgroundCG, 1f, _PauseMenu.TransitionSpeed));
        GameManager.GM.StartCoroutine(Utility.PopOutCanvasGroup(_PauseMenu.PauseMenuCG, 1f, _PauseMenu.TransitionSpeed));
        _PauseMenu.CurrentState = PauseMenu.PauseMenuStates.Inactive;
    }

    public void Execute()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _PauseMenu.PauseMenuStateMachine.ChangeState(new Active_PauseMenuState(_PauseMenu));
        }
    }

    public void Exit()
    {
        Debug.Log("Inactive PM Exit");
        // add switch back to gamestate logic?
    }
}

public class Active_PauseMenuState : IState
{
    private PauseMenu _PauseMenu;

    public Active_PauseMenuState(PauseMenu iPauseMenu) => _PauseMenu = iPauseMenu;

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
            _PauseMenu.PauseMenuStateMachine.ChangeState(new Inactive_PauseMenuState(_PauseMenu));
        }
    }

    public void Exit()
    {
        GameManager.GM.StartCoroutine(Utility.PopOutCanvasGroup(_PauseMenu.PauseMenuCG, 1f, _PauseMenu.TransitionSpeed));    }
}

public class Settings_PauseMenuState : IState
{
    private PauseMenu _PauseMenu;

    public Settings_PauseMenuState(PauseMenu iPauseMenu) => _PauseMenu = iPauseMenu;

    public void Enter()
    {
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

public class ConfirmationPrompt_PauseMenuState : IState
{
    private PauseMenu _PauseMenu;

    public ConfirmationPrompt_PauseMenuState(PauseMenu iPauseMenu) => _PauseMenu = iPauseMenu;

    public void Enter()
    {
        _PauseMenu.CurrentState = PauseMenu.PauseMenuStates.ConfirmationPrompt;
        GameManager.GM.StartCoroutine(Utility.PopOutCanvasGroup(_PauseMenu.PauseMenuCG, 1f, _PauseMenu.TransitionSpeed));
        GameManager.GM.StartCoroutine(Utility.PopInCanvasGroup(_PauseMenu.ConfirmationPromptCG, 1f, _PauseMenu.TransitionSpeed));
    }

    public void Execute() {}

    public void Exit()
    {
        GameManager.GM.StartCoroutine(Utility.PopOutCanvasGroup(_PauseMenu.ConfirmationPromptCG, 1f, _PauseMenu.TransitionSpeed));
    }
}
#endregion