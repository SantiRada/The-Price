using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using static ActionForControlPlayer;

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
    [HideInInspector] public bool killedThanatos = false;

    [Header("Current Data")]
    public Worlds currentWorld;
    public Worlds nextWorld;

    private SaveLoadManager _saveLoad;
    private PlayerStats _playerStats;
    private HUD _hud;
    private bool isActive = false;

    private void Awake()
    {
        _hud = FindAnyObjectByType<HUD>();
        _saveLoad = FindAnyObjectByType<SaveLoadManager>();
    }
    private void Start() { LoadInfo(); }
    private void LoadInfo()
    {
        int pos = PlayerPrefs.GetInt("PositionGame");
        _playerStats = GetComponent<PlayerStats>();

        if (_saveLoad == null) return;

        _saveLoad.LoadData();

        if (_saveLoad.GetWorldData() != null)
        {
            if (_saveLoad.GetWorldData().passedTutorial)
            {
                if (_saveLoad.GetWorldData().reasonSave == ReasonSave.deadSystem)
                {
                    if (SceneManager.GetActiveScene().name != currentWorld.ToString())
                    {
                        _saveLoad.SaveData(ReasonSave.Null);
                        SceneManager.LoadScene(currentWorld.ToString());
                        return;
                    }
                }
                else if (_saveLoad.GetWorldData().reasonSave == ReasonSave.closeGame)
                {
                    // MODIFICAR EL MUNDO AL CUAL IR COMO "TERRENAL" PARA LA PRIMERA VEZ QUE TOCAMOS EL PLANO ASTRAL
                    if(wasInAstral == 1) currentWorld = Worlds.Terrenal;

                    _saveLoad.SaveData(ReasonSave.Null);
                    SceneManager.LoadScene("Astral");
                    return;
                }

                if (_saveLoad.GetWorldData() != null)
                {
                    _saveLoad.SaveData(ReasonSave.closeGame);
                    currentWorld = (Worlds)_saveLoad.GetWorldData().currentWorld;
                }
            }
            else
            {
                if (_saveLoad.GetWorldData().reasonSave == ReasonSave.closeGame)
                {
                    File.Delete(Application.persistentDataPath + "/World-" + pos.ToString() + ".json");
                    File.Delete(Application.persistentDataPath + "/Player-" + pos.ToString() + ".json");

                    PlayerPrefs.DeleteKey("Position-" + pos);
                    PlayerPrefs.DeleteKey("PositionGame");

                    SceneManager.LoadScene("Terrenal");
                }
            }
        }
        else
        {
            _saveLoad.SaveData(ReasonSave.Null);
            SceneManager.LoadScene("Cinematic");
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
        _playerStats.SetValue(10, -5, false);

        bool canChange = true;
        if (_saveLoad.GetWorldData() != null)
        {
            if (!_saveLoad.GetWorldData().passedTutorial)
            {
                if (currentWorld == Worlds.Cielo)
                {
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
            currentWorld = nextWorld;
            _saveLoad.SaveData(ReasonSave.deadSystem);
        }
    }
}