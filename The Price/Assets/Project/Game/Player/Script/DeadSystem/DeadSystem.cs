using UnityEngine;
using UnityEngine.SceneManagement;

public enum Worlds { Terrenal, Cielo, Infierno, Astral, Inframundo }
public class DeadSystem : MonoBehaviour {

    [Header("Prev Data")]
    public int terrenal;
    public int celestial;
    public int infernal;
    public int astral;
    public int inframundo;

    [Header("Current Data")]
    public Worlds currentWorld;
    public Worlds nextWorld;

    private void Start() { if (PlayerPrefs.GetInt("CurrentWorld", 0) != 0) currentWorld = (Worlds)PlayerPrefs.GetInt("CurrentWorld", 0); }
    public void DiePlayer()
    {
        int aligment = LunarCycle.CalculateNextWorld();

        if(aligment != -1)
        {
            // ESTAS ALINEADO A UN PUNTO DEL CICLO LUNAR
            nextWorld = (Worlds)aligment;
        }
        else
        {
            if (currentWorld == Worlds.Terrenal)
            {
                terrenal++;
                nextWorld = Worlds.Cielo;
            }
            if (currentWorld == Worlds.Cielo)
            {
                celestial++;
                nextWorld = Worlds.Infierno;
            }
            if (currentWorld == Worlds.Infierno)
            {
                infernal++;
                nextWorld = Worlds.Inframundo;
            }
        }

        PlayerPrefs.SetInt("CurrentWorld", (int)nextWorld);

        // SE SUMA 1 PARA SALTAR LA ESCENA DEL MENÚ
        SceneManager.LoadScene(nextWorld.ToString());
    }
}
