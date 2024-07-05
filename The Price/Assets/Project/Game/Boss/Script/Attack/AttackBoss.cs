using System.Collections;
using UnityEngine;

public abstract class AttackBoss : MonoBehaviour {

    [Header("Visual")]
    public GameObject visualAttack;
    public int damage;
    public float distanceToAttack;
    [Range(0f, 5f)] public float timeToDestroy;

    [Header("Guides")]
    public GameObject guideObj;
    public float timeToGuide;
    protected Vector3 posInScene;

    [HideInInspector] public BossSystem bossParent;
    protected PlayerStats _player;

    private void OnEnable() { _player = FindAnyObjectByType<PlayerStats>(); }
    public IEnumerator Attack()
    {
        posInScene = GetPosition();

        CreateGuide(posInScene);

        yield return new WaitForSeconds((timeToGuide - 0.25f));

        LaunchedAttack();

        bossParent.CancelAttack();
    }
    private void CreateGuide(Vector2 pos)
    {
        GameObject obj = Instantiate(guideObj, pos, Quaternion.identity);
        Destroy(obj, timeToGuide);
    }
    // --- FUNCION INTEGRA ---- //
    protected int GetDamage() { return (damage + bossParent.damageMultiplier); }
    // ---- FUNCION ABSTRACT ---- //
    protected abstract void LaunchedAttack();
    protected abstract Vector3 GetPosition();
}