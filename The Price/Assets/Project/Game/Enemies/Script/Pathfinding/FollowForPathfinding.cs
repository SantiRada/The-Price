using System.Collections.Generic;
using UnityEngine;

public class FollowForPathfinding : MonoBehaviour {

    [SerializeField, Tooltip("Distancia al siguiente punto de camino")] private float _nextWaypointDistance = 1f;

    [Header("Private Data")]
    private int _currentWaypoint = 0;
    private TypeNode[,] _walkableMap;
    private Transform _target;
    private Rigidbody2D _rb2d;
    private EnemyManager _enemyManager;
    private List<Node> _path;
    private Pathfinding _pathfinding;

    private void Awake()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _enemyManager = GetComponent<EnemyManager>();
        _pathfinding = FindAnyObjectByType<Pathfinding>();
        _target = FindAnyObjectByType<PlayerMovement>().transform;
    }
    private void Start()
    {
        UpdatePath();
    }
    private void UpdatePath()
    {
        var mapGenerator = FindAnyObjectByType<WalkableMapGenerator>();
        _walkableMap = mapGenerator.walkableMap;

        Debug.Log("Start: (" + transform.position.x + ", " + transform.position.y + ")");
        Vector2Int start = ClearIndexToMap(new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y)));
        Debug.Log("End: (" + _target.position.x + ", " + _target.position.y + ")");
        Vector2Int end = ClearIndexToMap(new Vector2Int(Mathf.RoundToInt(_target.position.x), Mathf.RoundToInt(_target.position.y)));

        _path = _pathfinding.FindPath(start, end, _walkableMap);
        _currentWaypoint = 0;
    }
    private Vector2Int ClearIndexToMap(Vector2Int position)
    {
        Debug.Log("Recibo: (" + position.x + ", " + position.y + ")");
        int newX = Mathf.Abs((int)_pathfinding.transform.position.x) + position.x;
        int newY = Mathf.Abs((int)_pathfinding.transform.position.y) + position.y;

        position = new Vector2Int(newX, newY);
        Debug.Log("Envio: (" + position.x + ", " + position.y + ")");
        return position;
    }
    private Vector2Int RepositionToIndexMap(Vector2Int position)
    {
        position = new Vector2Int((int)_pathfinding.transform.position.x, (int)_pathfinding.transform.position.y) - ClearIndexToMap(position);

        return position;
    }
    private void FixedUpdate()
    {
        if (LoadingScreen.inLoading || Pause._inPause) return;

        if (_path == null || _path.Count == 0) return;

        if (_currentWaypoint >= _path.Count)
        {
            _path = null;
            return;
        }

        Vector2 direction = ((Vector2)_path[_currentWaypoint].position - _rb2d.position).normalized;
        Vector2 force = direction * _enemyManager.Speed * Time.deltaTime;

        _rb2d.AddForce(force);

        float distance = Vector2.Distance(_rb2d.position, _path[_currentWaypoint].position);

        if (distance < _nextWaypointDistance) _currentWaypoint++;
    }
    private void OnDrawGizmos()
    {
        if (_walkableMap == null) return;

        Vector2Int start = RepositionToIndexMap(new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y)));
        Vector2Int end = RepositionToIndexMap(new Vector2Int(Mathf.RoundToInt(_target.position.x), Mathf.RoundToInt(_target.position.y)));

        Gizmos.color = Color.magenta;
        Gizmos.DrawCube((Vector3Int)start, Vector3.one * 0.75f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube((Vector3Int)end, Vector3.one * 0.75f);

        if (_path != null)
        {
            foreach (Node node in _path)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawCube((Vector3Int)node.position, Vector3.one * 0.75f);
            }
        }
    }
}