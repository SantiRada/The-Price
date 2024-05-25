using System.Collections;
using UnityEngine;

public enum CountGold { Small = 2, Medium = 5, Big = 10 }
public class ManagerGold : MonoBehaviour {

    [Header("Data")]
    [SerializeField] private Gold _gold;
    [SerializeField] private float _delayToCreateCoin;
    private static Vector3 _position;
    private static int _count;
    private static bool _canCreate = false;

    private Transform _cam;
    private Vector3 _initPos;

    private void Start()
    {
        _cam = FindAnyObjectByType<CameraMovement>().transform;
        _initPos = transform.position;
    }
    private void Update()
    {
        transform.position = _cam.position - new Vector3(Mathf.Abs(_initPos.x), Mathf.Abs(_initPos.y), -10);

        if (_canCreate) StartCoroutine(Creator(_position, _count));
    }
    public static void CreateGold(Vector3 position, CountGold count)
    {
        _position = position;
        _count = (int)count;
        _canCreate = true;
    }
    private IEnumerator Creator(Vector3 position, int count)
    {
        _canCreate = false;
        for(int i = 0; i < count; i++)
        {
            Vector3 finalPos = new Vector3(Random.Range(position.x - 0.5f, position.x + 0.5f), Random.Range(position.y - 0.5f, position.y + 0.5f), 0);

            Gold money = Instantiate(_gold.gameObject, finalPos, Quaternion.identity).GetComponent<Gold>();
            money.target = transform;

            yield return new WaitForSeconds(_delayToCreateCoin);
        }
    }
}
