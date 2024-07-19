using System.Collections.Generic;
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
    public List<GameObject> enemies = new List<GameObject>();
    public float delayBetweenRedirection;
    private float delayBaseRedirection;
    private BoxCollider2D _sectorCross;
    private ActionForControlPlayer _controlPlayer;
    private int index = 0;
    private bool crossWithStick = false;

    private void Awake()
    {
        _sectorCross = GetComponent<BoxCollider2D>();
        _controlPlayer = FindAnyObjectByType<ActionForControlPlayer>();
    }
    private void Start()
    {
        delayBaseRedirection = delayBetweenRedirection;
        delayBetweenRedirection = 0;

        if (active)
        {
            _sectorCross.offset = new Vector2(0, _levelHelp * 0.75f);
            _sectorCross.size = new Vector2(_levelHelp, _levelHelp * 1.5f);
        }
        else { _sectorCross.enabled = false; }
    }
    private void Update()
    {
        if (Pause.Comprobation(State.Game)) return;

        if (enemies.Count != 0)
        {
            Vector2 rightStick = _controlPlayer.RightStick();
            if (rightStick != Vector2.zero || crossWithStick)
            {
                // SIRVE PARA QUE AL TOCAR EL STICK DERECHO MIRES A ALGUIEN MÁS QUE ESTÉ CERCA
                delayBetweenRedirection -= Time.deltaTime;

                if(delayBetweenRedirection <= 0)
                {
                    VerifyRightStick(rightStick);
                    delayBetweenRedirection = delayBaseRedirection;
                }

            }
            else { RevaluateIndex(); /* Sirve para que se haga auto-aim al enemigo más cercano al player */ }

            // VERIFICACIÓN DE QUE ESTE OBJETIVO SIGA EN LA ESCENA Y EN LA VISTA
            if (index > (enemies.Count - 1)) RevaluateIndex();

            _posEnemy = enemies[index].transform.position;
        }
        else { index = 0; crossWithStick = false; }

        Crosshair();
    }
    private void VerifyRightStick(Vector2 rightStick)
    {
        if (enemies.Count <= 1) return;

        float absX = Mathf.Abs(rightStick.x);
        float absY = Mathf.Abs(rightStick.y);

        #region CreateSublist
        Dictionary<GameObject, int> sublistEnemies = new Dictionary<GameObject, int>();
        // CAMBIAR AL ENEMIGO QUE ESTÉ A LA IZQUIERDA
        for (int i = 0; i < enemies.Count; i++)
        {
            if (i == index) continue;
            if (absX > absY)
            {
                // BUSCO EN IZQUIERDA O DERECHA
                if (rightStick.x > 0)
                {
                    // PREGUNTO POR DERECHA
                    if (enemies[i].transform.position.x > enemies[index].transform.position.x) { sublistEnemies.Add(enemies[i], i); }
                }
                else
                {
                    // PREGUNTO POR IZQUIERDA
                    if (enemies[i].transform.position.x < enemies[index].transform.position.x) { sublistEnemies.Add(enemies[i], i); }
                }
            }
            else
            {
                if (absX == 0 && absY == 0) return;

                // BUSCO POR ARRIBA O ABAJO
                if (rightStick.y > 0)
                {
                    // PREGUNTO POR ARRIBA
                    if (enemies[i].transform.position.y > enemies[index].transform.position.y) { sublistEnemies.Add(enemies[i], i); }
                }
                else
                {
                    // PREGUNTO POR ABAJO
                    if (enemies[i].transform.position.y < enemies[index].transform.position.y) { sublistEnemies.Add(enemies[i], i); }
                }
            }
        }
        #endregion

        #region CalculateMinDistance
        float distance = 1000;
        int newIndex = 0;
        foreach (KeyValuePair<GameObject, int> entry in sublistEnemies)
        {
            GameObject obj = entry.Key;
            int value = entry.Value;

            if (Vector2.Distance(enemies[index].transform.position, obj.transform.position) < distance)
            {
                newIndex = value;
                distance = Vector2.Distance(obj.transform.position, enemies[index].transform.position);
            }
        }
        #endregion

        crossWithStick = true;

        index = newIndex;
    }
    private void RevaluateIndex()
    {
        if (enemies.Count == 0) return;

        float distance = 1000;
        for (int i = 0; i < enemies.Count; i++)
        {
            if(Vector2.Distance(transform.position, enemies[i].transform.position) < distance)
            {
                index = i;
                distance = Vector2.Distance(transform.position, enemies[i].transform.position);
            }
        }
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
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) { enemies.Add(collision.gameObject); }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            enemies.Remove(collision.gameObject);

            _posEnemy = Vector2.zero;
        }
    }
    // ---- GETTERS ---- //
    public Vector2 GetPosEnemy() { return _posEnemy; }
    public void SetAimDirection(Vector2 direction) { _directionBase = direction; }
    public Vector2 GetCurrentAimDirection() { return _direction; }
}