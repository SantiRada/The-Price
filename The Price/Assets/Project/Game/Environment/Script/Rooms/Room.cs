using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {

    [Header("Enemies")]
    [SerializeField] private List<EnemyManager> _livingEnemies = new List<EnemyManager>();
    [SerializeField] private GameObject[] _spawnEnemy;
    private int _currentWeight = 0;

    [Header("Player Data")]
    public GameObject spawnPlayer;
    [SerializeField] private Vector2 minDistanceCam;
    [SerializeField] private Vector2 maxDistanceCam;

    [Header("Map Data")]
    public Vector2 posToReward;
    public Vector2Int sizeMap;
    public GameObject spawnMap;

    [Header("Private Data")]
    private RoomManager _roomManager;
    private bool _canAdvance = false;

    private void Start()
    {
        _roomManager = GetComponentInParent<RoomManager>();

        InitialValues();
    }
    private void InitialValues()
    {
        _currentWeight = 0;
        CameraMovement.SetMinMax(minDistanceCam, maxDistanceCam);

        Invoke("CreateEnemies", 0.25f);
    }
    private void CreateEnemies()
    {
        do
        {
            int rnd = Random.Range(0, 100);
            List<EnemyManager> possibleEnemies = new List<EnemyManager>();

            for (int i = 0; i < _roomManager.EnemyPool.Count; i++)
            {
                if (rnd <= _roomManager.EnemyPool[i].ProbabilityOfAppearing)
                    possibleEnemies.Add(_roomManager.EnemyPool[i]);
            }

            int selector = Random.Range(0, possibleEnemies.Count);

            EnemyManager enemy = Instantiate(possibleEnemies[selector], _spawnEnemy[Random.Range(0, _spawnEnemy.Length)].transform.position, Quaternion.identity);
            enemy.RoomCurrent = this;

            _livingEnemies.Add(enemy);

            _currentWeight += possibleEnemies[selector].Weight;

        } while (_currentWeight < _roomManager.WeightEnemiesForThisPlace);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && _canAdvance)
        {
            _roomManager.StartCoroutine("ChangeState");
        }
    }
    // ---- SETTERS && GETTERS ---- //
    public void SetLivingEnemies(EnemyManager enemy)
    {
        Vector3 posEnemy = enemy.transform.position;
        if (_livingEnemies.Remove(enemy))
        {
            if (_livingEnemies.Count <= 0)
            {
                _canAdvance = true;
                StartCoroutine(_roomManager.Advance(posEnemy));
            }
        }
    }
    public void SetUselessEnemies(TypeEnemyAttack typeUseless, bool value = false)
    {
        for(int i = 0; i < _livingEnemies.Count; i++)
        {
            if (_livingEnemies[i].typeAttack == typeUseless)
            {
                if (_livingEnemies[i].CanAttack != value)
                {
                    Debug.Log("Se inutilizaron los ataques de tipo: " + typeUseless.ToString());
                    _livingEnemies[i].CanAttack = value;
                }
            }
        }
    }
    public void SetShieldToNull()
    {
        for(int i = 0; i < _livingEnemies.Count; i++)
        {
            _livingEnemies[i].Shield = 0;
        }
    }
}