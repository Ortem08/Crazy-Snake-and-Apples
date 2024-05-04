public class StateMachine
{
    public IState currentState;

    public void ChangeState(IState newState)
    {
        currentState = newState;
    }

    public void Update()
    {
        if (currentState != null)
            currentState.Execute();
    }
}