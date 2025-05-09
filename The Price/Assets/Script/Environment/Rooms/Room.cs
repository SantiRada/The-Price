using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Room : MonoBehaviour {

    [Header("General Data")]
    [Tooltip("Que sea CHILL significa que no habrá enemigos y se puede avanzar sin hacer nada")] public bool isChill = false;
    [Tooltip("Si es sala de Boss no crea enemigos")] public bool isBossRoom = false;

    [Header("Enemies")]
    public List<EnemyManager> _livingEnemies = new List<EnemyManager>();
    [SerializeField] private GameObject[] _spawnEnemy;
    private int _currentWeight = 0;

    [Header("Player Data")]
    public GameObject spawnPlayer;
    [SerializeField] private Vector2 minDistanceCam;
    [SerializeField] private Vector2 maxDistanceCam;

    [Header("Map Data")]
    public Vector2 posToShop;
    public Vector2 posToReward;
    public Vector2Int sizeMap;
    public GameObject spawnMap;

    [Header("Private Data")]
    private RoomManager _roomManager;
    private bool _canAdvance = false;

    [Header("Walkable Data")]
    public Vector2Int mapSize;
    public TypeNode[] flatWalkableMap;

    [HideInInspector] public List<EnemyManager> _activeEnemies = new List<EnemyManager>();
    [SerializeField] private int _maxEnemiesPerWave = 3;
    private int _currentEnemyIndex = 0;

    private void Start()
    {
        _roomManager = GetComponentInParent<RoomManager>();

        InitialValues();
    }
    private void InitialValues()
    {
        _currentWeight = 0;
        CameraMovement.SetMinMax(minDistanceCam, maxDistanceCam);

        if (!isChill)
        {
            _roomManager.walkableMap.completeMap += CreateEnemies;
        }
        else { Advance(); }
    }
    public TypeNode[,] GetWalkableMap2D()
    {
        var result = new TypeNode[mapSize.x, mapSize.y];
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                result[x, y] = flatWalkableMap[y * mapSize.x + x];
            }
        }
        return result;
    }
    private void OnDestroy() { _roomManager.walkableMap.completeMap -= CreateEnemies; }
    private void CreateEnemies()
    {
        if (isBossRoom) return;

        int maxWeight = _roomManager.WeightEnemiesForThisPlace;
        int attempts = 0;
        int maxAttempts = 100;

        _livingEnemies.Clear(); // Limpiamos por si acaso

        while (_currentWeight < maxWeight && attempts < maxAttempts)
        {
            attempts++;

            List<EnemyManager> validEnemies = new List<EnemyManager>();
            foreach (var enemy in _roomManager.EnemyPool)
            {
                if (enemy.Weight + _currentWeight <= maxWeight)
                {
                    if (_roomManager._countRoomsComplete <= 1 && enemy.GetComponent<EnemyForAttack>()) continue;
                    validEnemies.Add(enemy);
                }
            }

            if (validEnemies.Count == 0) break;

            int totalProbability = 0;
            foreach (var enemy in validEnemies)
            {
                totalProbability += enemy.probabilityOfAppearing;
            }

            int rand = Random.Range(0, totalProbability);
            int cumulative = 0;
            EnemyManager selectedEnemy = null;

            foreach (var enemy in validEnemies)
            {
                cumulative += enemy.probabilityOfAppearing;
                if (rand < cumulative)
                {
                    selectedEnemy = enemy;
                    break;
                }
            }

            if (selectedEnemy != null)
            {
                _livingEnemies.Add(selectedEnemy);
                _currentWeight += selectedEnemy.Weight;
            }
        }

        // Solo instanciar la primera horda
        SpawnNextWave();
    }
    private void SpawnNextWave()
    {
        int count = 0;

        while (_currentEnemyIndex < _livingEnemies.Count && count < _maxEnemiesPerWave)
        {
            var enemyPrefab = _livingEnemies[_currentEnemyIndex];
            Vector3 spawnPos = _spawnEnemy[Random.Range(0, _spawnEnemy.Length)].transform.position;
            EnemyManager instance = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            instance.CurrentRoom = this;

            _activeEnemies.Add(instance);
            _currentEnemyIndex++;
            count++;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && _canAdvance)
        {
            if (_roomManager.madeInteraction) _roomManager.StartCoroutine("ChangeState");
            else _roomManager.CreateInteraction();
        }
    }
    // ---- SETTERS && GETTERS ---- //
    public void SetLivingEnemies(EnemyManager enemy)
    {
        if (isBossRoom) return;

        _roomManager.AddKillsToPlayer();

        if (_activeEnemies.Remove(enemy))
        {
            if (_activeEnemies.Count <= 1)
            {
                if (_currentEnemyIndex < _livingEnemies.Count)
                {
                    SpawnNextWave(); // Lanzar siguiente horda
                }
                else if (_activeEnemies.Count == 0)
                {
                    Advance(); // Todos muertos
                }
            }
        }
    }
    public void Advance()
    {
        _canAdvance = true;
        StartCoroutine(_roomManager.Advance());
    }
    public void SetUselessEnemies(TypeEnemyAttack typeUseless, bool value = false)
    {
        for (int i = 0; i < _livingEnemies.Count; i++)
        {
            if (_livingEnemies[i].typeAttack == typeUseless)
            {
                if (_livingEnemies[i].CanAttack != value)
                {
                    _livingEnemies[i].AddState(TypeState.Stun, 2);
                }
            }
        }
    }
    public void SetShieldToNull() { for (int i = 0; i < _livingEnemies.Count; i++) { _livingEnemies[i].shield = 0; } }
}