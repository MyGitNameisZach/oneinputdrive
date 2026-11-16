using UnityEngine;

//Tracks whether the run has started. Defaults to "playing" so the game still works if no start screen is shown
public static class GameState
{
    public static bool IsPlaying { get; private set; } = true;

    //The frame the run started on. Input on this frame is ignored so the start key press doesn't also change lane
    public static int StartedFrame { get; private set; } = -1;

    public static void WaitForStart()
    {
        IsPlaying = false;
        Time.timeScale = 0.0f;
    }

    public static void Begin()
    {
        IsPlaying = true;
        StartedFrame = Time.frameCount;
        Time.timeScale = 1.0f;
    }

    //Reset statics when entering play mode, in case domain reload is turned off in the editor
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics()
    {
        IsPlaying = true;
        StartedFrame = -1;
    }
}
