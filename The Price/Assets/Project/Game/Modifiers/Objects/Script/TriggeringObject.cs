using System.Collections.Generic;
using UnityEngine;

public class TriggeringObject : MonoBehaviour {

    [Header("Private Content")]
    private List<Object> _objects;
    private PlayerStats _playerStats;

    private void OnEnable()
    {
        _playerStats = GetComponent<PlayerStats>();
        GetObjects();
    }
    public void GetObjects()
    {
        _objects = _playerStats.objects;
        foreach (Object obj in _objects)
        {
            if (obj.trigger == TypeTrigger.perfectRoom) RoomManager.perfectRoom += PerfectRoomTrigger;
        }
    }
    private void PerfectRoomTrigger()
    {
        Debug.Log("Llamado de Sala Perfecta");
        foreach(Object obj in _objects)
        {
            if (obj.trigger == TypeTrigger.perfectRoom) obj.Use();
        }
    }
}