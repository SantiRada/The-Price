using UnityEngine;

public class SkillLaunched : SkillManager {

    private CrosshairData _crosshair;
    private Rigidbody2D _rb2d;

    private void Start()
    {
        _rb2d = GetComponent<Rigidbody2D>();

        _crosshair = FindAnyObjectByType<CrosshairData>();
    }
    private void Update()
    {
        if (typeShow == TypeShowSkill.launched)
        {
            Vector2 direction = Disperser();
            _rb2d.velocity = direction * _player.GetterStats(3, false);
        }
    }
    protected override void TakeEffect() { Debug.Log("Skill Launched!"); }
    private Vector2 Disperser()
    {
        float dispersion = Random.Range(0f, 1f);
        Vector2 direction = (_crosshair.transform.position - _player.transform.position);

        direction.x += Random.Range(-dispersion, dispersion);
        direction.y += Random.Range(-dispersion, dispersion);
        direction.Normalize();

        return direction;
    }
}
