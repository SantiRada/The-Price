using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Maneja el guardado y carga de partidas.
/// Actualizado para el nuevo sistema: 7 stats, 1 arma, 2 habilidades.
/// </summary>
// C:\Users\santy\AppData\LocalLow\DefaultCompany\The Price
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
    private RoomManager _roomManager;
    private HUD _hud;

    private void Awake()
    {
        // Buscar componentes con validaciones
        ComponentHelper.TryFindObjectQuiet(out _hud);
        ComponentHelper.TryFindObjectQuiet(out _roomManager);
        ComponentHelper.TryFindObjectQuiet(out _weaponManager);
        ComponentHelper.TryFindObjectQuiet(out _playerStats);

        if (_playerStats != null)
        {
            _playerStats.TryGetComponentSafe(out _deadSystem);
        }
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

            if (_playerStats == null) return;

            #region Stats (7 total)
            // Stats máximas
            _playerStats.SetValue(0, _player.maxPV, true, false, true);
            _playerStats.SetValue(1, _player.maxConcentracion, true, false, true);
            _playerStats.SetValue(2, _player.speedMovement, true, false, true);
            _playerStats.SetValue(3, _player.speedAttack, true, false, true);
            _playerStats.SetValue(4, _player.skillDamage, true, false, true);
            _playerStats.SetValue(5, _player.damage, true, false, true);
            _playerStats.SetValue(6, _player.criticalChance, true, false, true);

            // Stats actuales
            if (_player.pv <= 0)
                _playerStats.SetValue(0, _player.maxPV, false, false, true);
            else
                _playerStats.SetValue(0, _player.pv, false, false, true);

            _playerStats.SetValue(1, _player.concentracion, false, false, true);
            #endregion

            if (_hud == null) return;

            _hud.SetGold(_player.gold);

            // Cargar arma (solo 1) - usar SetWeapon para que se instancie correctamente
            WeaponSystem loadedWeapon = LoadWeapon(_player.weaponID);
            if (loadedWeapon != null)
            {
                _playerStats.SetWeapon(loadedWeapon);
            }

            // Cargar objetos y habilidades
            _playerStats.objects = LoadObjects(_player.objects);
            _playerStats.skills = LoadSkills(_player.skills);

            // Actualizar UI de habilidades
            if (ComponentHelper.TryFindObjectQuiet(out StatsInUI statsUI))
            {
                statsUI.SetChangeSkillsInUI(_playerStats.skills);
            }
        }

        //// FUNCIONAMIENTO PARA WORLDS ----------------- ////
        filePath = Path.Combine(Application.persistentDataPath, "World-" + PlayerPrefs.GetInt("PositionGame").ToString() + ".json");

        if (File.Exists(filePath))
        {
            string jsonWorld = File.ReadAllText(filePath);
            _world = JsonUtility.FromJson<SaveWorld>(jsonWorld);

            if (_deadSystem == null) return;

            if (_world.reasonSave == ReasonSave.closeGame) _deadSystem.currentWorld = Worlds.Astral;
            else _deadSystem.currentWorld = (Worlds)_world.currentWorld;

            // Tutorial system removed - always set position as passed
            PlayerPrefs.SetString("Position-" + (_world.positionGame), "Si");

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
        if (_deadSystem == null || _world == null) return;

        _world.positionGame = PlayerPrefs.GetInt("PositionGame", 0);

        // Tutorial system removed - always activate lunar cycle and position
        LunarCycle.isActive = true;
        PlayerPrefs.SetString("Position-" + (_world.positionGame), "Si");

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
    }

    private void RefillDataPlayer()
    {
        if (_playerStats == null || _hud == null || _deadSystem == null) return;

        // Stats máximas (7 total)
        _player.maxPV = _playerStats.GetterStats(0, true);
        _player.maxConcentracion = _playerStats.GetterStats(1, true);
        _player.speedMovement = _playerStats.GetterStats(2, true);
        _player.speedAttack = _playerStats.GetterStats(3, true);
        _player.skillDamage = _playerStats.GetterStats(4, true);
        _player.damage = _playerStats.GetterStats(5, true);
        _player.criticalChance = _playerStats.GetterStats(6, true);

        // Stats actuales
        _player.pv = _playerStats.GetterStats(0, false);
        _player.concentracion = _playerStats.GetterStats(1, false);

        _player.gold = _hud.GetGold();

        // Guardar arma (solo 1)
        if (_playerStats.weapon != null)
        {
            _player.weaponID = _playerStats.weapon.weaponID;
        }
        else
        {
            _player.weaponID = -1;
        }

        // Guardar objetos
        List<int> objects = new List<int>();
        for(int i = 0; i < _playerStats.objects.Count; i++)
        {
            if (_playerStats.objects[i] != null)
            {
                objects.Add(_playerStats.objects[i].objectID);
            }
        }
        _player.objects = objects;

        // Guardar habilidades (máximo 2)
        List<int> skills = new List<int>();
        for (int i = 0; i < _playerStats.skills.Count && i < 2; i++)
        {
            if (_playerStats.skills[i] != null)
            {
                skills.Add(_playerStats.skills[i].skillID);
            }
        }
        _player.skills = skills;

        // Actualizar UI de habilidades
        if (ComponentHelper.TryFindObjectQuiet(out StatsInUI statsUI))
        {
            statsUI.SetChangeSkillsInUI(_playerStats.skills);
        }
    }

    private void RefillDataControls()
    {
        if (_controls == null) return;

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

        // Solo 1 arma ahora (usamos attackOne)
        _controls.attackOne = 13;

        // Solo 2 habilidades ahora
        _controls.skillOne = 14;
        _controls.skillTwo = 15;

        // UI-CONTENT
        _controls.back = 16;
        _controls.select = 5;
        _controls.resetValues = 17;
        _controls.otherFunction = 13;

        _controls.leftUI = 6;
        _controls.rightUI = 4;
    }

    // ---- INTEGRAS ---- //
    private List<SkillManager> LoadSkills(List<int> skillsID)
    {
        if (!ComponentHelper.TryFindObjectQuiet(out SkillPlacement placement))
        {
            Debug.LogWarning("[SaveLoadManager] SkillPlacement no encontrado");
            return new List<SkillManager>();
        }

        List<SkillManager> skills = new List<SkillManager>();

        // Máximo 2 habilidades
        for(int i = 0; i < skillsID.Count && i < 2; i++)
        {
            SkillManager skill = placement.GetSkillPerID(skillsID[i]);
            if (skill != null)
            {
                skills.Add(skill);
            }
        }

        return skills;
    }

    private List<Object> LoadObjects(List<int> objectID)
    {
        if (!ComponentHelper.TryFindObjectQuiet(out ObjectPlacement placement))
        {
            Debug.LogWarning("[SaveLoadManager] ObjectPlacement no encontrado");
            return new List<Object>();
        }

        List<Object> objects = new List<Object>();

        for (int i = 0; i < objectID.Count; i++)
        {
            Object obj = placement.GetObjectPerID(objectID[i]);
            if (obj != null)
            {
                objects.Add(obj);
            }
        }

        return objects;
    }

    private WeaponSystem LoadWeapon(int weaponID)
    {
        if (weaponID == -1) return null;

        if (_weaponManager == null || _weaponManager.weapons == null)
        {
            Debug.LogWarning("[SaveLoadManager] WeaponManager no encontrado o sin armas");
            return null;
        }

        for(int i = 0; i < _weaponManager.weapons.Count; i++)
        {
            if (_weaponManager.weapons[i] != null && _weaponManager.weapons[i].weaponID == weaponID)
            {
                return _weaponManager.weapons[i];
            }
        }

        Debug.LogWarning($"[SaveLoadManager] Arma con ID {weaponID} no encontrada");
        return null;
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
