using System.Collections;
using UnityEngine;

public enum TypePermanenceOfObject { permanent, perSeconds, inPerfect, perRoom }
public class ChangeStatsObject : Object {

    [Header("Stats Modifiable")]
    [SerializeField] private int _hp;
    [SerializeField] private int _concentration;
    [SerializeField] private int _speedMove;
    [SerializeField, Range(-2, 2)] private float _speedAttack;
    [SerializeField] private int _skillDamage;
    [SerializeField] private int _damage;
    [SerializeField] private int _subsequentDamage;
    [SerializeField] private int _criticChance;
    [SerializeField] private int _missChance;
    [SerializeField] private int _hpStealing;
    [SerializeField] private int _sanity;

    [Header("Prev Values")]
    private int type = -1;
    private float value = -1;
    private int modifier = 0;
    private float _prevValue = 0;

    public override void Use()
    {
        #region Comprobation Values
        if (_hp != 0)
        {
            type = 0;
            value = _hp;
            _prevValue = playerStats.HP;
            if (_hp > 0) modifier = 1;
            else if (_hp < 0) modifier = -1;
        }
        if (_concentration != 0)
        {
            type = 1;
            value = _concentration;

            _prevValue = playerStats.Concentration;

            if (_concentration > 0) modifier = 1;
            else if (_concentration < 0) modifier = -1;
        }
        if (_speedMove != 0)
        {
            type = 2;
            value = _speedMove;

            _prevValue = playerStats.SpeedMove;

            if (_speedMove > 0) modifier = 1;
            else if (_speedMove < 0) modifier = -1;
        }
        if (_speedAttack != 0)
        {
            type = 3;
            value = _speedAttack;

            _prevValue = playerStats.SpeedAttack;

            if (_speedAttack > 0) modifier = 1;
            else if (_speedAttack < 0) modifier = -1;
        }
        if (_skillDamage != 0)
        {
            type = 4;
            value = _damage;

            _prevValue = playerStats.SkillDamage;

            if (_damage > 0) modifier = 1;
            else if (_damage < 0) modifier = -1;
        }
        if (_damage != 0)
        {
            type = 5;
            value = _damage;

            _prevValue = playerStats.Damage;

            if (_damage > 0) modifier = 1;
            else if (_damage < 0) modifier = -1;
        }
        if (_subsequentDamage != 0)
        {
            type = 6;
            value = _subsequentDamage;

            _prevValue = playerStats.SubsequentDamage;

            if (_subsequentDamage > 0) modifier = 1;
            else if (_subsequentDamage < 0) modifier = -1;
        }
        if (_criticChance != 0)
        {
            type = 7;
            value = _criticChance;

            _prevValue = playerStats.CriticChance;

            if (_criticChance > 0) modifier = 1;
            else if (_criticChance < 0) modifier = -1;
        }
        if (_missChance != 0)
        {
            type = 8;
            value = _missChance;

            _prevValue = playerStats.MissChance;

            if (_missChance > 0) modifier = 1;
            else if (_missChance < 0) modifier = -1;
        }
        if (_hpStealing != 0)
        {
            type = 9;
            value = _hpStealing;

            _prevValue = playerStats.StealingHP;

            if (_hpStealing > 0) modifier = 1;
            else if (_hpStealing < 0) modifier = -1;
        }
        if (_sanity != 0)
        {
            type = 10;
            value = _sanity;

            _prevValue = playerStats.Sanity;

            if (_sanity > 0) modifier = 1;
            else if (_sanity < 0) modifier = -1;
        }
        #endregion

        playerStats.SetValue(type, value, modifier);

        if(_typePermanence != TypePermanenceOfObject.permanent)
        {
            if(_typePermanence == TypePermanenceOfObject.perSeconds) StartCoroutine("ResetPerSecond");

            if (_typePermanence == TypePermanenceOfObject.perRoom) RoomManager.finishRoom += ResetValues;

            if (_typePermanence == TypePermanenceOfObject.inPerfect) playerStats.takeDamage += ResetValues;
        }
    }
    private IEnumerator ResetPerSecond()
    {
        yield return new WaitForSeconds(waitForPermanence);
        ResetValues();
    }
    private void ResetValues() { playerStats.SetValue(type, _prevValue, 0); }
}
