using System;
using System.Collections;
using UnityEngine;

public abstract class AttackBoss : MonoBehaviour {

    [Header("Visual")]
    [Tooltip("Prefab del ataque visual que se instanciar�")] public GameObject visualAttack;
    [Tooltip("Da�o base del ataque (se suma al da�o del enemigo)")] public int damage;
    [Tooltip("Distancia total del ataque respecto a su punto de origen")] public float distanceToAttack;
    [Range(0f, 5f)] [Tooltip("Tiempo que permanece activo el objeto de ataque antes de destruirse")] public float timeToDestroy;
    public bool moveInAttack;

    [Header("Repeaters")]
    [Tooltip("Cantidad de instancias de ataque que se crear�n")] public int countCreated;
    [Tooltip("Tiempo entre creaci�n de cada piso")] public float timeBetweenCreated;

    [Header("Guides")]
    [Tooltip("Prefab visual que indica por d�nde se ejecutar� el ataque")] public GameObject guideObj;
    [Tooltip("Tiempo que permanece activa la gu�a visual antes de desaparecer")] public float timeToGuide;

    protected Vector3 posInScene;
    [Space]
    protected GameObject guideInScene;
    protected event Action guideCreated;

    [HideInInspector] public EnemyBase enemyParent;
    protected PlayerStats _player;
    private Vector3 _playerPosition;

    private void OnEnable() { _player = FindAnyObjectByType<PlayerStats>(); }
    public IEnumerator Attack()
    {
        posInScene = GetPosition();
        _playerPosition = _player.transform.position;

        CreateGuide(posInScene);

        enemyParent.CanMove = moveInAttack;

        yield return new WaitForSeconds((timeToGuide - 0.25f));

        // Inicia la secuencia de ataque tras mostrar la gu�a
        StartCoroutine("LaunchedAttack");

        // Activa la l�gica de cancelaci�n del ataque desde el enemigo
        enemyParent.StartCoroutine("CancelAttack");
    }
    private void CreateGuide(Vector2 pos)
    {
        guideInScene = Instantiate(guideObj, pos, Quaternion.identity);

        // Evento que notifica a otros scripts que la gu�a fue creada
        guideCreated?.Invoke();

        Destroy(guideInScene, timeToGuide);
    }
    // --- FUNCION INTEGRA ---- //
    protected int GetDamage() { return (damage + enemyParent.damage); }
    // ---- FUNCION ABSTRACT ---- //
    protected abstract IEnumerator LaunchedAttack();
    protected abstract Vector3 GetPosition();
    protected Vector3 GetPlayerPosition() { return _playerPosition; }
}
