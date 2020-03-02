using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class Puzzle : MonoBehaviour
{
    public StateMachine PuzzleStateMachine = new StateMachine();
    public enum PuzzleStates { Playing = 0, Paused, WinScreen, ConfirmationPrompt };
    public PuzzleStates CurrentState;

    public enum ConfirmationPromptTarget { None = 0, MainMenu, LevelSelection, Restart };
    public ConfirmationPromptTarget CurrentPromptTarget;

    private Camera _WinScreen_Cam;
    private PauseMenu _PauseMenuUI;
    private PostProcessProfile _PostProcess;
    private CanvasGroup _WinScreen_CG;
    private CanvasGroup _WinScreen_Panel_CG;
    private CanvasGroup _WinScreen_Background_CG;
    private CanvasGroup _ConfirmationPrompt_CG;

    private Button _WinScreen_MainMenu_BTN;
    private Button _WinScreen_LevelSelection_BTN;
    private Button _WinScreen_Restart_BTN;
    private Button _WinScreen_NextLevel_BTN;
    private Button _WinScreen_Prompt_Yes_BTN;
    private Button _WinScreen_Prompt_No_BTN;
    private ConfettisSpawner _WinScreen_ConfettisSpawner;
    public bool WinScreen_FirstPop = true;

    [SerializeField] private PuzzlePiece[] _PuzzlePieces;
    [SerializeField] private bool _PuzzleValidated = false;

    void Awake()
    {
        // DEBUG
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Log($"PUZZLE - {this.name} - Awake()");
        #endif

        if (_WinScreen_Cam == null)
            _WinScreen_Cam = GameObject.FindWithTag("In Game/Win Screen/Camera").GetComponent<Camera>();
        Assert.IsNotNull(_WinScreen_Cam, "Win Screen Camera not found in scene!");

        if (_PauseMenuUI == null)
            _PauseMenuUI = GameObject.FindGameObjectWithTag("Pause Menu/UI").GetComponent<PauseMenu>();
        Assert.IsNotNull(_PauseMenuUI, "Pause Menu UI not found in scene!");

        if (_PostProcess == null)
            _PostProcess = GameObject.FindGameObjectWithTag("In Game/Post Process").GetComponent<PostProcessVolume>().profile;
        Assert.IsNotNull(_PostProcess, "Post Process Volume not found in scene!");
        _PostProcess.GetSetting<DepthOfField>().focusDistance.value = 3f;

        if (_WinScreen_CG == null)
            _WinScreen_CG = GameObject.FindGameObjectWithTag("In Game/Win Screen/UI").GetComponent<CanvasGroup>();
        Assert.IsNotNull(_WinScreen_CG, "Win Screen UI not found in scene!");

        if (_WinScreen_Panel_CG == null)
            _WinScreen_Panel_CG = GameObject.FindGameObjectWithTag("In Game/Win Screen/Panel").GetComponent<CanvasGroup>();
        Assert.IsNotNull(_WinScreen_Panel_CG, "Win Screen UI panel canvas group not found in scene!");

        if (_WinScreen_Background_CG == null)
            _WinScreen_Background_CG = GameObject.FindGameObjectWithTag("In Game/Win Screen/Background").GetComponent<CanvasGroup>();
        Assert.IsNotNull(_WinScreen_Background_CG, "Win Screen background canvas not found in scene!");

        if (_ConfirmationPrompt_CG == null)
            _ConfirmationPrompt_CG = GameObject.FindGameObjectWithTag("In Game/Win Screen/Confirmation Prompt").GetComponent<CanvasGroup>();
        Assert.IsNotNull(_ConfirmationPrompt_CG, "Confirmation Prompt Canvas group not found!");

        if (_WinScreen_MainMenu_BTN == null)
            _WinScreen_MainMenu_BTN = GameObject.FindGameObjectWithTag("In Game/Win Screen/Main Menu Button").GetComponent<Button>();
        Assert.IsNotNull(_WinScreen_MainMenu_BTN, "Win screen Main Menu button not found in scene!");

        if (_WinScreen_LevelSelection_BTN == null)
            _WinScreen_LevelSelection_BTN = GameObject.FindGameObjectWithTag("In Game/Win Screen/Level Selection Button").GetComponent<Button>();
        Assert.IsNotNull(_WinScreen_LevelSelection_BTN, "Win screen Level Selection button not found in scene!");

        if (_WinScreen_Restart_BTN == null)
            _WinScreen_Restart_BTN = GameObject.FindGameObjectWithTag("In Game/Win Screen/Restart Button").GetComponent<Button>();
        Assert.IsNotNull(_WinScreen_Restart_BTN, "Win screen Restart button not found in scene!");

        if (_WinScreen_NextLevel_BTN == null)
            _WinScreen_NextLevel_BTN = GameObject.FindGameObjectWithTag("In Game/Win Screen/Next Level Button").GetComponent<Button>();
        Assert.IsNotNull(_WinScreen_NextLevel_BTN, "Win screen Next Level button not found in scene!");

        if (_WinScreen_Prompt_Yes_BTN == null)
            _WinScreen_Prompt_Yes_BTN = GameObject.FindGameObjectWithTag("In Game/Win Screen/Yes Button").GetComponent<Button>();
        Assert.IsNotNull(_WinScreen_Prompt_Yes_BTN, "Win screen Yes confirmation prompt button not found in scene!");
        
        if (_WinScreen_Prompt_No_BTN == null)
            _WinScreen_Prompt_No_BTN = GameObject.FindGameObjectWithTag("In Game/Win Screen/No Button").GetComponent<Button>();
        Assert.IsNotNull(_WinScreen_Prompt_No_BTN, "Win screen No confirmation prompt button not found in scene!");

        if (_WinScreen_ConfettisSpawner == null)
            _WinScreen_ConfettisSpawner = GameObject.FindGameObjectWithTag("In Game/Confettis Spawner").GetComponent<ConfettisSpawner>();
        Assert.IsNotNull(_WinScreen_ConfettisSpawner, "Win screen confettis spawner not found in scene!");

        GameObject[] tmp = GameObject.FindGameObjectsWithTag("In Game/Puzzle Piece");
        _PuzzlePieces = new PuzzlePiece[tmp.Length];
        for (int i = 0; i < tmp.Length; i++)
            _PuzzlePieces[i] = tmp[i].GetComponent<PuzzlePiece>();
        Assert.IsNotNull(_PuzzlePieces, "No Puzzle pieces found in scene!");

        _WinScreen_MainMenu_BTN.onClick.AddListener(delegate { QuitButtonPress(); });
        _WinScreen_LevelSelection_BTN.onClick.AddListener(delegate { LevelSelectButtonPress(); });
        _WinScreen_Restart_BTN.onClick.AddListener(delegate { RestartButtonPress(); });
        _WinScreen_NextLevel_BTN.onClick.AddListener(delegate { NextLevelButtonPress(); });
        _WinScreen_Prompt_Yes_BTN.onClick.AddListener(delegate { ConfirmationYesButtonPress(); });
        _WinScreen_Prompt_No_BTN.onClick.AddListener(delegate { ConfirmationNoButtonPress(); });
        
    }

    void Start()
    {
        // DEBUG
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Log($"PUZZLE - {this.name} - Start()");
        #endif

        if (Utility.CurrentLevelIndex + 1 >= Utility.PuzzleAmount)
            _WinScreen_NextLevel_BTN.interactable = false;
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

        _PuzzleValidated = Utility.CheckPuzzlePieces(_PuzzlePieces);

        if (_PuzzleValidated && CurrentState == PuzzleStates.Playing && CurrentState != PuzzleStates.WinScreen)
        {
            PuzzleStateMachine.ChangeState(new WinScreen_PuzzleState(this, _WinScreen_Cam, _WinScreen_CG, _WinScreen_Panel_CG, _WinScreen_Background_CG, _PostProcess, _WinScreen_ConfettisSpawner));
        }
    }

    public void PushLevelComplete()
    {
        GameManager.GM.Players.ToComplete[Utility.CurrentPlayer].q.Add(Utility.CurrentLevelIndex);
    }

    public void PushLevelUnlock()
    {
        int tmp = Utility.CurrentLevelIndex + 1;

        if (tmp >= Utility.LevelSceneIndexOffset + Utility.PuzzleAmount)
        {
            // DEBUG
            #if UNITY_EDITOR
            Debug.Log("Should launch the end game!");
            #endif
            
            ;
        }
        else
        {
            GameManager.GM.Players.ToUnlock[Utility.CurrentPlayer].q.Add(tmp);
        }
    }

    #region Buttons Logic

    public void ConfirmationYesButtonPress()
    {
        GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[1]);
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
    }

    public void ConfirmationNoButtonPress()
    {
        GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[1]);        
        PuzzleStateMachine.GoBackToPreviousState();
    }

    public void QuitButtonPress()
    {
        GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[1]);        
        CurrentPromptTarget = Puzzle.ConfirmationPromptTarget.MainMenu;
        PuzzleStateMachine.ChangeState(new ConfirmationPrompt_PuzzleState(this, _ConfirmationPrompt_CG));
    }

    public void LevelSelectButtonPress()
    {
        GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[1]);
        CurrentPromptTarget = Puzzle.ConfirmationPromptTarget.LevelSelection;
        PuzzleStateMachine.ChangeState(new ConfirmationPrompt_PuzzleState(this, _ConfirmationPrompt_CG));
    }

    public void RestartButtonPress()
    {
        GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[1]);
        CurrentPromptTarget = Puzzle.ConfirmationPromptTarget.Restart;
        PuzzleStateMachine.ChangeState(new ConfirmationPrompt_PuzzleState(this, _ConfirmationPrompt_CG));
    }

    public void NextLevelButtonPress()
    {
        GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[1]);
        GameManager.GM.GameStateMachine.ChangeState(new InGame_GameState(SceneManager.GetActiveScene().buildIndex + 1));
    }

    #endregion
}

#region Puzzle States

public class Playing_PuzzleState : IState
{
    private Puzzle _PuzzleScript;

    public Playing_PuzzleState(Puzzle iPuzzleScript) => _PuzzleScript = iPuzzleScript;

    public void Enter()
    {
        _PuzzleScript.CurrentState = Puzzle.PuzzleStates.Playing;
    }

    public void Execute() { }

    public void Exit() { }
}

public class Paused_PuzzleState : IState
{
    private Puzzle _PuzzleScript;

    public Paused_PuzzleState(Puzzle iPuzzleScript) => _PuzzleScript = iPuzzleScript;

    public void Enter()
    {
        _PuzzleScript.CurrentState = Puzzle.PuzzleStates.Paused;
    }

    public void Execute() { }

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
    private CanvasGroup _WinScreen_Panel_CG;
    private CanvasGroup _WinScreen_Background_CG;
    private PostProcessProfile _PostProcess;
    private ConfettisSpawner _ConfettisSpawner;

    public WinScreen_PuzzleState(Puzzle iPuzzleScript,
                                Camera iWinCam,
                                CanvasGroup iWinScreenCG,
                                CanvasGroup iWinScreen_Panel_CG,
                                CanvasGroup iWinScreenBackgroundCG,
                                PostProcessProfile iPostProcess,
                                ConfettisSpawner iConfettisSpawner)
    {
        _PuzzleScript = iPuzzleScript;
        _WinCam = iWinCam;
        _WinScreen_CG = iWinScreenCG;
        _WinScreen_Panel_CG = iWinScreen_Panel_CG;
        _WinScreen_Background_CG = iWinScreenBackgroundCG;
        _PostProcess = iPostProcess;
        _ConfettisSpawner = iConfettisSpawner;
    }

    public void Enter()
    {
        _PuzzleScript.CurrentState = Puzzle.PuzzleStates.WinScreen;
        _WinCam.enabled = true;
        _WinScreen_Panel_CG.interactable = true;        
        _WinScreen_Panel_CG.blocksRaycasts = true;
        _WinScreen_Background_CG.blocksRaycasts = true;
        _WinScreen_Background_CG.interactable = true;
        GameManager.GM.StartCoroutine(Utility.PopInCanvasGroup(_WinScreen_Background_CG, 1f, Utility.TransitionSpeed));        
        GameManager.GM.StartCoroutine(Utility.PopInCanvasGroup(_WinScreen_CG, 1f, Utility.TransitionSpeed));
        _PostProcess.GetSetting<DepthOfField>().focusDistance.value = 0.1f;
        _ConfettisSpawner.enabled = true;

        int tmp = Utility.CurrentLevelIndex;

        /* Player Data update */
        GameManager.GM.Players.LastPlayedLevel[Utility.CurrentPlayer] = tmp;
        
        if (tmp + 1 < Utility.PuzzleAmount && GameManager.GM.Players.Progression[Utility.CurrentPlayer].Level[tmp + 1] == 0)
        {
            _PuzzleScript.PushLevelUnlock();
            GameManager.GM.Players.Progression[Utility.CurrentPlayer].Level[tmp + 1] = 1;
        }   

        if (GameManager.GM.Players.Progression[Utility.CurrentPlayer].Level[tmp] < 2)
            _PuzzleScript.PushLevelComplete();

        GameManager.GM.Players.Progression[Utility.CurrentPlayer].Level[tmp] = 2;

        if (_PuzzleScript.WinScreen_FirstPop)
        {
            GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[10]);
            GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[11]);
        }

        _PuzzleScript.WinScreen_FirstPop = false;
    }

    public void Execute() { }

    public void Exit()
    {
        _WinScreen_Background_CG.blocksRaycasts = false;
        _WinScreen_Background_CG.interactable = false;
        GameManager.GM.SM.SfxSrc.Stop();
    }
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

    public void Execute() { }

    public void Exit()
    {
        _PuzzleScript.CurrentPromptTarget = Puzzle.ConfirmationPromptTarget.None;
        GameManager.GM.StartCoroutine(Utility.PopOutCanvasGroup(_ConfirmationPrompt, 1f, Utility.TransitionSpeed));
    }
}
#endregion