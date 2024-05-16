using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    [Header("Info Movement")]
    [SerializeField] private Vector2 _distanceForMovement;
    [SerializeField] private float _delayMovement = 0.5f;
    [SerializeField] private Vector2 minDistance, maxDistance;

    [Header("Info Players")]
    private List<PlayerMovement> _players = new List<PlayerMovement>();
    private static Vector3 _positionCam;

    private void Start()
    {
        InputManager._InitializateValues += InitialValues;
    }
    private void InitialValues()
    {
        PlayerMovement[] _playersArray = FindObjectsByType<PlayerMovement>(FindObjectsSortMode.None);

        _players.AddRange(_playersArray);

        if(_players.Count > 0) RepositionCamera();
    }
    private void Update()
    {
        if (LoadingScreen.InLoading) return;

        RepositionCamera();
    }
    private void RepositionCamera()
    {
        Vector3 ultimatePosition = transform.position;
        if(_players.Count == 1)
        {
            ultimatePosition = new Vector3(_players[0].transform.position.x, _players[0].transform.position.y, transform.position.z);
        }
        else
        {
            for (int i = 0; i < _players.Count; i++)
            {
                ultimatePosition += _players[i].transform.position;
            }
            ultimatePosition /= 2;
            ultimatePosition.z = transform.position.z;
        }

        ultimatePosition = new Vector3(Mathf.Clamp(ultimatePosition.x, minDistance.x, maxDistance.x),Mathf.Clamp(ultimatePosition.y, minDistance.y, maxDistance.y), -10);
        transform.position = Vector3.Slerp(transform.position, ultimatePosition, _delayMovement * Time.deltaTime);
        _positionCam = transform.position;
    }
    // ---- SETTERS Y GETTERS ---- //
    public void SetDistances(Vector2 min, Vector2 max)
    {
        minDistance = min;
        maxDistance = max;
    }
    public static Vector3 GetPosition()
    {
        return _positionCam;
    }
}
