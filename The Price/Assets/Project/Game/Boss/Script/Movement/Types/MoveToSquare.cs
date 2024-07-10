using UnityEngine;

public class MoveToSquare : TypeMovement {

    [Tooltip("Amplitud del direccionamiento tomado por el Boss")] public float intensity;
    private Vector3 nextPos;
    private int index = 0;

    private void Start() { CalculateNewPos(); }
    public override void DataMove() { transform.position = Vector3.Lerp(transform.position, nextPos, speedMove * Time.deltaTime); }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Unwalkable")) { CalculateNewPos(); }
    }
    private void CalculateNewPos()
    {
        switch (index)
        {
            case 0: nextPos = transform.position + new Vector3(intensity, intensity, 0); break;
            case 1: nextPos = transform.position + new Vector3(intensity, -intensity, 0); break;
            case 2: nextPos = transform.position + new Vector3(-intensity, -intensity, 0); break;
            case 3: nextPos = transform.position + new Vector3(-intensity, intensity, 0); break;
        }

        if (index >= 3) index = 0;
        else index++;
    }
}
