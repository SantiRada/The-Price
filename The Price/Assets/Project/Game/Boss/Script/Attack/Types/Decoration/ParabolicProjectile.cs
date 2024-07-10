using UnityEngine;

public class ParabolicProjectile : MonoBehaviour {

    public float speed = 10f;
    public float gravity = -9.81f;
    [Tooltip("Umbral para considerar que ha alcanzado el objetivo")] public float distanceToTarget = 0.1f;
    [HideInInspector] public Vector3 target;

    private Vector2 _startVelocity;
    private Rigidbody2D _rb2d;

    private void OnEnable() { _rb2d = GetComponent<Rigidbody2D>(); }
    private void Start()
    {
        Vector2 direction = (target - transform.position);
        float distance = direction.magnitude;
        direction.Normalize();

        // Calcular el ángulo inicial
        float angle = Mathf.Atan((distance * gravity) / (speed * speed)) / 2.0f;

        // Calcular la velocidad inicial en los componentes X y Y
        float vx = speed * Mathf.Cos(angle);
        float vy = speed * Mathf.Sin(angle);

        // Calcular la velocidad inicial total
        _startVelocity = new Vector2(vx * direction.x, vy);

        // Aplicar la velocidad inicial al proyectil
        _rb2d.velocity = _startVelocity;
    }
    private void Update()
    {
        // Aplica la gravedad manualmente
        _rb2d.velocity += Vector2.up * gravity * Time.deltaTime;

        // Verifica la distancia al objetivo
        if (Vector2.Distance(transform.position, target) < distanceToTarget) { Destroy(gameObject); }
    }
}
