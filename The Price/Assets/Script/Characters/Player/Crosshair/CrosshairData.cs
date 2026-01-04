using System.Collections.Generic;
using UnityEngine;

public class CrosshairData : MonoBehaviour {

    [Header("Data Rotation")]
    [SerializeField] private float _distanceCursor = 5f;
    [SerializeField] private float _rotationSpeed;
    private Vector2 _direction, _posEnemy;

    [Header("Auto Crosshair Data")]
    [SerializeField] private float _detectionRadius = 8f; // Radio de detecci\u00f3n circular
    public bool active = true;
    private Vector2 _directionBase;

    [Header("Object")]
    public List<GameObject> enemies = new List<GameObject>();
    public float delayBetweenRedirection;
    private float delayBaseRedirection;
    private ActionForControlPlayer _controlPlayer;
    private int index = 0;
    private bool crossWithStick = false;

    [Header("Debug")]
    [SerializeField] private bool showDetectionRadius = false; // Para debug visual

    private void Awake()
    {
        _controlPlayer = FindAnyObjectByType<ActionForControlPlayer>();
    }

    private void Start()
    {
        delayBaseRedirection = delayBetweenRedirection;
        delayBetweenRedirection = 0;
    }

    private void Update()
    {
        if (Pause.Comprobation(State.Game)) return;

        // Actualizar lista de enemigos en rango circular
        if (active)
        {
            UpdateEnemiesInRange();
        }

        if (enemies.Count != 0)
        {
            Vector2 rightStick = _controlPlayer.RightStick();
            if (rightStick != Vector2.zero || crossWithStick)
            {
                // SIRVE PARA QUE AL TOCAR EL STICK DERECHO MIRES A ALGUIEN M\u00c1S QUE EST\u00c9 CERCA
                delayBetweenRedirection -= Time.deltaTime;

                if(delayBetweenRedirection <= 0)
                {
                    VerifyRightStick(rightStick);
                    delayBetweenRedirection = delayBaseRedirection;
                }

            }
            else { RevaluateIndex(); /* Sirve para que se haga auto-aim al enemigo m\u00e1s cercano al player */ }

            // VERIFICACI\u00d3N DE QUE ESTE OBJETIVO SIGA EN LA ESCENA Y EN LA VISTA
            if (index > (enemies.Count - 1)) RevaluateIndex();

            if (enemies.Count > 0 && index < enemies.Count && enemies[index] != null)
            {
                _posEnemy = enemies[index].transform.position;
            }
        }
        else { index = 0; crossWithStick = false; }

        Crosshair();
    }

    /// <summary>
    /// Actualiza la lista de enemigos usando detecci\u00f3n circular en 360\u00b0
    /// </summary>
    private void UpdateEnemiesInRange()
    {
        enemies.Clear();

        // Detecci\u00f3n circular omnidireccional
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, _detectionRadius);

        foreach (Collider2D collider in hitColliders)
        {
            if (collider.CompareTag("Enemy") || collider.CompareTag("Boss"))
            {
                enemies.Add(collider.gameObject);
            }
        }
    }
    private void VerifyRightStick(Vector2 rightStick)
    {
        if (enemies.Count <= 1) return;

        float absX = Mathf.Abs(rightStick.x);
        float absY = Mathf.Abs(rightStick.y);

        #region CreateSublist
        Dictionary<GameObject, int> sublistEnemies = new Dictionary<GameObject, int>();
        // CAMBIAR AL ENEMIGO QUE EST� A LA IZQUIERDA
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
        // ESPACIO DE CORTE SEG�N AUTO-AIM
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

    // Debug visual para ver el radio de detecci\u00f3n
    private void OnDrawGizmosSelected()
    {
        if (showDetectionRadius)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _detectionRadius);
        }
    }

    // ---- GETTERS ---- //
    public Vector2 GetPosEnemy() { return _posEnemy; }
    public void SetAimDirection(Vector2 direction) { _directionBase = direction; }
    public Vector2 GetCurrentAimDirection() { return _direction; }
}