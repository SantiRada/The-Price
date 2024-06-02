using UnityEngine;

public class ChangeStatsObject : Object {

    [Header("Stats Modifiable")]
    [SerializeField] private int _hp;
    [SerializeField] private int _concentration;
    [SerializeField] private int _speedMove;
    [SerializeField, Range(-2, 2)] private float _speedAttack;
    [SerializeField] private int _damage;
    [SerializeField] private int _subsequentDamage;
    [SerializeField] private int _criticChance;
    [SerializeField] private int _missChance;
    [SerializeField] private int _hpStealing;
    [SerializeField] private int _sanity;

    public override void Use()
    {
        int type = -1;
        float value = -1;
        int modifier = 0;

        #region Comprobation Values
        if (_hp != 0)
        {
            type = 0;
            value = _hp;
            if (_hp > 0) modifier = 1;
            else if (_hp < 0) modifier = -1;
        }
        if (_concentration != 0)
        {
            type = 0;
            value = _concentration;
            if (_concentration > 0) modifier = 1;
            else if (_concentration < 0) modifier = -1;
        }
        if (_speedMove != 0)
        {
            type = 0;
            value = _speedMove;
            if (_speedMove > 0) modifier = 1;
            else if (_speedMove < 0) modifier = -1;
        }
        if (_speedAttack != 0)
        {
            type = 0;
            value = _speedAttack;
            if (_speedAttack > 0) modifier = 1;
            else if (_speedAttack < 0) modifier = -1;
        }
        if (_damage != 0)
        {
            type = 0;
            value = _damage;
            if (_damage > 0) modifier = 1;
            else if (_damage < 0) modifier = -1;
        }
        if (_subsequentDamage != 0)
        {
            type = 0;
            value = _subsequentDamage;
            if (_subsequentDamage > 0) modifier = 1;
            else if (_subsequentDamage < 0) modifier = -1;
        }
        if (_criticChance != 0)
        {
            type = 0;
            value = _criticChance;
            if (_criticChance > 0) modifier = 1;
            else if (_criticChance < 0) modifier = -1;
        }
        if (_missChance != 0)
        {
            type = 0;
            value = _missChance;
            if (_missChance > 0) modifier = 1;
            else if (_missChance < 0) modifier = -1;
        }
        if (_sanity != 0)
        {
            type = 0;
            value = _sanity;
            if (_sanity > 0) modifier = 1;
            else if (_sanity < 0) modifier = -1;
        }
        #endregion

        playerStats.SetValue(type, value, modifier);
    }
}
