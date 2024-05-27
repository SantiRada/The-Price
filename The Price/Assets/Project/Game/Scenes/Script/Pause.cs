using UnityEngine;

public enum State { Pause, Game, Interface }
public class Pause : MonoBehaviour {

    public static bool inPause;
    public static State state;

    private void Start() { StateChange = State.Game; }
    public static void SetPause(bool value) { inPause = value; }
    public static State StateChange
    {
        get { return state; }
        set { state = value; }
    }
    public static bool Comprobation(State st)
    {
        bool value = false;

        if(LoadingScreen.inLoading || Pause.inPause || Pause.state != st)
        {
            value = true;
        }

        return value;
    }
}
