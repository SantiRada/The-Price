using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorSystem : MonoBehaviour {

    [Header("Data")]
    [Tooltip("A dónde me lleva esta puerta")] public Worlds whereItTakesMe;
    [Range(0f, 2f)] public float timerToLoadScene;
    public bool isActive = true;

    private SaveLoadManager _saveLoad;
    private RoomManager _roomManager;
    private LunarCycle _lunarCycle;
    private DeadSystem _deadSystem;
    PlayerStats _playerStats;

    private void OnEnable()
    {
        _saveLoad = FindAnyObjectByType<SaveLoadManager>();
        _roomManager = FindAnyObjectByType<RoomManager>();
        _playerStats = FindAnyObjectByType<PlayerStats>();
        _lunarCycle = FindAnyObjectByType<LunarCycle>();
        _deadSystem = FindAnyObjectByType<DeadSystem>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (isActive)
            {
                StartCoroutine("ChangeScene");
            }
            else { VoiceSystem.StartDialogue(40); }
        }
    }
    private IEnumerator ChangeScene()
    {
        _playerStats.SetValue(10, -5, false);
        _lunarCycle.canvas.alpha = 0;

        _roomManager.loadingSector.SetBool("inLoading", true);

        switch (_deadSystem.currentWorld)
        {
            case Worlds.Terrenal: _deadSystem.wasInTerrenal++; break;
            case Worlds.Cielo: _deadSystem.wasInCielo++; break;
            case Worlds.Infierno: _deadSystem.wasInInfierno++; break;
            case Worlds.Astral: _deadSystem.wasInAstral++; break;
            case Worlds.Inframundo: _deadSystem.wasInInframundo++; break;
        }

        _deadSystem.currentWorld = whereItTakesMe;

        _saveLoad.SaveData(ReasonSave.deadSystem);

        yield return new WaitForSeconds(timerToLoadScene);

        SceneManager.LoadScene(whereItTakesMe.ToString());
    }
}
