using UnityEngine;

public enum State { Pause, Game, Interface }
public class Pause : MonoBehaviour {

    public static State state;

    private void Start() { StateChange = State.Game; }

    /// <summary>
    /// Cambia el estado de pausa del juego. NO usar durante loading.
    /// </summary>
    public static void SetPause(bool isPaused)
    {
        if (LoadingScreen.inLoading) return; // No cambiar estado durante loading

        state = isPaused ? State.Pause : State.Game;
    }

    public static State StateChange
    {
        get { return state; }
        set { state = value; }
    }

    /// <summary>
    /// Verifica si el juego NO está en el estado especificado (o está en loading/pausa)
    /// </summary>
    public static bool Comprobation(State st)
    {
        return LoadingScreen.inLoading || state != st;
    }

    /// <summary>
    /// Verifica si el juego está actualmente jugable (no en loading, no en pausa, no en interfaz)
    /// </summary>
    public static bool IsGamePlaying()
    {
        return !LoadingScreen.inLoading && state == State.Game;
    }
}
