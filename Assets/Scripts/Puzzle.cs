using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class Puzzle : MonoBehaviour
{
    public StateMachine PuzzleStateMachine = new StateMachine();
    public enum PuzzleStates {Playing = 0, Paused, WinScreen, ConfirmationPrompt};
    public PuzzleStates CurrentState;

    public enum ConfirmationPromptTarget {None = 0, MainMenu, LevelSelection, Restart};
    public ConfirmationPromptTarget CurrentPromptTarget;

    private Camera _WinScreen_Cam;
    private PauseMenu _PauseMenuUI;
    private PostProcessProfile _PostProcess;
    private CanvasGroup _WinScreen_CG;
    private CanvasGroup _ConfirmationPrompt_CG;

    private Button _WinScreen_MainMenu_BTN;
    private Button _WinScreen_LevelSelection_BTN;
    private Button _WinScreen_Restart_BTN;
    private Button _WinScreen_NextLevel_BTN;
    private Button _WinScreen_Prompt_Yes_BTN;
    private Button _WinScreen_Prompt_No_BTN;

    [SerializeField] private PuzzlePiece[] _PuzzlePieces;
    [SerializeField] private bool _PuzzleValidated = false;

    void Awake()
    {
        if (_WinScreen_Cam == null)
            _WinScreen_Cam = GameObject.FindWithTag("InGame_WinScreen_Camera").GetComponent<Camera>();
        Assert.IsNotNull(_WinScreen_Cam, "Win Screen Camera not found in scene!");

        if (_PauseMenuUI == null)
            _PauseMenuUI = GameObject.FindGameObjectWithTag("PauseMenu_UI").GetComponent<PauseMenu>();
        Assert.IsNotNull(_PauseMenuUI, "Pause Menu UI not found in scene!");
    
        if (_PostProcess == null)
            _PostProcess = GameObject.FindGameObjectWithTag("InGame_PostProcess").GetComponent<PostProcessVolume>().profile;
        Assert.IsNotNull(_PostProcess, "Post Process Volume not found in scene!");
        _PostProcess.GetSetting<DepthOfField>().focusDistance.value = 3f;

        if (_WinScreen_CG == null)
            _WinScreen_CG = GameObject.FindGameObjectWithTag("InGame_WinScreen_UI").GetComponent<CanvasGroup>();
        Assert.IsNotNull(_WinScreen_CG, "Win Screen UI not found in scene!");

        if (_ConfirmationPrompt_CG == null)
            _ConfirmationPrompt_CG = GameObject.FindGameObjectWithTag("InGame_WinScreen_ConfirmationPrompt").GetComponent<CanvasGroup>();
        Assert.IsNotNull(_ConfirmationPrompt_CG, "Confirmation Prompt Canvas group not found!");

        if (_WinScreen_MainMenu_BTN == null)
            _WinScreen_MainMenu_BTN = GameObject.FindGameObjectWithTag("InGame_WinScreen_MainMenu_Button").GetComponent<Button>();
        Assert.IsNotNull(_WinScreen_MainMenu_BTN, "Win screen Main Menu button not found in scene!");

        if (_WinScreen_LevelSelection_BTN == null)
            _WinScreen_LevelSelection_BTN = GameObject.FindGameObjectWithTag("InGame_WinScreen_LevelSelection_Button").GetComponent<Button>();
        Assert.IsNotNull(_WinScreen_LevelSelection_BTN, "Win screen Level Selection button not found in scene!");
        
        if (_WinScreen_Restart_BTN == null)
            _WinScreen_Restart_BTN = GameObject.FindGameObjectWithTag("InGame_WinScreen_Restart_Button").GetComponent<Button>();
        Assert.IsNotNull(_WinScreen_Restart_BTN, "Win screen Restart button not found in scene!");
        
        if (_WinScreen_NextLevel_BTN == null)
            _WinScreen_NextLevel_BTN = GameObject.FindGameObjectWithTag("InGame_WinScreen_NextLevel_Button").GetComponent<Button>();
        Assert.IsNotNull(_WinScreen_NextLevel_BTN, "Win screen Next Level button not found in scene!");

        if (_WinScreen_Prompt_Yes_BTN == null)
            _WinScreen_Prompt_Yes_BTN = GameObject.FindGameObjectWithTag("InGame_WinScreen_Yes_Button").GetComponent<Button>();
        Assert.IsNotNull(_WinScreen_Prompt_Yes_BTN, "Win screen Yes confirmation prompt button not found in scene!");
        if (_WinScreen_Prompt_No_BTN == null)
            _WinScreen_Prompt_No_BTN = GameObject.FindGameObjectWithTag("InGame_WinScreen_No_Button").GetComponent<Button>();
        Assert.IsNotNull(_WinScreen_Prompt_No_BTN, "Win screen No confirmation prompt button not found in scene!");

        GameObject[] tmp = GameObject.FindGameObjectsWithTag("InGame_PuzzlePiece");
        _PuzzlePieces = new PuzzlePiece[tmp.Length];
        for (int i = 0; i < tmp.Length; i++)
            _PuzzlePieces[i] = tmp[i].GetComponent<PuzzlePiece>();
        Assert.IsNotNull(_PuzzlePieces, "No Puzzle pieces found in scene!");
    }

    void Start()
    {
        _WinScreen_MainMenu_BTN.onClick.AddListener(delegate{QuitButtonPress();});
        _WinScreen_LevelSelection_BTN.onClick.AddListener(delegate{LevelSelectButtonPress();});
        _WinScreen_Restart_BTN.onClick.AddListener(delegate{RestartButtonPress();});
        _WinScreen_NextLevel_BTN.onClick.AddListener(delegate{NextLevelButtonPress();});
        _WinScreen_Prompt_Yes_BTN.onClick.AddListener(delegate{ConfirmationYesButtonPress();});
        _WinScreen_Prompt_No_BTN.onClick.AddListener(delegate{ConfirmationNoButtonPress();});
        PuzzleStateMachine.ChangeState(new Playing_PuzzleState(this));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && CurrentState != PuzzleStates.WinScreen)
        {
            switch (_PauseMenuUI.CurrentState)
            {
                case PauseMenu.PauseMenuStates.Inactive:
                {
                    PuzzleStateMachine.ChangeState(new Paused_PuzzleState(this));
                    _PauseMenuUI.PauseMenuStateMachine.ChangeState((new Active_PauseMenuState(_PauseMenuUI)));
                }
                    break;
                case PauseMenu.PauseMenuStates.Active:
                {
                    PuzzleStateMachine.ChangeState(new Playing_PuzzleState(this));
                    _PauseMenuUI.PauseMenuStateMachine.ChangeState((new Inactive_PauseMenuState(_PauseMenuUI)));
                }
                    break;
                case PauseMenu.PauseMenuStates.Settings:
                {
                    _PauseMenuUI.PauseMenuStateMachine.GoBackToPreviousState();
                }
                    break;
            }
        }

        CheckPuzzlePieces();
        if (_PuzzleValidated && CurrentState == PuzzleStates.Playing && CurrentState != PuzzleStates.WinScreen)
        {
            PuzzleStateMachine.ChangeState(new WinScreen_PuzzleState(this, _WinScreen_Cam, _WinScreen_CG, _PostProcess));
        }
    }

    void CheckPuzzlePieces()
    {
        bool   tmp = false;
        for (int i = 0; i < _PuzzlePieces.Length; i++)
        {
            PuzzlePiece p = (PuzzlePiece)_PuzzlePieces[i];
            tmp = p.isPuzzlePieceValidated;
        }
        _PuzzleValidated = tmp;
    }

    #region Buttons Logic

    public void ConfirmationYesButtonPress()
    {
        switch (CurrentPromptTarget)
        {
            case Puzzle.ConfirmationPromptTarget.MainMenu:
            {
                GameManager.GM.GameStateMachine.ChangeState(new InMainMenu_GameState());
            }
                break;
            case Puzzle.ConfirmationPromptTarget.Restart:
            {
                GameManager.GM.GameStateMachine.ChangeState(new InGame_GameState(SceneManager.GetActiveScene().buildIndex));
            }    
                break;
            case Puzzle.ConfirmationPromptTarget.LevelSelection:
            {
                GameManager.GM.GameStateMachine.ChangeState(new LevelSelection_GameState());
            }    
                break;
            default:
                break;
        }
        GameManager.GM.PushLevelComplete();
        GameManager.GM.PushLevelUnlock();
    }

    public void ConfirmationNoButtonPress()
    {
        PuzzleStateMachine.GoBackToPreviousState();
    }

    public void QuitButtonPress()
    {
        CurrentPromptTarget = Puzzle.ConfirmationPromptTarget.MainMenu;
        PuzzleStateMachine.ChangeState(new ConfirmationPrompt_PuzzleState(this, _ConfirmationPrompt_CG));
    }

    public void LevelSelectButtonPress()
    {
        CurrentPromptTarget = Puzzle.ConfirmationPromptTarget.LevelSelection;
        PuzzleStateMachine.ChangeState(new ConfirmationPrompt_PuzzleState(this, _ConfirmationPrompt_CG));
    }

    public void RestartButtonPress()
    {
        CurrentPromptTarget = Puzzle.ConfirmationPromptTarget.Restart;
        PuzzleStateMachine.ChangeState(new ConfirmationPrompt_PuzzleState(this, _ConfirmationPrompt_CG));        
    }

    public void NextLevelButtonPress()
    {
        //DEBUG
        GameManager.GM.GameStateMachine.ChangeState(new InGame_GameState(2));
        // GameManager.GM.LoadNextLevel();
        // GameManager.GM.GameStateMachine.ChangeState(new InGame_GameState(SceneManager.GetActiveScene().buildIndex));
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    #endregion
}

#region Puzzle States

public class Playing_PuzzleState : IState
{
    private Puzzle _PuzzleScript;

    public Playing_PuzzleState(Puzzle iPuzzleScript)  => _PuzzleScript = iPuzzleScript;

    public void Enter()
    {
        _PuzzleScript.CurrentState = Puzzle.PuzzleStates.Playing;
    }

    public void Execute() {}

    public void Exit() {}
}

public class Paused_PuzzleState : IState
{
    private Puzzle _PuzzleScript;

    public Paused_PuzzleState(Puzzle iPuzzleScript) => _PuzzleScript = iPuzzleScript;

    public void Enter()
    {
        _PuzzleScript.CurrentState = Puzzle.PuzzleStates.Paused;
    }

    public void Execute() {}

    public void Exit()
    {  
        _PuzzleScript.CurrentState = Puzzle.PuzzleStates.Playing;
    }
}

public class WinScreen_PuzzleState : IState
{
    private Puzzle _PuzzleScript;
    private Camera _WinCam;
    private CanvasGroup _WinScreen_CG;
    private PostProcessProfile _PostProcess;

    public WinScreen_PuzzleState(Puzzle iPuzzleScript, Camera iWinCam, CanvasGroup iWinScreenCG, PostProcessProfile iPostProcess)
    {
        _PuzzleScript = iPuzzleScript;
        _WinCam = iWinCam;
        _WinScreen_CG = iWinScreenCG;
        _PostProcess = iPostProcess;
    } 

    public void Enter()
    {
        _PuzzleScript.CurrentState = Puzzle.PuzzleStates.WinScreen;
        _WinCam.enabled = true;
        GameManager.GM.StartCoroutine(Utility.PopInCanvasGroup(_WinScreen_CG, 1f, Utility.TransitionSpeed));
        _PostProcess.GetSetting<DepthOfField>().focusDistance.value = 0.1f;
    }

    public void Execute() {}

    public void Exit() {}
}

public class ConfirmationPrompt_PuzzleState : IState
{
    private Puzzle _PuzzleScript;
    private CanvasGroup _ConfirmationPrompt;

    public ConfirmationPrompt_PuzzleState(Puzzle iPuzzleScript, CanvasGroup iConfirmationPrompt)
    {
        _PuzzleScript = iPuzzleScript;
        _ConfirmationPrompt = iConfirmationPrompt;
    }

    public void Enter()
    {
        _PuzzleScript.CurrentState = Puzzle.PuzzleStates.ConfirmationPrompt;
        GameManager.GM.StartCoroutine(Utility.PopInCanvasGroup(_ConfirmationPrompt, 1f, Utility.TransitionSpeed));
    }

    public void Execute() {}

    public void Exit()
    {
        _PuzzleScript.CurrentPromptTarget = Puzzle.ConfirmationPromptTarget.None;
        GameManager.GM.StartCoroutine(Utility.PopOutCanvasGroup(_ConfirmationPrompt, 1f, Utility.TransitionSpeed));
    }
}
#endregion
