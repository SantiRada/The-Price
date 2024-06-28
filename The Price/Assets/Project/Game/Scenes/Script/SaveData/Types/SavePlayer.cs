using System.Collections.Generic;

[System.Serializable]
public class SavePlayer
{
    public float maxPV;
    public float maxConcentracion;
    public float speedMovement;
    public float speedAttack;
    public float skillDamage;
    public float damage;
    public float subsequenceDamage;
    public float criticalChance;
    public float missChance;
    public float stealing;
    public float maxSanity;

    public float pv;
    public float concentracion;
    public float sanity;

    public int gold;

    public WeaponSystem weaponInHand;

    public List<SkillManager> skills;
    public List<Object> objects;
}
