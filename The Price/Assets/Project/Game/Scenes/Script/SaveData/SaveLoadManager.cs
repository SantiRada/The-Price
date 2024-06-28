using System.IO;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour {

    [Header("Directions")]
    private string filePath;

    [Header("Save & Load")]
    private SavePlayer _player;
    private SaveWorld _world;

    [Header("Private Calls")]
    private PlayerStats _playerStats;
    private DeadSystem _deadSystem;

    private void Awake()
    {
        _playerStats = FindAnyObjectByType<PlayerStats>();
        _deadSystem = _playerStats.GetComponent<DeadSystem>();
    }
    private void Start()
    {
        _player = new SavePlayer();
        _world = new SaveWorld();

        RoomManager.finishRoom += SaveData;
    }
    public void SaveData()
    {
        // ---- RELLENAR VALORES A GUARDAR ---- //
        RefillDataPlayer();
        RefillDataWorld();

        filePath = Path.Combine(Application.persistentDataPath, "Player.json");
        string jsonPlayer = JsonUtility.ToJson(_player);
        File.WriteAllText(filePath, jsonPlayer);
        Debug.Log("Data saved to " + filePath);


        filePath = Path.Combine(Application.persistentDataPath, "World.json");
        string jsonWorld = JsonUtility.ToJson(_world);
        File.WriteAllText(filePath, jsonWorld);
        Debug.Log("Data saved to " + filePath);
    }
    private void RefillDataPlayer()
    {
        _player.maxPV = _playerStats.GetterStats(0, true);
        _player.maxConcentracion = _playerStats.GetterStats(1, true);
        _player.speedMovement = _playerStats.GetterStats(2, true);
        _player.speedAttack = _playerStats.GetterStats(3, true);
        _player.skillDamage = _playerStats.GetterStats(4, true);
        _player.damage = _playerStats.GetterStats(5, true);
        _player.subsequenceDamage = _playerStats.GetterStats(6, true);
        _player.criticalChance = _playerStats.GetterStats(7, true);
        _player.missChance = _playerStats.GetterStats(8, true);
        _player.stealing = _playerStats.GetterStats(9, true);
        _player.maxSanity = _playerStats.GetterStats(10, true);

        _player.pv = _playerStats.GetterStats(0, false);
        _player.concentracion = _playerStats.GetterStats(1, false);
        _player.sanity = _playerStats.GetterStats(10, false);

        _player.gold = _playerStats.countGold;

        _player.weaponInHand = _playerStats.weapon;
        _player.objects = _playerStats.objects;
        _player.skills = _playerStats.skills;
    }
    private void RefillDataWorld()
    {
        _world.passedTutorial = (_deadSystem.wasInAstral > 0) ? true : false;
        _world.currentWorld = (int)_deadSystem.currentWorld;

        _world.wasInTerrenal = _deadSystem.wasInTerrenal;
        _world.wasInCielo = _deadSystem.wasInCielo;
        _world.wasInInfierno = _deadSystem.wasInInfierno;
        _world.wasInAstral = _deadSystem.wasInAstral;
        _world.wasInInframundo = _deadSystem.wasInInframundo;

        _world.deadInTerrenal = _deadSystem.deadInTerrenal;
        _world.deadInCielo = _deadSystem.deadInCielo;
        _world.deadInInfierno = _deadSystem.deadInInfierno;
        _world.deadInInframundo = _deadSystem.deadInInframundo;

        // FALTA FUNCIONAMIENTO DE MBOSS, BOSS, MAXBOSS Y THANATOS //
    }
    public void LoadData()
    {
        //// FUNCIONAMIENTO PARA PLAYER ----------------- ////
        filePath = Path.Combine(Application.persistentDataPath, "Player.json");
        if (File.Exists(filePath))
        {
            string jsonPlayer = File.ReadAllText(filePath);
            _player = JsonUtility.FromJson<SavePlayer>(jsonPlayer);
            Debug.Log("Data loaded from " + filePath);

            _playerStats.SetValue(0, _player.maxPV, true, false, true);
            _playerStats.SetValue(1, _player.maxConcentracion, true, false, true);
            _playerStats.SetValue(2, _player.speedMovement, true, false, true);
            _playerStats.SetValue(3, _player.speedAttack, true, false, true);
            _playerStats.SetValue(4, _player.skillDamage, true, false, true);
            _playerStats.SetValue(5, _player.damage, true, false, true);
            _playerStats.SetValue(6, _player.subsequenceDamage, true, false, true);
            _playerStats.SetValue(7, _player.criticalChance, true, false, true);
            _playerStats.SetValue(8, _player.missChance, true, false, true);
            _playerStats.SetValue(9, _player.stealing, true, false, true);
            _playerStats.SetValue(10, _player.maxSanity, true, false, true);

            if (_player.pv <= 0) _playerStats.SetValue(0, _player.maxPV, false, false, true);
            else _playerStats.SetValue(0, _player.pv, false, false, true);

            _playerStats.SetValue(1, _player.concentracion, false, false, true);
            _playerStats.SetValue(10, _player.sanity, false, false, true);

            _playerStats.countGold = _player.gold;

            _playerStats.weapon = _player.weaponInHand;
            _playerStats.skills = _player.skills;
            _playerStats.objects = _player.objects;
        }
        else { Debug.LogWarning("No data file found"); }


        //// FUNCIONAMIENTO PARA WORLDS ----------------- ////
        filePath = Path.Combine(Application.persistentDataPath, "World.json");

        if (File.Exists(filePath))
        {
            string jsonWorld = File.ReadAllText(filePath);
            _player = JsonUtility.FromJson<SavePlayer>(jsonWorld);
            Debug.Log("Data loaded from " + filePath);

            _deadSystem.currentWorld = (Worlds)_world.currentWorld;

            _deadSystem.wasInTerrenal = _world.wasInTerrenal;
            _deadSystem.wasInCielo = _world.wasInCielo;
            _deadSystem.wasInInfierno = _world.wasInInfierno;
            _deadSystem.wasInAstral = _world.wasInAstral;
            _deadSystem.wasInInframundo = _world.wasInInframundo;

            _deadSystem.deadInTerrenal = _world.deadInTerrenal;
            _deadSystem.deadInCielo = _world.deadInCielo;
            _deadSystem.deadInInfierno = _world.deadInInfierno;
            _deadSystem.deadInInframundo = _world.deadInInframundo;
        }
        else { Debug.LogWarning("No data file found"); }
    }
    // ---- GETTERS ---- //
    public SaveWorld GetWorldData()
    {
        if (_world != null) return _world;
        else return null;
    }
}
