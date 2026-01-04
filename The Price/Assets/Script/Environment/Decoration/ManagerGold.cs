using System.Collections;
using UnityEngine;

public enum CountGold { Small = 2, Medium = 5, Big = 10 }
public class ManagerGold : MonoBehaviour {

    [Header("Data")]
    public Gold gold;
    public float delayToCreateCoin;
    [Space]
    private static int _count;
    private static Vector3 _position;
    private static bool _canCreate = false;

    private Transform _cam;
    private HUD _hud;

    private void Start()
    {
        _hud = FindAnyObjectByType<HUD>();
        _cam = FindAnyObjectByType<CameraMovement>().transform;
    }
    private void Update()
    {
        transform.position = _cam.position + new Vector3(12, 9, 10);

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

        _hud.SetGold(_count);

        for (int i = 0; i < count; i++)
        {
            Gold money;
            Vector3 finalPos = new Vector3(Random.Range(position.x - 0.5f, position.x + 0.5f), Random.Range(position.y - 0.5f, position.y + 0.5f), 0);

            money = Instantiate(gold.gameObject, finalPos, Quaternion.identity).GetComponent<Gold>();

            yield return new WaitForSeconds(delayToCreateCoin);
        }
    }
}
