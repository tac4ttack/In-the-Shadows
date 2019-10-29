
// using UnityEngine;

// public class StateMachine : MonoBehaviour
public class StateMachine
{
    private IState _CurrentState;
    private IState _PreviousState;

    public void ChangeState(IState newState)
    {
        if (_CurrentState != null)
            _CurrentState.Exit();
        _PreviousState = _CurrentState;
        _CurrentState = newState;
        _CurrentState.Enter();
    }

    public void ExecuteState()
    {
        if (_CurrentState != null)
            _CurrentState.Execute();
    }

    public void GoBackToPreviousState()
    {
        if (_PreviousState != null)
            ChangeState(_PreviousState);
    }

    /*
    public void SwitchToPreviousState()
    {
        _CurrentState.Exit();
        _CurrentState = _PreviousState;
        _CurrentState.Enter();
    }
    */
}
