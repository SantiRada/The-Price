using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour {

    [Header("Room Content")]
    [SerializeField] private Room[] _roomPool;
    [SerializeField] private float _delayToChange;
    [SerializeField] private float _timeToGenerateMap;

    [Header("UI Content")]
    [SerializeField] private Animator _loadingSector;
    [SerializeField] private float _timeToLoad;
    [Space]
    [SerializeField] private GameObject _advanceDataRoom;

    [Header("Enemy Data")]
    [SerializeField, Tooltip("Peso de los enemigos para este mundo")] private int _weightEnemiesForThisPlace;
    [SerializeField] private List<EnemyManager> _enemyPool = new List<EnemyManager>();
    private List<int> _enemyWeights = new List<int>();

    [Header("Private Content")]
    private PlayerMovement _player;
    [HideInInspector] public Room currentRoom;
    private WalkableMapGenerator _walkableMap;

    private void Awake()
    {
        _player = FindAnyObjectByType<PlayerMovement>();
        _walkableMap = FindAnyObjectByType<WalkableMapGenerator>();
    }
    private void Start()
    {
        InitialValues();
    }
    private void InitialValues()
    {
        _loadingSector.SetBool("inLoading", false);
        _advanceDataRoom.SetActive(false);

        // Create Weight For the Enemies
        for (int i = 0; i < _enemyPool.Count; i++)
        {
            _enemyWeights.Add(_enemyPool[i].Weight);
        }

        CreateRoom();
    }
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
    private void CreateRoom()
    {
        int rnd = Random.Range(0, _roomPool.Length);

        currentRoom = Instantiate(_roomPool[rnd], Vector3.zero, Quaternion.identity, transform);

        _player.transform.position = currentRoom.spawnPlayer.transform.position;

        _walkableMap.Invoke("GenerateWalkableMap", _timeToGenerateMap);
    }
    // ---------------------------- //
    public void Advance() { _advanceDataRoom.SetActive(true); }
    // ---- SETTERS && GETTERS ---- //
    public int WeightEnemiesForThisPlace
    {
        get { return _weightEnemiesForThisPlace; }
        set { _weightEnemiesForThisPlace = value; }
    }
    public List<EnemyManager> EnemyPool { get { return _enemyPool; } }
    public List<int> EnemyWeights { get { return _enemyWeights; } }
}
