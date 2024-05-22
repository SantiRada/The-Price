using UnityEngine;

public class CameraMovement : MonoBehaviour {

    [SerializeField] private GameObject _target;
    [SerializeField] private float offset;
    [SerializeField] private Vector2 min, max;
    private Vector3 _initialPos;

    private void Start()
    {
        _initialPos = transform.position;
    }
    private void Update()
    {
        Vector3 newPos = new Vector3(Mathf.Clamp(_target.transform.position.x, min.x, max.x), Mathf.Clamp(_target.transform.position.y, min.y, max.y), _initialPos.z);
        transform.position = Vector3.Lerp(transform.position, newPos, offset * Time.deltaTime);
    }
}
