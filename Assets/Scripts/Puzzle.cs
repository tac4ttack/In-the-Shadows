using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class Puzzle : MonoBehaviour
{
    public StateMachine PuzzleStateMachine = new StateMachine();
    public enum PuzzleStates {Playing = 0, Paused, WinScreen, ConfirmationPrompt};
    public PuzzleStates CurrentState;

    private enum WinScreenConfirmationPromptTarget {None = 0, MainMenu, LevelSelection, Restart};
    private WinScreenConfirmationPromptTarget _CurrentWinScreenConfirmationPromptTarget;

    public Camera WinScreen_Cam;
    private PauseMenu _PauseMenuUI;
    
    [SerializeField] private PuzzlePiece[] _PuzzlePieces;
    [SerializeField] private bool _PuzzleValidated = false;
    
    // DEBUG
    public Light            DebugSpot;

    void Awake()
    {
        if (WinScreen_Cam == null)
            WinScreen_Cam = GameObject.FindWithTag("InGame_WinScreen_Camera").GetComponent<Camera>();
        Assert.IsNotNull(WinScreen_Cam, "Win Screen Camera not found in scene!");

        if (_PauseMenuUI == null)
            _PauseMenuUI = GameObject.FindGameObjectWithTag("PauseMenu_UI").GetComponent<PauseMenu>();
        Assert.IsNotNull(_PauseMenuUI, "Pause Menu UI not found in scene!");

        GameObject[] tmp = GameObject.FindGameObjectsWithTag("InGame_PuzzlePiece");
        _PuzzlePieces = new PuzzlePiece[tmp.Length];
        for (int i = 0; i < tmp.Length; i++)
            _PuzzlePieces[i] = tmp[i].GetComponent<PuzzlePiece>();
        Assert.IsNotNull(_PuzzlePieces, "No Puzzle pieces found in scene!");
    }

    void Start()
    {
        PuzzleStateMachine.ChangeState(new Playing_PuzzleState(this));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
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

        // DEBUG
        if (_PuzzleValidated && CurrentState == PuzzleStates.Playing)
        {
        //     DebugSpot.color = Color.green;
        //     WinScreen_Cam.enabled = true;
            PuzzleStateMachine.ChangeState(new WinScreen_PuzzleState(this));
        }
        // else
        // {
        //     DebugSpot.color = Color.white;
        //     WinScreen_Cam.enabled = false;
        // }
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

    public WinScreen_PuzzleState(Puzzle iPuzzleScript) => _PuzzleScript = iPuzzleScript;

    public void Enter()
    {
        _PuzzleScript.CurrentState = Puzzle.PuzzleStates.WinScreen;
        _PuzzleScript.WinScreen_Cam.enabled = true;
    }

    public void Execute() {}

    public void Exit() {}
}

public class ConfirmationPrompt_PuzzleState : IState
{
    private Puzzle _PuzzleScript;

    public ConfirmationPrompt_PuzzleState(Puzzle iPuzzleScript) => _PuzzleScript = iPuzzleScript;

    public void Enter()
    {
        _PuzzleScript.CurrentState = Puzzle.PuzzleStates.ConfirmationPrompt;
    }

    public void Execute() {}

    public void Exit() {}
}
#endregion
