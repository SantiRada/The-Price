using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    [Header("Info Movement")]
    [SerializeField] private Vector2 _distanceForMovement;
    [SerializeField] private float _delayMovement = 0.5f;

    [Header("Info Players")]
    private List<PlayerMovement> _players = new List<PlayerMovement>();

    private void Start()
    {
        InputManager._InitializateValues += InitialValues;
    }
    private void InitialValues()
    {
        PlayerMovement[] _playersArray = FindObjectsByType<PlayerMovement>(FindObjectsSortMode.None);

        _players.AddRange(_playersArray);

        RepositionCamera();
    }
    private void Update()
    {
        if (LoadingScreen.InLoading) return;

        RepositionCamera();
    }
    private void RepositionCamera()
    {
        switch (_players.Count)
        {
            case 1: transform.position = new Vector3(_players[0].transform.position.x, _players[0].transform.position.y, transform.position.z); break;
            case 2:
                Vector3 player1Pos = _players[0].transform.position;
                Vector3 player2Pos = _players[1].transform.position;

                Vector3 midpoint = (player1Pos + player2Pos) / 2f;
                midpoint.z = transform.position.z;

                transform.position = Vector3.Slerp(transform.position, midpoint, _delayMovement * Time.deltaTime);

                break;
        }
    }
}
