using UnityEngine;
using UnityEngine.SceneManagement;

public enum Worlds { Terrenal, Cielo, Infierno, Astral, Inframundo }
public class DeadSystem : MonoBehaviour {

    [Header("Prev Data")]
    [HideInInspector] public int wasInTerrenal;
    [HideInInspector] public int wasInCielo;
    [HideInInspector] public int wasInInfierno;
    [HideInInspector] public int wasInAstral;
    [HideInInspector] public int wasInInframundo;
    [Space]
    [HideInInspector] public int deadInTerrenal;
    [HideInInspector] public int deadInCielo;
    [HideInInspector] public int deadInInfierno;
    [HideInInspector] public int deadInInframundo;

    [Header("Bosses")]
    [HideInInspector] public int killedMBoss_Terrenal;
    [HideInInspector] public int killedMBoss_Cielo;
    [HideInInspector] public int killedMBoss_Infierno;
    [HideInInspector] public int killedMBoss_Inframundo;
    [Space]
    [HideInInspector] public int killedBoss_Terrenal;
    [HideInInspector] public int killedBoss_Cielo;
    [HideInInspector] public int killedBoss_Infierno;
    [Space]
    [HideInInspector] public int killedMaxBoss_Terrenal;
    [HideInInspector] public int killedMaxBoss_Cielo;
    [HideInInspector] public int killedMaxBoss_Infierno;
    [Space]
    [HideInInspector] public bool killedThanatos = false;

    [Header("Current Data")]
    public Worlds currentWorld;
    [HideInInspector] public Worlds nextWorld;

    private SaveLoadManager _saveLoad;

    private void Awake() { _saveLoad = FindAnyObjectByType<SaveLoadManager>(); }
    private void Start()
    {
        _saveLoad.Invoke("LoadData", 0.35f);

        if (_saveLoad.GetWorldData() != null) if(_saveLoad.GetWorldData().currentWorld != (int)currentWorld) SceneManager.LoadScene(_saveLoad.GetWorldData().currentWorld);

        // ---- SUMAR UNA CANTIDAD DE ENTRADAS AL PLANO ACTUAL ---- //
        switch (currentWorld)
        {
            case Worlds.Terrenal: wasInTerrenal++; break;
            case Worlds.Cielo: wasInCielo++; break;
            case Worlds.Infierno: wasInInfierno++; break;
            case Worlds.Astral: wasInAstral++; break;
            case Worlds.Inframundo: wasInInframundo++; break;
        }
    }
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
                deadInTerrenal++;
                nextWorld = Worlds.Cielo;
            }
            if (currentWorld == Worlds.Cielo)
            {
                deadInCielo++;
                nextWorld = Worlds.Infierno;
            }
            if (currentWorld == Worlds.Infierno)
            {
                deadInInfierno++;
                nextWorld = Worlds.Inframundo;
            }
            if (currentWorld == Worlds.Inframundo)
            {
                deadInInframundo++;
                nextWorld = Worlds.Astral;
            }
        }

        currentWorld = nextWorld;
        _saveLoad.SaveData();

        // SE SUMA 1 PARA SALTAR LA ESCENA DEL MENÚ
        SceneManager.LoadScene(nextWorld.ToString());
    }
}
