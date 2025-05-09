using UnityEngine;

public class DoorsManager : MonoBehaviour {

    [Header("Content")]
    [SerializeField] private GameObject[] _doors;
    [SerializeField] private GameObject _posInScene;

    [Header("Private Data")]
    private SaveLoadManager _saveLoad;

    private void Awake() { _saveLoad = FindAnyObjectByType<SaveLoadManager>(); }
    private void Start() { Invoke("CreateDoors", 0.5f); }
    private void CreateDoors()
    {
        if(_saveLoad.GetWorldData().reasonSave == ReasonSave.closeGame)
        {
            int value = _saveLoad.GetWorldData().currentWorld;

            if (value >= _doors.Length) value = _doors.Length - 1;

            Instantiate(_doors[value], _posInScene.transform.position, Quaternion.identity);
        }
    }
}
