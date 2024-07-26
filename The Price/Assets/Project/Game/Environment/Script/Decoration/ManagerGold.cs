using System.Collections;
using UnityEngine;

public enum CountGold { Small = 2, Medium = 5, Big = 10 }
public enum TypeCollectable { Gold, Souls }
public class ManagerGold : MonoBehaviour {

    [Header("Data")]
    public Gold gold;
    public Gold souls;
    public float delayToCreateCoin;
    [Space]
    private static int _count;
    private static Vector3 _position;
    private static bool _canCreate = false;
    private static TypeCollectable _typeCollectable;

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
        _typeCollectable = TypeCollectable.Gold;
        _position = position;
        _count = (int)count;
        _canCreate = true;
    }
    public static void CreateSouls(Vector3 position, int count)
    {
        _typeCollectable = TypeCollectable.Souls;
        _position = position;
        _count = count;
        _canCreate = true;
    }
    private IEnumerator Creator(Vector3 position, int count)
    {
        _canCreate = false;

        if (_typeCollectable == TypeCollectable.Gold) _hud.SetGold(_count);
        else _hud.SetSouls(_count);

        for (int i = 0; i < count; i++)
        {
            Gold money;
            Vector3 finalPos = new Vector3(Random.Range(position.x - 0.5f, position.x + 0.5f), Random.Range(position.y - 0.5f, position.y + 0.5f), 0);

            if (_typeCollectable == TypeCollectable.Gold) money = Instantiate(gold.gameObject, finalPos, Quaternion.identity).GetComponent<Gold>();
            else money = Instantiate(souls.gameObject, finalPos, Quaternion.identity).GetComponent<Gold>();
            
            money.target = transform;

            yield return new WaitForSeconds(delayToCreateCoin);
        }
    }
}
