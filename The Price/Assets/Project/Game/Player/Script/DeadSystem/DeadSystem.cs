using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Worlds { Terrenal, Cielo, Infierno, Astral, Inframundo }
public class DeadSystem : MonoBehaviour {

    [Header("Prev Data")]
    public int wasInTerrenal;
    public int wasInCielo;
    public int wasInInfierno;
    public int wasInAstral;
    public int wasInInframundo;
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
    public Worlds nextWorld;

    private SaveLoadManager _saveLoad;
    private bool isActive = false;

    private void Awake() { _saveLoad = FindAnyObjectByType<SaveLoadManager>(); }
    private void Start() { StartCoroutine("LoadInfo"); }
    private IEnumerator LoadInfo()
    {
        yield return new WaitForSeconds(0.15f);

        _saveLoad.LoadData();

        yield return new WaitForSeconds(0.1f);

        if(_saveLoad.GetWorldData() != null)
        {
            if (_saveLoad.GetWorldData().passedTutorial)
            {
                if (SceneManager.GetActiveScene().name != currentWorld.ToString()) SceneManager.LoadScene(currentWorld.ToString());
            }
            else
            {
                if (_saveLoad.GetWorldData().reasonSave == ReasonSave.closeGame)
                {
                    File.Delete(Application.persistentDataPath + "/World.json");
                    File.Delete(Application.persistentDataPath + "/Player.json");

                    SceneManager.LoadScene("Terrenal");
                }
            }

            currentWorld = (Worlds)_saveLoad.GetWorldData().currentWorld;
        }
    }
    public void DiePlayer()
    {
        if (isActive) return;

        int aligment = LunarCycle.CalculateNextWorld();

        if (aligment != -1)
        {
            // ESTAS ALINEADO A UN PUNTO DEL CICLO LUNAR
            nextWorld = (Worlds)aligment;
        }
        else
        {
            if (currentWorld == Worlds.Terrenal)
            {
                wasInTerrenal++;
                deadInTerrenal++;
                nextWorld = Worlds.Cielo;
            }
            else if (currentWorld == Worlds.Cielo)
            {
                wasInCielo++;
                deadInCielo++;
                nextWorld = Worlds.Infierno;
            }
            else if (currentWorld == Worlds.Infierno)
            {
                wasInInfierno++;
                deadInInfierno++;
                nextWorld = Worlds.Inframundo;
            }
            else if (currentWorld == Worlds.Inframundo)
            {
                wasInInframundo++;
                deadInInframundo++;
                nextWorld = Worlds.Astral;
            }
            else if (currentWorld == Worlds.Astral) { wasInAstral++; }
        }

        isActive = true;

        ApplyChanges();

        SceneManager.LoadScene(nextWorld.ToString());
    }
    private void ApplyChanges()
    {
        bool canChange = true;
        if (_saveLoad.GetWorldData() != null)
        {
            if (!_saveLoad.GetWorldData().passedTutorial)
            {
                if (currentWorld == Worlds.Cielo)
                {
                    Debug.Log("ESTOY EN EL TUTORIAL");
                    wasInAstral++;
                    currentWorld = Worlds.Terrenal;
                    nextWorld = Worlds.Astral;
                    _saveLoad.SaveData(ReasonSave.closeGame);
                    canChange = false;
                }
            }
        }

        if (canChange)
        {
            Debug.Log("Cambios diferentes");
            currentWorld = nextWorld;
            _saveLoad.SaveData(ReasonSave.deadSystem);
        }
    }
}