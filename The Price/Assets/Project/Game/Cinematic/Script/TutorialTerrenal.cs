using System.Collections;
using UnityEngine;

public class TutorialTerrenal : MonoBehaviour {

    [Header("Scene 01")]
    public CallVoice[] _voices;
    public InteractiveWeapon[] weapons;

    [Header("Scene 02")]
    public GameObject talkativeEnemy;

    [Header("Scene 05")]
    public GameObject soulBoy;
    public GameObject soulObject;

    private SaveLoadManager _saveLoad;
    private RoomManager _roomManager;
    private PlayerStats _playerStats;

    private void Awake()
    {
        _playerStats = FindAnyObjectByType<PlayerStats>();
        _roomManager = FindAnyObjectByType<RoomManager>();
        _saveLoad = FindAnyObjectByType<SaveLoadManager>();
    }
    private void Start()
    {
        if(_saveLoad.GetWorldData() != null) { if (_saveLoad.GetWorldData().passedTutorial) Destroy(this); }

        LoadingScreen.finishLoading += InitialContent;
    }
    private IEnumerator VerifyRoom()
    {
        yield return new WaitForSeconds(1f);

        if (_roomManager._countRoomsComplete == 1)
        {
            Vector3 pos = _roomManager.GetRoom._livingEnemies[0].transform.position;
            GameObject obj = Instantiate(talkativeEnemy, pos, Quaternion.identity, _roomManager.GetRoom._livingEnemies[0].transform);

            Instantiate(_voices[2].gameObject, Vector3.zero, Quaternion.identity);
        }
        if (_roomManager._countRoomsComplete == 2)
        {
            Instantiate(_voices[3].gameObject, _playerStats.transform.position, Quaternion.identity);
        }
        if (_roomManager._countRoomsComplete == 4)
        {
            Vector3 pos = _roomManager.GetRoom._livingEnemies[_roomManager.GetRoom._livingEnemies.Count - 1].transform.position;
            GameObject obj = Instantiate(soulBoy, pos, Quaternion.identity);
            _roomManager.SetNecessaryInteraction();

            yield return new WaitForSeconds(0.5f);

            Instantiate(_voices[4].gameObject, _playerStats.transform.position, Quaternion.identity);

            yield return new WaitForSeconds(3f);

            pos = obj.transform.position + new Vector3(obj.transform.position.x > 0 ? -2 : 2, obj.transform.position.y < -2 ? 1 : -1, 0);
            Instantiate(soulObject, pos, Quaternion.identity);
        }
    }
    private void InitialContent()
    {
        Instantiate(_voices[0].gameObject, _voices[0].positionToCreate, Quaternion.identity);

        Instantiate(_voices[1].gameObject, _voices[1].positionToCreate, Quaternion.identity);

        Vector3 newPos = new Vector3(_voices[1].positionToCreate.x - 4, _voices[1].positionToCreate.y - 1f, _voices[1].positionToCreate.z);
        for (int i = 0; i < weapons.Length; i++)
        {
            Instantiate(weapons[i], newPos, Quaternion.identity);
            newPos = new Vector3(newPos.x + 4, (i == 0 ? newPos.y + 1 : (i == 1 ? newPos.y - 1 : newPos.y)), newPos.z);
        }

        RoomManager.advanceRoom += () => StartCoroutine(VerifyRoom());
    }
    private void OnDestroy() { LoadingScreen.finishLoading -= InitialContent; }
}