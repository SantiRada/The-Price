using UnityEngine;

public class Room : MonoBehaviour {

    public GameObject spawnPlayer;

    private RoomManager _roomManager;

    private void Awake()
    {
        _roomManager = GetComponentInParent<RoomManager>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _roomManager.StartCoroutine("ChangeState");
            Destroy(gameObject, _roomManager._delayToChangeRoom);
        }
    }
}
