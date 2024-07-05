using UnityEngine;

public abstract class TypeMovement : MonoBehaviour {

    protected PlayerStats _playerStats;
    protected BossSystem _boss;

    private void OnEnable()
    {
        _boss = GetComponent<BossSystem>();
        _playerStats = FindAnyObjectByType<PlayerStats>();
    }
    public abstract void Move(float speed);
}