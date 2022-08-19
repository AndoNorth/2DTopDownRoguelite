public interface IState
{
    // string name for debug
    string StateName();
    // update state
    void Tick();
    // on enter state
    void OnEnter();
    // on exit state
    void OnExit();
}
