using UnityEngine;

public abstract class TypeMovement : MonoBehaviour {

    [Header("Data Movement")]
    [HideInInspector] public bool inMove = false;

    [Header("Private Content")]
    protected PlayerStats _playerStats;
    protected BossSystem _boss;

    private void OnEnable()
    {
        _boss = GetComponent<BossSystem>();
        _playerStats = FindAnyObjectByType<PlayerStats>();
    }
    public void CancelMove() { inMove = false; }
    public abstract void Move(float speed);
}