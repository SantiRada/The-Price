using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeRoom { Null, Basic, Gold, Skill, Aptitud, Object, Lore, Shop, MiniBoss, Boss, Astral, MaxBoss }
public class RoomManager : MonoBehaviour {

    [Header("Room Content")]
    [SerializeField] private Room[] _roomPool;
    [SerializeField] private TypeRoom[] _typeRooms;
    private int _countRoomsComplete = -1;
    private bool isPerfectRoom = false;
    public static event Action finishRoom;
    public static event Action perfectRoom;

    [Header("UI Content")]
    [SerializeField] private Animator _loadingSector;
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
    [SerializeField] private GameObject _rewardSkill;
    [SerializeField] private GameObject _rewardObject;
    [SerializeField] private GameObject _rewardFlair;
    [SerializeField] private GameObject _shop;

    [Header("Private Content")]
    private WalkableMapGenerator _walkableMap;
    private Room currentRoom;

    [Header("Player Content")]
    private PlayerMovement _player;
    private TriggeringObject _triggering;

    private void Awake()
    {
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

        _loadingSector.SetBool("inLoading", false);
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
        _loadingSector.SetBool("inLoading", true);
        Pause.SetPause(true);

        yield return new WaitForSeconds(_timeToLoad * 0.3f);
        _advanceDataRoom.SetActive(false);
        if (currentRoom != null) Destroy(currentRoom.gameObject);
        CreateRoom();

        yield return new WaitForSeconds(_timeToLoad * 0.6f);
        _loadingSector.SetBool("inLoading", false);
        Pause.SetPause(false);
    }
    private void ResetValuesToNewRoom()
    {
        // VERIFICA OBJETOS EN ESCENA PARA ELIMINARLOS ANTES DE LA CREACIÓN DE UNA NUEVA SALA
        SkillManager[] skills = FindObjectsByType<SkillManager>(FindObjectsSortMode.None);
        if (skills.Length > 0) { for (int i = 0; i < skills.Length; i++) { Destroy(skills[i].gameObject); } }

        ObjectSpread[] spread = FindObjectsByType<ObjectSpread>(FindObjectsSortMode.None);
        if (spread.Length > 0) { for (int i = 0; i < spread.Length; i++) { Destroy(spread[i].gameObject); } }

        isPerfectRoom = true;
    }
    private void CreateRoom()
    {
        ResetValuesToNewRoom();

        _countRoomsComplete++;
        int rnd = UnityEngine.Random.Range(0, _roomPool.Length);

        currentRoom = Instantiate(_roomPool[rnd], Vector3.zero, Quaternion.identity, transform);

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
    public IEnumerator Advance(Vector3 pos)
    {
        yield return new WaitForSeconds(0.25f);
        _advanceDataRoom.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        finishRoom?.Invoke();

        // COMPROBAR SI FUE UNA SALA PERFECTA -------------- //
        if(isPerfectRoom)
        {
            perfectRoom?.Invoke();
            _textForPerfectRoom.SetActive(true);
        }

        switch (_typeRooms[_countRoomsComplete])
        {
            case TypeRoom.Gold: ManagerGold.CreateGold(pos, CountGold.Big); break;
            case TypeRoom.Skill: Instantiate(_rewardSkill, currentRoom.posToReward, Quaternion.identity); break;
            case TypeRoom.Object: Instantiate(_rewardObject, currentRoom.posToReward, Quaternion.identity); break;
            case TypeRoom.Aptitud: Instantiate(_rewardFlair, currentRoom.posToReward, Quaternion.identity); break;
            case TypeRoom.Shop: Instantiate(_shop, currentRoom.posToShop, Quaternion.identity); break;
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
