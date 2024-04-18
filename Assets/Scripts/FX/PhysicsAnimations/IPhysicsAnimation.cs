
/// <summary>
/// Transform based animations for the sword, separate from any actual animation controllers
/// </summary>
public interface IPhysicsAnimation {

    public bool IsRunning { get; }

    void StartAnimation();

    void StopAnimation();

}
