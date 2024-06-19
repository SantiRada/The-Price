using UnityEngine;

public class Bounce : MonoBehaviour {

    public float jumpHeight = 1.0f;  // Altura máxima del salto
    public float jumpDistance = 1.0f;  // Distancia en X durante el salto
    public float jumpDuration = 0.65f;  // Duración del salto

    private Vector3 initialPosition;
    private bool isJumping = false;
    private float jumpStartTime;

    private void Start() { DoJump(); }
    public void DoJump()
    {
        if (!isJumping)
        {
            initialPosition = transform.position;
            jumpStartTime = Time.time;
            isJumping = true;

            Destroy(this, jumpDuration * 2);
        }
    }
    private void Update()
    {
        if (isJumping)
        {
            float elapsedTime = Time.time - jumpStartTime;
            float progress = elapsedTime / jumpDuration;

            if (progress < 1.0f)
            {
                float newX = Mathf.Lerp(initialPosition.x, initialPosition.x + jumpDistance, progress);
                float newY = initialPosition.y + Mathf.Sin(Mathf.PI * progress) * jumpHeight;
                transform.position = new Vector3(newX, newY, initialPosition.z);
            }
            else
            {
                transform.position = new Vector3(initialPosition.x + jumpDistance, initialPosition.y, initialPosition.z);
                isJumping = false;
            }
        }
    }
}
