using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowForPathfinding : MonoBehaviour {

    [SerializeField, Tooltip("Distancia al siguiente punto de camino")] private float _nextWaypointDistance = 1f;
    [SerializeField, Tooltip("Distancia mínima para recalcular el camino")] private float _repathDistance = 1f;
    [SerializeField, Tooltip("Tiempo entre revisiones de camino")] private float _pathUpdateInterval = 0.5f;
    private Vector2 _lastTargetPosition;

    [Header("Private Data")]
    private int _currentWaypoint = 0;
    private TypeNode[,] _walkableMap;
    private Pathfinding _pathfinding;
    private Transform _target;
    private List<Node> _path;

    [Header("Components")]
    private Rigidbody2D _rb2d;
    private EnemyManager _enemyManager;

    private void Awake()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _enemyManager = GetComponent<EnemyManager>();
        _pathfinding = FindAnyObjectByType<Pathfinding>();
        _target = FindAnyObjectByType<PlayerMovement>().transform;
    }
    private void Start()
    {
        _lastTargetPosition = _target.position;
        StartCoroutine("UpdatePathCoroutine");
    }
    private IEnumerator UpdatePathCoroutine()
    {
        UpdatePath();

        while (true)
        {
            yield return new WaitForSeconds(_pathUpdateInterval);
            if (Vector2.Distance(_target.position, _lastTargetPosition) > _repathDistance)
            {
                UpdatePath();
                _lastTargetPosition = _target.position;
            }
        }
    }
    private void UpdatePath()
    {
        var mapGenerator = FindAnyObjectByType<WalkableMapGenerator>();
        _walkableMap = mapGenerator.walkableMap;

        Vector2Int start = ClearIndexToMap(new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y)));
        Vector2Int end = ClearIndexToMap(new Vector2Int(Mathf.RoundToInt(_target.position.x), Mathf.RoundToInt(_target.position.y)));

        _path = _pathfinding.FindPath(start, end, _walkableMap);
        _currentWaypoint = 0;
    }
    private Vector2Int ClearIndexToMap(Vector2Int position)
    {
        int newX = Mathf.Abs((int)_pathfinding.transform.position.x) + position.x;
        int newY = Mathf.Abs((int)_pathfinding.transform.position.y) + position.y;

        position = new Vector2Int(newX, newY);
        return position;
    }
    private void FixedUpdate()
    {
        if (LoadingScreen.inLoading || Pause.inPause) return;

        if (_path == null || _path.Count == 0) return;

        if (_currentWaypoint >= _path.Count)
        {
            _path = null;
            return;
        }

        if (!_enemyManager.CanMove) return;

        MoveEnemy();
    }
    private void MoveEnemy()
    {
        Vector2 newPosition;
        if (_path.Count > _enemyManager.DistanceToJump && _enemyManager.canJump)
        {
            // SALTO UNICAMENTE SI SUPERA LA DISTANCIA MÍNIMA DE SALTO
            Vector2 newTarget = (Vector2)_path[_enemyManager.DistanceToJump].position;
            newTarget.y += 1.5f;
            newPosition = Vector2.MoveTowards(_rb2d.position, newTarget, (_enemyManager.Speed * 2) * Time.fixedDeltaTime);
            _enemyManager.inJump = true;
        }
        else
        {
            // MOVIMIENTO NORMAL
            newPosition = Vector2.MoveTowards(_rb2d.position, (Vector2)_path[_currentWaypoint].position, _enemyManager.Speed * Time.fixedDeltaTime);
        }

        // APLICAR EL MOVIMIENTO
        _rb2d.MovePosition(newPosition);

        if (_enemyManager.inJump) _enemyManager.StartCoroutine("DelayToJump");

        // CALCULAR LA DISTANCIA AL SIGUIENTE POINT DEL PATH
        float distance = Vector2.Distance(_rb2d.position, _path[_currentWaypoint].position);
        if (distance < _nextWaypointDistance)
        {
            _currentWaypoint++;
        }
    }
    #region Gizmos
    /*
    private void OnDrawGizmo()
    {
        if (_walkableMap == null) return;

        Vector2Int start = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        Vector2Int end = new Vector2Int(Mathf.RoundToInt(_target.position.x), Mathf.RoundToInt(_target.position.y));

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
    */
    #endregion
}
