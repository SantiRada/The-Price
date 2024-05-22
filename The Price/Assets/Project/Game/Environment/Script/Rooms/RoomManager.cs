using System.Collections;
using UnityEngine;

public class RoomManager : MonoBehaviour {

    [Header("Room Content")]
    [SerializeField] private Room[] _roomPool;
    public float _delayToChangeRoom;

    [Header("UI Content")]
    [SerializeField] private Animator _loadingSector;
    [SerializeField] private float _timeToLoad;

    [Header("Private Content")]
    private PlayerMovement _player;

    private void Awake()
    {
        _player = FindAnyObjectByType<PlayerMovement>();
    }
    private void Start()
    {
        _loadingSector.SetBool("inLoading", false);
        CreateRoom();
    }
    public IEnumerator ChangeState()
    {
        yield return new WaitForSeconds(_delayToChangeRoom);
        _loadingSector.SetBool("inLoading", true);

        yield return new WaitForSeconds(_timeToLoad);
        CreateRoom();
        yield return new WaitForSeconds(_timeToLoad / 2);

        _loadingSector.SetBool("inLoading", false);
    }
    private void CreateRoom()
    {
        int rnd = Random.Range(0, _roomPool.Length);

        Room rm = Instantiate(_roomPool[rnd], Vector3.zero, Quaternion.identity, transform);

        _player.transform.position = rm.spawnPlayer.transform.position;
    }
}
