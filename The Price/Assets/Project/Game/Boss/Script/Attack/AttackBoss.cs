using System;
using System.Collections;
using UnityEngine;

public abstract class AttackBoss : MonoBehaviour {

    [Header("Visual")]
    public GameObject visualAttack;
    public int damage;
    public float distanceToAttack;
    [Range(0f, 5f)] public float timeToDestroy;

    [Header("Repeaters")]
    public int countCreated;
    [Tooltip("Tiempo entre creación de cada piso")] public float timeBetweenCreated;

    [Header("Parabolic")]
    public bool isParabolic;
    public ParabolicProjectile parabolicObj;

    [Header("Guides")]
    public GameObject guideObj;
    public float timeToGuide;
    protected Vector3 posInScene;
    [Space]
    protected GameObject guideInScene;
    protected event Action guideCreated;

    [HideInInspector] public BossSystem bossParent;
    protected PlayerStats _player;
    private Vector3 _playerPosition;

    private void OnEnable() { _player = FindAnyObjectByType<PlayerStats>(); }
    public IEnumerator Attack()
    {
        posInScene = GetPosition();

        _playerPosition = _player.transform.position;

        CreateGuide(posInScene);

        if (isParabolic)
        {
            ParabolicProjectile obj = Instantiate(parabolicObj.gameObject, transform.position, Quaternion.identity).GetComponent<ParabolicProjectile>();
            obj.target = _player.transform.position;
        }

        yield return new WaitForSeconds((timeToGuide - 0.25f));

        StartCoroutine("LaunchedAttack");

        bossParent.StartCoroutine("CancelAttack");
    }
    private void CreateGuide(Vector2 pos)
    {
        guideInScene = Instantiate(guideObj, pos, Quaternion.identity);

        // EVENTO PARA SABER QUE LA GUÍA SE CREÓ
        guideCreated?.Invoke();

        Destroy(guideInScene, timeToGuide);
    }
    // --- FUNCION INTEGRA ---- //
    protected int GetDamage() { return (damage + bossParent.damageMultiplier); }
    // ---- FUNCION ABSTRACT ---- //
    protected abstract IEnumerator LaunchedAttack();
    protected abstract Vector3 GetPosition();
    protected Vector3 GetPlayerPosition() { return _playerPosition; }
}