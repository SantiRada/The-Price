using System.Collections;
using UnityEngine;

public class TutorialTerrenal : Tutorial {

    [Header("Scene 01")]
    public CallVoice[] _voices;
    public InteractiveWeapon[] weapons;
    private int countWeapon = 0;

    [Header("Scene 02")]
    public GameObject talkativeEnemy;

    [Header("Scene 05")]
    public GameObject soulBoy;
    public GameObject soulObject;

    private PlayerStats _player;

    private void Awake() { _player = FindAnyObjectByType<PlayerStats>(); }
    private void Start()
    {
        canCreateChest = false;

        _roomManager.canCreateChest = canCreateChest;
    }
    public override void CallMadeInteraction()
    {
        // Sistema simplificado: solo hay 1 arma ahora
        countWeapon = (_player.weapon != null) ? 1 : 0;

        if(countWeapon >= 1) { RoomManager.CallMadeInteraction(); }
    }
    protected override IEnumerator ChangesInZeroRoom()
    {
        yield return new WaitForSeconds(0.1f);

        Instantiate(_voices[0].gameObject, _voices[0].positionToCreate, Quaternion.identity);

        Instantiate(_voices[1].gameObject, _voices[1].positionToCreate, Quaternion.identity);

        Vector3 newPos = new Vector3(_voices[1].positionToCreate.x - 4, _voices[1].positionToCreate.y - 0.5f, _voices[1].positionToCreate.z);
        for (int i = 0; i < weapons.Length; i++)
        {
            Instantiate(weapons[i], newPos, Quaternion.identity);
            newPos = new Vector3(newPos.x + 4, (i == 0 ? newPos.y + 1 : (i == 1 ? newPos.y - 1 : newPos.y)), newPos.z);
        }
    }
    protected override IEnumerator ChangesInFirstRoom()
    {
        yield return new WaitForSeconds(0.1f);

        Vector3 pos = _roomManager.GetRoom._activeEnemies[0].transform.position;
        GameObject obj = Instantiate(talkativeEnemy, pos, Quaternion.identity, _roomManager.GetRoom._activeEnemies[0].transform);

        Instantiate(_voices[2].gameObject, Vector3.zero, Quaternion.identity);
    }
    protected override IEnumerator ChangesInSecondRoom()
    {
        yield return new WaitForSeconds(0.1f);

        Instantiate(_voices[3].gameObject, _playerStats.transform.position, Quaternion.identity);
    }
    protected override IEnumerator ChangesInThirdRoom()
    {
        yield return new WaitForSeconds(0.1f);
    }
    protected override IEnumerator ChangesInFourthRoom()
    {
        yield return new WaitForSeconds(0.1f);

        Vector3 pos = _roomManager.GetRoom._livingEnemies[_roomManager.GetRoom._livingEnemies.Count - 1].transform.position;
        GameObject obj = Instantiate(soulBoy, pos, Quaternion.identity);
        _roomManager.SetNecessaryInteraction();

        yield return new WaitForSeconds(0.5f);

        Instantiate(_voices[4].gameObject, _playerStats.transform.position, Quaternion.identity);

        yield return new WaitForSeconds(4f);
        
        pos = obj.transform.position + new Vector3(obj.transform.position.x > 0 ? -2 : 2, obj.transform.position.y < -2 ? 1 : -1, 0);
        Instantiate(soulObject, pos, Quaternion.identity);
    }
}