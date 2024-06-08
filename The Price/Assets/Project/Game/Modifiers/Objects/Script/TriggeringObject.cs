using System.Collections.Generic;
using UnityEngine;

public class TriggeringObject : MonoBehaviour {

    private List<Object> _objects;
    private PlayerStats _playerStats;

    private void OnEnable()
    {
        _playerStats = GetComponent<PlayerStats>();
    }
    public void SetObjects(List<Object> obj)
    {
        _objects = obj;

        AssignPlayerStats();
        SetVerifyObjects();
    }
    private void SetVerifyObjects()
    {
        foreach(Object obj in _objects)
        {
            if (obj.trigger == TypeTrigger.perfectRoom) RoomManager.perfectRoom += obj.Use;
        }
    }
    public void SetUseNew()
    {
        if(_objects != null) { if(_objects.Count > 0) { for (int i = 0; i < _objects.Count; i++) { _objects[i].canGet = true; } } }
    }
    // ---- FUNCIÓN INTEGRA ---- //
    private void AssignPlayerStats()
    {
        foreach (var obj in _objects)
        {
            obj.playerStats = _playerStats;
        }
    }
}