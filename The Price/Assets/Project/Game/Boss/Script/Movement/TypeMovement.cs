using UnityEngine;

public abstract class TypeMovement : MonoBehaviour {

    [Header("Data Movement")]
    public float timeToCancel;
    public float speedMove = 0;
    [HideInInspector] public bool inMove = false;

    [Header("Private Content")]
    protected PlayerStats _playerStats;
    protected EnemyBase _boss;

    private void OnEnable()
    {
        _boss = GetComponent<EnemyBase>();
        _playerStats = FindAnyObjectByType<PlayerStats>();

        inMove = false;
    }
    private void Update()
    {
        if (inMove)
        {
            timeToCancel -= Time.deltaTime;

            if (timeToCancel <= 0) _boss.StartCoroutine("CancelMove");

            DataMove();
        }
    }
    public void CancelMove() { inMove = false; }
    public void Move() { inMove = true; }
    public abstract void DataMove();
}