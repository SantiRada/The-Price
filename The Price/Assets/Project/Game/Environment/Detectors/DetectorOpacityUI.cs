using System.Collections.Generic;
using UnityEngine;

public class DetectorOpacityUI : MonoBehaviour {

    [Header("Data Elements")]
    [SerializeField] private string[] _detectableTypes;
    [SerializeField] private float _sizeHUD = 4.35f;
    private float distance;

    private bool _detectElement = false;

    [Header("Data UI")]
    private List<DataPlayerHUD> _playerHUD = new List<DataPlayerHUD>();

    private void Start()
    {
        InputManager._InitializateValues += InitialValues;
    }
    private void InitialValues()
    {
        _playerHUD.AddRange(FindObjectsByType<DataPlayerHUD>(FindObjectsSortMode.None));
    }
    private void Update()
    {
        if (!_detectElement) return;

        if (distance <= (_sizeHUD * _playerHUD.Count))
        {
            for (int j = 0; j < _playerHUD.Count; j++)
            {
                _playerHUD[j].DecreaseOpacity();
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        for(int i = 0; i < _detectableTypes.Length; i++)
        {
            if (collision.tag == _detectableTypes[i])
            {
                // REVISAR EL PRIMER CONTACTO
                distance = CameraMovement.GetPosition().x - 7.5f;
                distance = collision.transform.position.x - distance;

                _detectElement = true;
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        for (int i = 0; i < _detectableTypes.Length; i++)
        {
            if (collision.tag == _detectableTypes[i])
            {
                // REVISAR EL CONTACTO CONSTANTE
                distance = CameraMovement.GetPosition().x - 7.5f;
                distance = collision.transform.position.x - distance;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        for (int i = 0; i < _detectableTypes.Length; i++)
        {
            if (collision.tag == _detectableTypes[i])
            {
                for (int j = 0; j < _playerHUD.Count; j++)
                {
                    _playerHUD[j].IncreaseOpacity();
                }
            }
        }
        _detectElement = false;
    }
}
