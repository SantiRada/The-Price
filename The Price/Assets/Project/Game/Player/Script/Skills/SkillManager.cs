using UnityEngine;

public enum TypeSkill { Neutral = 0, Defensive = 1, Agressive = 2 }
public abstract class SkillManager : MonoBehaviour {

    [Header("Data Skill")]
    public TypeSkill _typeSkill;
    public int _skillName;
    public int _descName;
    public int _featureUsed;

    [Header("Passive")]
    public float _timeToDetectPassive = 4f;
    private float timeBase;

    private void Start() { timeBase = _timeToDetectPassive; }
    private void Update()
    {
        _timeToDetectPassive -= Time.deltaTime;

        if( _timeToDetectPassive < 0 )
        {
            Passive();
            _timeToDetectPassive = timeBase;
        }
    }
    public abstract void Attack();
    public abstract void Passive();
}