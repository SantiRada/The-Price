using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TriggeringObject : MonoBehaviour {

    [Header("Data")]
    private List<Object> _objects;
    private PlayerStats _playerStats;

    // EVENTS
    public event Action fourKills;
    public event Action kills;

    [Header("Subdata")]
    private List<CanceledObject> _objectsPerTime;

    private void OnEnable()
    {
        _playerStats = GetComponent<PlayerStats>();
    }
    private void Update()
    {
        // VERIFICA EL TIMER DE LOS OBJETOS QUE FUNCIONAN "PERTIME"
        if(_objectsPerTime != null)
        {
            if(_objectsPerTime.Count != 0)
            {
                for (int i = 0; i < _objectsPerTime.Count; i++) { if (_objectsPerTime[i].isActive) _objectsPerTime[i].Timer(); }
            }
        }

        // VERIFICA EL ENFRIAMIENTO DE LOS OBJETOS QUE LO TIENEN
        if(_objects != null)
        {
            if(_objects.Count != 0)
            {
                for (int i = 0; i < _objects.Count; i++) { if (!_objects[i].canActive && _objects[i].hasCooling) { _objects[i].Cooling(); } }
            }
        }
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
            // TRIGGERS
            if (obj.trigger == TypeTrigger.kills) kills += obj.Use;
            if (obj.trigger == TypeTrigger.fourKills) fourKills += obj.Use;
            if (obj.trigger == TypeTrigger.perfectRoom) RoomManager.perfectRoom += obj.Use;

            // CANCELAMIENTOS
            if (obj.typeCanceled == TypeCanceled.receivedDamage) PlayerStats.takeDamage += obj.CancelContent;
            if (obj.typeCanceled == TypeCanceled.perRoom) RoomManager.finishRoom += obj.CancelContent;
            if (obj.typeCanceled == TypeCanceled.perTime) _objectsPerTime.Add(obj.GetComponent<CanceledObject>()); // CREAR UNA SUBLISTA DE LOS ELEMENTOS "PERTIME"
        }
    }
    // ---- SETTERS && GETTERS ---- //
    public void AddKills()
    {
        _playerStats.countKillsInRoom++;
        
        // INVOCA UN EVENTO QUE MARCA QUE SE HIZO UN ASESINATO
        kills?.Invoke();

        // INVOCA UN EVENTO ESPECÍFICO DE QUE SE ALCANZARON MÚLTIPLOS DE 4 ASESINATOS
        if (_playerStats.countKillsInRoom % 4 == 0) fourKills?.Invoke();
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