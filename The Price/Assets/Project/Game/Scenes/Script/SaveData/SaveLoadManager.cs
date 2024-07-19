using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour {

    [Header("Directions")]
    private string filePath;

    [Header("Save & Load")]
    private SavePlayer _player;
    private SaveWorld _world;
    private SaveControls _controls;

    [Header("Private Calls")]
    private PlayerStats _playerStats;
    private DeadSystem _deadSystem;
    private WeaponManagerUI _weaponManager;
    private HUD _hud;

    private void Awake()
    {
        _hud = FindAnyObjectByType<HUD>();
        _weaponManager = FindAnyObjectByType<WeaponManagerUI>();
        _playerStats = FindAnyObjectByType<PlayerStats>();
        _deadSystem = _playerStats.GetComponent<DeadSystem>();
    }
    // ---- MAINS ---- //
    public void SaveData(ReasonSave reason)
    {
        if (_controls == null) _controls = new SaveControls();
        if (_player == null) _player = new SavePlayer();
        if (_world == null) _world = new SaveWorld();

        // ---- RELLENAR VALORES A GUARDAR ---- //
        RefillDataWorld(reason);
        RefillDataControls();
        RefillDataPlayer();

        filePath = Path.Combine(Application.persistentDataPath, "Player-" + PlayerPrefs.GetInt("PositionGame").ToString() + ".json");
        string jsonPlayer = JsonUtility.ToJson(_player);
        File.WriteAllText(filePath, jsonPlayer);
        // Debug.Log(filePath);


        filePath = Path.Combine(Application.persistentDataPath, "World-" + PlayerPrefs.GetInt("PositionGame").ToString() + ".json");
        string jsonWorld = JsonUtility.ToJson(_world);
        File.WriteAllText(filePath, jsonWorld);

        filePath = Path.Combine(Application.persistentDataPath, "Controls-" + PlayerPrefs.GetInt("PositionGame").ToString() + ".json");
        string jsonControls = JsonUtility.ToJson(_controls);
        File.WriteAllText(filePath, jsonControls);
    }
    public void LoadData()
    {
        filePath = Path.Combine(Application.persistentDataPath, "Player-" + PlayerPrefs.GetInt("PositionGame").ToString() + ".json");
        if (!File.Exists(filePath)) { return; }

        //// FUNCIONAMIENTO PARA PLAYER ----------------- ////
        if (File.Exists(filePath))
        {
            string jsonPlayer = File.ReadAllText(filePath);
            _player = JsonUtility.FromJson<SavePlayer>(jsonPlayer);

            #region Stats
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
            #endregion

            _hud.SetGold(_player.gold);
            _hud.SetSouls(_player.souls);

            _playerStats.weapons = LoadWeapons(_player.weaponInHand);

            _playerStats.objects = LoadObjects(_player.objects);
            _playerStats.skills = LoadSkills(_player.skills);
        }

        //// FUNCIONAMIENTO PARA WORLDS ----------------- ////
        filePath = Path.Combine(Application.persistentDataPath, "World-" + PlayerPrefs.GetInt("PositionGame").ToString() + ".json");

        if (File.Exists(filePath))
        {
            string jsonWorld = File.ReadAllText(filePath);
            _world = JsonUtility.FromJson<SaveWorld>(jsonWorld);

            if (_world.reasonSave == ReasonSave.closeGame) _deadSystem.currentWorld = Worlds.Astral;
            else _deadSystem.currentWorld = (Worlds)_world.currentWorld;

            if (_world.passedTutorial) PlayerPrefs.SetString("Position-" + (_world.positionGame), "Si");
            else PlayerPrefs.SetString("Position-" + (_world.positionGame), "");

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

        //// FUNCIONAMIENTO PARA CONTROLS --------------- ////
        filePath = Path.Combine(Application.persistentDataPath, "Controls-" + PlayerPrefs.GetInt("PositionGame").ToString() + ".json");

        if (File.Exists(filePath))
        {
            string jsonControls = File.ReadAllText(filePath);
            _controls = JsonUtility.FromJson<SaveControls>(jsonControls);
        }
    }
    // ---- GUARDADOS ---- //
    private void RefillDataWorld(ReasonSave reason)
    {
        _world.positionGame = PlayerPrefs.GetInt("PositionGame", 0);
        _world.passedTutorial = (_deadSystem.wasInAstral > 0) ? true : false;

        if (_world.passedTutorial) LunarCycle.isActive = true;

        if (_world.passedTutorial) PlayerPrefs.SetString("Position-" + (_world.positionGame), "Si");
        else PlayerPrefs.SetString("Position-" + (_world.positionGame), "");

        _world.reasonSave = reason;
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

        _player.canHaveSouls = _deadSystem.canHaveSouls;
        _player.souls = _hud.GetSouls();
        _player.gold = _hud.GetGold();

        if(_playerStats.weapons.Count > 0)
        {
            List<int> weapons = new List<int>();
            for(int i = 0; i< _playerStats.weapons.Count; i++)
            {
                if (_playerStats.weapons[i] != null) weapons.Add(_playerStats.weapons[i].weaponID);
                else weapons.Add(-1);
        }

            _player.weaponInHand = weapons;
        }

        List<int> objects = new List<int>();
        for(int i = 0; i < _playerStats.objects.Count; i++) { objects.Add(_playerStats.objects[i].objectID); }
        _player.objects = objects;

        List<int> skills = new List<int>();
        for (int i = 0; i < _playerStats.skills.Count; i++) { objects.Add(_playerStats.skills[i].skillID); }
        _player.skills = skills;
    }
    private void RefillDataControls()
    {
        _controls.moveUp = 0;
        _controls.moveDown = 1;
        _controls.moveLeft = 2;
        _controls.moveRight = 3;

        _controls.use = 4;
        _controls.dash = 5;

        _controls.staticAim = 6;
        _controls.aimUp = 7;
        _controls.aimDown = 8;
        _controls.aimLeft = 9;
        _controls.aimRight = 10;

        _controls.stats = 11;
        _controls.pause = 12;

        _controls.attackOne = 13;
        _controls.attackTwo = 14;
        _controls.attackThree = 15;

        _controls.skillOne = 16;
        _controls.skillTwo = 17;
        _controls.skillThree = 18;

        // UI-CONTENT
        _controls.back = 15;
        _controls.select = 5;
        _controls.resetValues = 14;
        _controls.otherFunction = 13;

        _controls.leftUI = 6;
        _controls.rightUI = 4;
    }
    // ---- INTEGRAS ---- //
    private List<SkillManager> LoadSkills(List<int> skillsID)
    {
        SkillPlacement placement = FindAnyObjectByType<SkillPlacement>();
        List<SkillManager> skills = new List<SkillManager>();

        for(int i = 0; i < skillsID.Count; i++)
        {
            skills.Add(placement.GetSkillPerID(skillsID[i]));
        }

        return skills;
    }
    private List<Object> LoadObjects(List<int> objectID)
    {
        ObjectPlacement placement = FindAnyObjectByType<ObjectPlacement>();
        List<Object> objects = new List<Object>();

        for (int i = 0; i < objectID.Count; i++)
        {
            objects.Add(placement.GetObjectPerID(objectID[i]));
        }

        return objects;
    }
    private List<WeaponSystem> LoadWeapons(List<int> weaponID)
    {
        List<WeaponSystem> weaponSystem = new List<WeaponSystem>();

        for(int i = 0; i < _weaponManager.weapons.Count; i++)
        {
            for(int j = 0; j < weaponID.Count; j++)
            {
                if (_weaponManager.weapons[i].weaponID == weaponID[j])
                {
                    weaponSystem.Add(_weaponManager.weapons[i]);
                    break;
                }
                if (weaponID[j] == -1)
                {
                    weaponSystem.Add(null);
                    break;
                }
            }
        }

        return weaponSystem;
    }
    // ---- GETTERS ---- //
    public SaveWorld GetWorldData()
    {
        filePath = Path.Combine(Application.persistentDataPath, "World-" + PlayerPrefs.GetInt("PositionGame").ToString() + ".json");

        if (File.Exists(filePath)) { return _world; }
        else { return null; }
    }
    public SaveControls GetControls(){ return _controls; }
}
