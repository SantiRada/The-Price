using UnityEngine;

public enum State { Pause, Game, Interface }
public class Pause : MonoBehaviour {

    public static bool inPause;
    public static State state;

    public static void SetPause(bool value) { inPause = value; }
    public static State StateChange
    {
        get { return state; }
        set { state = value; }
    }
}
