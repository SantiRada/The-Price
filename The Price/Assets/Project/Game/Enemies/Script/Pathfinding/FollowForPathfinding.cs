using System.Collections.Generic;
using UnityEngine;

public class FollowForPathfinding : MonoBehaviour {

    [SerializeField, Tooltip("Distancia al siguiente punto de camino")] private float _nextWaypointDistance = 3f;

    [Header("Private Data")]
    private int _currentWaypoint = 0;
    private bool[,] _walkableMap;
    [Space]
    private Pathfinding _pathfinding;
    private Transform _target;
    private List<Node> _path;
    private Rigidbody2D _rb2d;
    private EnemyManager _enemyManager;

    private void Start()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _enemyManager = GetComponent<EnemyManager>();

        _pathfinding = FindAnyObjectByType<Pathfinding>();
        _target = FindAnyObjectByType<PlayerMovement>().transform;
        _walkableMap = FindAnyObjectByType<WalkableMapGenerator>().walkableMap;

        UpdatePath();
    }
    private void UpdatePath()
    {
        Vector2Int start = ClampPositionToMap(new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y)));
        Vector2Int end = ClampPositionToMap(new Vector2Int(Mathf.RoundToInt(_target.position.x), Mathf.RoundToInt(_target.position.y)));

        // Validación de coordenadas
        if (start.x < 0 || start.x >= _walkableMap.GetLength(0) || start.y < 0 || start.y >= _walkableMap.GetLength(1) ||
            end.x < 0 || end.x >= _walkableMap.GetLength(0) || end.y < 0 || end.y >= _walkableMap.GetLength(1))
        {
            Debug.LogError("Start or target position is out of bounds");
            return;
        }

        _path = _pathfinding.FindPath(start, end, _walkableMap);
        _currentWaypoint = 0;
    }
    private Vector2Int ClampPositionToMap(Vector2Int position)
    {
        int clampedX = Mathf.Clamp(position.x, 0, _walkableMap.GetLength(0) - 1);
        int clampedY = Mathf.Clamp(position.y, 0, _walkableMap.GetLength(1) - 1);
        return new Vector2Int(clampedX, clampedY);
    }
    private void FixedUpdate()
    {
        if (_path == null || _path.Count == 0)
            return;

        if (_currentWaypoint >= _path.Count)
        {
            _path = null;
            return;
        }

        Vector2 direction = ((Vector2)_path[_currentWaypoint].position - _rb2d.position).normalized;
        Vector2 force = direction * _enemyManager.Speed * Time.deltaTime;

        _rb2d.AddForce(force);

        float distance = Vector2.Distance(_rb2d.position, _path[_currentWaypoint].position);

        if (distance < _nextWaypointDistance)
        {
            _currentWaypoint++;
        }
    }
    private void OnDrawGizmos()
    {
        if (_path != null)
        {
            foreach (Node node in _path)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawCube((Vector3Int)node.position, Vector3.one * 0.5f);
            }
        }
    }
}