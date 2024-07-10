using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum TypeRoom { Null, Basic, Gold, Skill, Aptitud, Object, Lore, Shop, MiniBoss, Boss, Astral, MaxBoss }
public class RoomManager : MonoBehaviour {

    [Header("Room Content")]
    public Room[] _roomPool;
    public Room _roomAstral;

    [Header("Boss Content")]
    public Room _minBossRoom;
    public BossSystem _minBoss;

    [Header("Extra Room Content")]
    public TextMeshProUGUI textNextRoom;
    [SerializeField] private TypeRoom[] _typeRooms;
    private int _countRoomsComplete = -1;
    private bool isPerfectRoom = false;
    public static event Action finishRoom;
    public static event Action perfectRoom;
    public static event Action advanceRoom;

    [Header("UI Content")]
    public Animator loadingSector;
    [SerializeField] private float _timeToLoad;
    [Space]
    [SerializeField] private GameObject _advanceDataRoom;
    [SerializeField] private GameObject _textForPerfectRoom;
    [SerializeField] private float _delayToChange;
    [SerializeField] private float _timeToGenerateMap;

    [Header("Enemy Data")]
    [SerializeField, Tooltip("Peso de los enemigos para este mundo")] private int _weightEnemies;
    [SerializeField] private List<EnemyManager> _enemyPool = new List<EnemyManager>();
    private List<int> _enemyWeights = new List<int>();

    [Header("Reward Data")]
    [SerializeField] private GameObject _weaponPlacement;
    [Range(0, 100), Tooltip("Porcentaje de probabilidad de que aparezca un arma en cada sala")] public float weaponChance;
    [SerializeField] private GameObject _rewardSkill;
    [SerializeField] private GameObject _rewardObject;
    [SerializeField] private GameObject _rewardFlair;
    [SerializeField] private GameObject _meditation;
    [SerializeField] private GameObject _shop;
    private GameObject _rewardInScene;

    [Header("Private Content")]
    private WalkableMapGenerator _walkableMap;
    private LunarCycle _lunarCycle;
    private Room currentRoom;

    [Header("Player Content")]
    private PlayerMovement _player;
    private TriggeringObject _triggering;

    private void Awake()
    {
        _lunarCycle = FindAnyObjectByType<LunarCycle>();
        _player = FindAnyObjectByType<PlayerMovement>();
        _triggering = _player.GetComponent<TriggeringObject>();
        _walkableMap = FindAnyObjectByType<WalkableMapGenerator>();
    }
    private void Start()
    {
        InitialValues();
    }
    private void InitialValues()
    {
        SetTypeRoomForThisPlace();

        loadingSector.SetBool("inLoading", false);
        _textForPerfectRoom.SetActive(false);
        _advanceDataRoom.SetActive(false);
        PlayerStats.takeDamage += () => isPerfectRoom = false;

        // Create Weight For the Enemies
        for (int i = 0; i < _enemyPool.Count; i++)
        {
            _enemyWeights.Add(_enemyPool[i].Weight);
        }

        CreateRoom();
    }
    // ---- CREATE ROOMS --------- //
    public IEnumerator ChangeState()
    {
        yield return new WaitForSeconds(_delayToChange);
        loadingSector.SetBool("inLoading", true);
        Pause.SetPause(true);

        yield return new WaitForSeconds(_timeToLoad * 0.3f);
        _advanceDataRoom.SetActive(false);
        if (currentRoom != null) Destroy(currentRoom.gameObject);
        CreateRoom();

        _lunarCycle.StartCoroutine("AddRoom");

        yield return new WaitForSeconds(_timeToLoad * 0.6f + 1.5f);
        loadingSector.SetBool("inLoading", false);
        Pause.SetPause(false);
    }
    private void ResetValuesToNewRoom()
    {
        advanceRoom?.Invoke();

        // VERIFICA OBJETOS EN ESCENA PARA ELIMINARLOS ANTES DE LA CREACIÓN DE UNA NUEVA SALA
        SkillManager[] skills = FindObjectsByType<SkillManager>(FindObjectsSortMode.None);
        if (skills.Length > 0) { for (int i = 0; i < skills.Length; i++) { Destroy(skills[i].gameObject); } }

        // ELIMINAR TODOS LOS OBJETOS SPREAD
        ObjectSpread[] spread = FindObjectsByType<ObjectSpread>(FindObjectsSortMode.None);
        if (spread.Length > 0) { for (int i = 0; i < spread.Length; i++) { Destroy(spread[i].gameObject); } }

        // ELIMINA EL REWARD
        if(_rewardInScene != null) Destroy(_rewardInScene.gameObject);

        // ELIMINA EL OBJETO INTERACTIVE
        Interactive[] interactiveObj = FindObjectsByType<Interactive>(FindObjectsSortMode.None);
        if(interactiveObj.Length > 0) { for(int i = 0; i < interactiveObj.Length; i++) { Destroy(interactiveObj[i].gameObject); } }

        // ELIMINA TODOS LOS PROYECTILES
        Projectile[] pr = FindObjectsByType<Projectile>(FindObjectsSortMode.None);
        if(pr.Length > 0) for(int i = 0; i< pr.Length; i++) { Destroy(pr[i].gameObject); }

        // ELIMINA TODOS LOS OBJETOS DE DAÑO
        ObjectPerDamage[] objDamage = FindObjectsByType<ObjectPerDamage>(FindObjectsSortMode.None);
        if (objDamage.Length > 0) for (int i = 0; i < objDamage.Length; i++) { Destroy(objDamage[i].gameObject); }

        isPerfectRoom = true;
    }
    private void CreateRoom()   
    {
        ResetValuesToNewRoom();

        _countRoomsComplete++;
        int rnd = UnityEngine.Random.Range(0, _roomPool.Length);

        // --- CREAR SALA SEGÚN ESPACIO ACTUAL --- //
        CameraMovement.SetSize(SizeCamera.normal);

        if (_typeRooms[_countRoomsComplete] == TypeRoom.Astral) { currentRoom = Instantiate(_roomAstral, Vector3.zero, Quaternion.identity, transform); }
        else if (_typeRooms[_countRoomsComplete] == TypeRoom.MiniBoss)
        {
            // CREAS LA SALA DEL MINI-BOSS Y AL M-BOSS EN ELLA
            currentRoom = Instantiate(_minBossRoom, Vector3.zero, Quaternion.identity, transform);
            Instantiate(_minBoss.gameObject, currentRoom.posToReward, Quaternion.identity);

            CameraMovement.SetSize(SizeCamera.boss);
        }
        else { currentRoom = Instantiate(_roomPool[rnd], Vector3.zero, Quaternion.identity, transform); }
        // --------------------------------------- //

        #region TEXT PER ROOM
        string nextRoom = (_countRoomsComplete + 1) < (_typeRooms.Length - 1) ? _typeRooms[(_countRoomsComplete++)].ToString() : "";
        textNextRoom.text = "Sala Actual: " + _typeRooms[_countRoomsComplete].ToString() + "\n";
        textNextRoom.text += "Siguiente Sala: " + nextRoom;
        #endregion

        _player.transform.position = currentRoom.spawnPlayer.transform.position;

        // Rezising, Reposition & Create
        _walkableMap.SizeMap = currentRoom.sizeMap;
        _walkableMap.transform.position = currentRoom.spawnMap.transform.position;
        _walkableMap.GenerateWalkableMap();
    }
    // ---- CREATE TYPE ROOMS ---- //
    private void SetTypeRoomForThisPlace()
    {
        _typeRooms = new TypeRoom[18];

        // Asignar las salas fijas
        _typeRooms[17] = TypeRoom.MaxBoss;
        _typeRooms[16] = TypeRoom.Astral;
        _typeRooms[15] = TypeRoom.Boss;
        _typeRooms[7] = TypeRoom.MiniBoss;
        _typeRooms[8] = TypeRoom.Shop;
        _typeRooms[2] = TypeRoom.Skill;
        _typeRooms[9] = TypeRoom.Skill;

        // Asignar salas Lore
        SelectedOptionalRooms(TypeRoom.Lore, new int[] { 2, 7, 14 }, 0, 2);

        // Asignar salas Objetos
        SelectedOptionalRooms(TypeRoom.Object, new int[] { 4, 7, 11, 13 }, 2, 4);

        // Asignar salas Aptitudes
        SelectedOptionalRooms(TypeRoom.Aptitud, new int[] { 2, 6, 12, 14 }, 2, 4);

        // Asignar salas aleatorias restantes a Oro y Básica
        for (int i = 0; i < _typeRooms.Length; i++)
        {
            if (_typeRooms[i] == TypeRoom.Null)
            {
                _typeRooms[i] = UnityEngine.Random.value < 0.5 ? TypeRoom.Gold : TypeRoom.Basic;
            }
        }
    }
    private void SelectedOptionalRooms(TypeRoom type, int[] posiciones, int minCount, int maxCount)
    {
        List<int> disponibles = new List<int>();
        foreach (int pos in posiciones)
        {
            if (_typeRooms[pos - 1] == TypeRoom.Null)
            {
                disponibles.Add(pos);
            }
        }
        int count = UnityEngine.Random.Range(minCount, Mathf.Min(maxCount, disponibles.Count) + 1);
        List<int> seleccionadas = new List<int>();
        while (seleccionadas.Count < count)
        {
            int randomIndex = UnityEngine.Random.Range(0, disponibles.Count);
            if (!seleccionadas.Contains(disponibles[randomIndex]))
            {
                seleccionadas.Add(disponibles[randomIndex]);
            }
        }
        foreach (int pos in seleccionadas)
        {
            _typeRooms[pos - 1] = type;
        }
    }
    // ---- VERIFICA EL REWARD PARA OTORGARLO Y AVANZAR ---- //
    public IEnumerator Advance()
    {
        yield return new WaitForSeconds(0.25f);
        if(!currentRoom.isChill) _advanceDataRoom.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        finishRoom?.Invoke();

        // COMPROBAR SI FUE UNA SALA PERFECTA -------------- //
        if (isPerfectRoom)
        {
            perfectRoom?.Invoke();
            _textForPerfectRoom.SetActive(true);
        }

        // INSTANCIA UN ARMA SOLO SI NO ES SALA DE BOSSES NI TIENDA
        if (_typeRooms[_countRoomsComplete] != TypeRoom.Shop && _typeRooms[_countRoomsComplete] != TypeRoom.MiniBoss && _typeRooms[_countRoomsComplete] != TypeRoom.Boss && _typeRooms[_countRoomsComplete] != TypeRoom.MaxBoss)
        {
            int rnd = UnityEngine.Random.Range(0, 100);

            if(rnd < weaponChance) Instantiate(_weaponPlacement.gameObject, _player.transform.position, Quaternion.identity);
        }

        switch (_typeRooms[_countRoomsComplete])
        {
            case TypeRoom.Gold: ManagerGold.CreateGold((_player.transform.position + Vector3.one), CountGold.Big); break;
            case TypeRoom.Skill: _rewardInScene = Instantiate(_rewardSkill, currentRoom.posToReward, Quaternion.identity); break;
            case TypeRoom.Object: _rewardInScene = Instantiate(_rewardObject, currentRoom.posToReward, Quaternion.identity); break;
            case TypeRoom.Aptitud: _rewardInScene = Instantiate(_rewardFlair, currentRoom.posToReward, Quaternion.identity); break;
            case TypeRoom.Shop: _rewardInScene = Instantiate(_shop, currentRoom.posToShop, Quaternion.identity); break;
            case TypeRoom.Lore: _rewardInScene = Instantiate(_meditation, currentRoom.posToShop, Quaternion.identity); break;
            case TypeRoom.MiniBoss: Debug.Log("Has eliminado al Mini-Boss"); break;
        }
    }
    // ---- SETTERS && GETTERS ---- //
    public int WeightEnemiesForThisPlace
    {
        get { return _weightEnemies; }
        set { _weightEnemies = value; }
    }
    public List<EnemyManager> EnemyPool { get { return _enemyPool; } }
    public List<int> EnemyWeights { get { return _enemyWeights; } }
    public void AddKillsToPlayer()
    {
        _triggering.AddKills();
    }
}
