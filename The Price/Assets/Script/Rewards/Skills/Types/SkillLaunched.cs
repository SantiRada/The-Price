using UnityEngine;

public class SkillLaunched : SkillManager {

    [SerializeField, Range(0, 0.5f)] private float _disperser;

    private CrosshairData _crosshair;
    private Rigidbody2D _rb2d;

    private Vector2 direction;

    private void Start()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _crosshair = _player.GetComponentInChildren<CrosshairData>();

        direction = Disperser();
    }
    private void Update()
    {
        if (typeShow == TypeShowSkill.launched)
        {
            _rb2d.linearVelocity = direction * _player.GetterStats(3, false);
        }
    }
    protected override void TakeEffect() { Debug.Log("Skill Launched!"); }
    private Vector2 Disperser()
    {
        float dispersion = Random.Range(0, _disperser);
        Vector2 dir = (_crosshair.transform.position - _player.transform.position);

        dir.x += Random.Range(-dispersion, dispersion);
        dir.y += Random.Range(-dispersion, dispersion);
        dir.Normalize();

        return dir;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if(hasState) collision.GetComponent<EnemyManager>().AddState(state, countOfLoads);
        }
    }
}
