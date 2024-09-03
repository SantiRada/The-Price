using System.Collections;
using UnityEngine;

public abstract class Tutorial : MonoBehaviour {

    public bool canCreateChest;

    protected SaveLoadManager _saveLoad;
    protected RoomManager _roomManager;
    protected PlayerStats _playerStats;

    private void OnEnable()
    {
        _playerStats = FindAnyObjectByType<PlayerStats>();
        _roomManager = FindAnyObjectByType<RoomManager>();
        _saveLoad = FindAnyObjectByType<SaveLoadManager>();
    }
    public IEnumerator VerifyRoom()
    {
        yield return new WaitForSeconds(1f);

        if (_saveLoad.GetWorldData() != null)
        {
            if (!_saveLoad.GetWorldData().passedTutorial)
            {
                if (_roomManager._countRoomsComplete == 0) { StartCoroutine("ChangesInZeroRoom"); }
                if (_roomManager._countRoomsComplete == 1) { StartCoroutine("ChangesInFirstRoom"); }
                if (_roomManager._countRoomsComplete == 2) { StartCoroutine("ChangesInSecondRoom"); }
                if (_roomManager._countRoomsComplete == 3) { StartCoroutine("ChangesInThirdRoom"); }
                if (_roomManager._countRoomsComplete == 4) { StartCoroutine("ChangesInFourthRoom"); }
            }
        }
    }
    public abstract void CallMadeInteraction();
    protected abstract IEnumerator ChangesInZeroRoom();
    protected abstract IEnumerator ChangesInFirstRoom();
    protected abstract IEnumerator ChangesInSecondRoom();
    protected abstract IEnumerator ChangesInThirdRoom();
    protected abstract IEnumerator ChangesInFourthRoom();
}