using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowForPathfinding : MonoBehaviour {

    public float initialDelay = 1.0f;

    [SerializeField, Tooltip("Distancia al siguiente punto de camino")] private float _nextWaypointDistance = 1f;
    [SerializeField, Tooltip("Distancia m�nima para recalcular el camino")] private float _repathDistance = 1f;
    [SerializeField, Tooltip("Tiempo entre revisiones de camino")] private float _pathUpdateInterval = 0.5f;
    private Vector2 _lastTargetPosition;

    [Header("Private Data")]
    private int _currentWaypoint = 0;
    private Pathfinding _pathfinding;
    private Transform _target;
    private List<Node> _path;

    [Header("Components")]
    private Rigidbody2D _rb2d;
    private EnemyBase _enemyManager;
    private WalkableMapGenerator mapGenerator;

    private void Awake()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _enemyManager = GetComponent<EnemyBase>();
        _pathfinding = FindAnyObjectByType<Pathfinding>();
        _target = FindAnyObjectByType<PlayerMovement>().transform;
        mapGenerator = FindAnyObjectByType<WalkableMapGenerator>();
    }
    private void Start()
    {
        _lastTargetPosition = _target.position;

        initialDelay = Random.Range(1.0f, 4.5f);

        StartCoroutine(UpdatePathCoroutine());
    }
    private IEnumerator UpdatePathCoroutine()
    {
        yield return new WaitForSeconds(initialDelay);

        UpdatePath();

        while (true)
        {
            yield return new WaitForSeconds(_pathUpdateInterval);

            // Actualizar path si el target se movió O si no hay path válido
            if (Pause.state == State.Game)
            {
                bool targetMoved = Vector2.Distance(_target.position, _lastTargetPosition) > _repathDistance;
                bool needsPath = _path == null || _path.Count == 0;

                if (targetMoved || needsPath)
                {
                    UpdatePath();
                    _lastTargetPosition = _target.position;
                }
            }
        }
    }
    private void UpdatePath()
    {
        if (mapGenerator.walkableMap == null)
        {
            Debug.Log("Walkable Map Not Loaded");
            return;
        }

        Vector2Int start = ClearIndexToMap(new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y)));
        Vector2Int end = ClearIndexToMap(new Vector2Int(Mathf.RoundToInt(_target.position.x), Mathf.RoundToInt(_target.position.y)));

        List<Node> newPath = _pathfinding.FindPath(start, end, mapGenerator.walkableMap);

        if (newPath != null && newPath.Count > 0)
        {
            // Preservar progreso: encontrar el waypoint más cercano en el nuevo path
            if (_path != null && _path.Count > 0 && _currentWaypoint < _path.Count)
            {
                Vector2 currentTarget = _path[_currentWaypoint].position;
                _currentWaypoint = FindClosestWaypointIndex(newPath, currentTarget);
            }
            else
            {
                _currentWaypoint = 0;
            }

            _path = newPath;
        }
    }

    /// <summary>
    /// Encuentra el índice del waypoint más cercano a una posición
    /// </summary>
    private int FindClosestWaypointIndex(List<Node> path, Vector2 position)
    {
        int closestIndex = 0;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < path.Count; i++)
        {
            float distance = Vector2.Distance(path[i].position, position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }

        return closestIndex;
    }
    private Vector2Int ClearIndexToMap(Vector2Int position)
    {
        // Usar el offset correcto desde mapGenerator.InitialPosition
        // NO usar Mathf.Abs() ya que destruye coordenadas negativas
        Vector2Int offset = new Vector2Int(
            Mathf.RoundToInt(mapGenerator.InitialPosition.x),
            Mathf.RoundToInt(mapGenerator.InitialPosition.y)
        );

        // Las coordenadas del mundo YA incluyen el offset, solo se pasan directamente
        return position;
    }
    private void FixedUpdate()
    {
        if (LoadingScreen.inLoading || Pause.state != State.Game) return;

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
        Vector2 newPosition = Vector2.MoveTowards(_rb2d.position, (Vector2)_path[_currentWaypoint].position, _enemyManager.speed * Time.fixedDeltaTime);

        // APLICAR EL MOVIMIENTO
        _rb2d.MovePosition(newPosition);

        // CALCULAR LA DISTANCIA AL SIGUIENTE POINT DEL PATH
        float distance = Vector2.Distance(_rb2d.position, _path[_currentWaypoint].position);
        if (distance < _nextWaypointDistance) { _currentWaypoint++; }
    }

    #region Gizmos
    private void OnDrawGizmosSelected()
    {
        if (_path == null || _path.Count < 1) return;

        Gizmos.color = Color.yellow;

        for (int i = 0; i < _path.Count; i++)
        {
            Gizmos.DrawSphere(new Vector3(_path[i].position.x, _path[i].position.y, 0), 0.15f);

            if (i < _path.Count - 1)
            {
                Gizmos.DrawLine(new Vector3(_path[i].position.x, _path[i].position.y, 0), new Vector3(_path[i+1].position.x, _path[i+1].position.y, 0));
            }
        }

        // Marcar el waypoint actual en otro color
        if (_currentWaypoint < _path.Count)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(new Vector3(_path[_currentWaypoint].position.x, _path[_currentWaypoint].position.y, 0), 0.25f);
        }
    }
    #endregion
}