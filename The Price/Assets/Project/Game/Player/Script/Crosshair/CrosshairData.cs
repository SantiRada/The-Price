using UnityEngine;

public class CrosshairData : MonoBehaviour {

    [Header("Data Rotation")]
    [SerializeField] private float _distanceCursor = 5f;
    [SerializeField] private float _rotationSpeed;
    private Vector2 _direction, _posEnemy;

    [Header("Auto Crosshair Data")]
    [SerializeField] private int _levelHelp = 4;
    public bool active = true;
    private Vector2 _directionBase;

    [Header("Object")]
    private BoxCollider2D _sectorCross;

    private void Awake()
    {
        _sectorCross = GetComponent<BoxCollider2D>();
    }
    private void Start()
    {
        if (active)
        {
            _sectorCross.offset = new Vector2(0, _levelHelp * 0.75f);
            _sectorCross.size = new Vector2(_levelHelp, _levelHelp * 1.5f);
        }
        else
        {
            _sectorCross.enabled = false;
        }
    }
    private void Update()
    {
        if (Pause.Comprobation(State.Game)) return;

        Crosshair();
    }
    private void Crosshair()
    {
        // ESPACIO DE CORTE SEGÚN AUTO-AIM
        if (active)
        {
            if(_posEnemy != Vector2.zero && _posEnemy != null)
            {
                _direction = new Vector2(_posEnemy.x - transform.position.x, _posEnemy.y - transform.position.y).normalized * _distanceCursor;
            }
            else
            {
                if (_directionBase.x != 0 || _directionBase.y != 0) _direction = new Vector2(_directionBase.x, _directionBase.y).normalized * _distanceCursor;
            }
        }
        else
        {
            if (_directionBase.x != 0 || _directionBase.y != 0) _direction = new Vector2(_directionBase.x, _directionBase.y).normalized * _distanceCursor;
        }

        transform.up = _direction;

        if (_direction != Vector2.zero) transform.localPosition = Vector3.Slerp(transform.position, _direction, _rotationSpeed);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Enemy") _posEnemy = (Vector2)collision.transform.position;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Enemy") _posEnemy = Vector2.zero;
    }
    // ---- GETTERS ---- //
    public Vector2 GetPosEnemy() { return _posEnemy; }
    public void SetAimDirection(Vector2 direction)
    {
        _directionBase = direction;
    }
    public Vector2 GetCurrentAimDirection()
    {
        return _direction;
    }

}
