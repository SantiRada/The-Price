using UnityEngine;

public class MeditateSector : MonoBehaviour {

    [Header("Data Meditation")]
    public float multiplierSanity;

    private Interactive _interactive;
    private PlayerStats _player;

    private void Awake()
    {
        _interactive = GetComponent<Interactive>();
        _player = FindAnyObjectByType<PlayerStats>();
    }
    private void Update() { if (_interactive.inSelect) Meditate(); }
    private void Meditate()
    {
        if(_player.GetterStats(1, false) >= _player.GetterStats(1, true))
        {
            // TENES LA CONCENTRACIÓN AL MÁXIMO
            float time = _player.GetterStats(10, true) * multiplierSanity;

            Debug.Log("Te fuiste a meditar por " + time.ToString() + " segundos");
        }
        else
        {
            // NO PUEDES MEDITAR
            _interactive.descContent = "44";
            _interactive.OpenWindow();
        }
    }
}
