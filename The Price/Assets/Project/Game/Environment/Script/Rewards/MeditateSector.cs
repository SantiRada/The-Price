using System.Collections;
using UnityEngine;

public class MeditateSector : MonoBehaviour {

    [Header("Private Content")]
    private Vector3 _prevPosPlayer;
    private bool inMeditation = false;

    [Header("Private Callers")]
    private Interactive _interactive;
    private PlayerStats _playerStats;
    private RoomManager _roomManager;
    private MeditationRoom _meditation;
    private Collider2D _col2D;

    private void Awake()
    {
        _col2D = GetComponent<Collider2D>();
        _interactive = GetComponent<Interactive>();
        _playerStats = FindAnyObjectByType<PlayerStats>();
        _roomManager = FindAnyObjectByType<RoomManager>();
        _meditation = FindAnyObjectByType<MeditationRoom>();
    }
    private void Update() { if (_interactive.inSelect && !inMeditation) StartCoroutine("Meditate"); }
    private IEnumerator Meditate()
    {
        inMeditation = true;

        yield return new WaitForSeconds(0.1f);

        if (_playerStats.GetterStats(1, false) >= 12)
        {
            // TENES LA CONCENTRACIÓN AL MÁXIMO
            float time = 20;

            // CAMBIAR CONCENTRACION
            _playerStats.SetValue(1, -12, false);

            _prevPosPlayer = _playerStats.transform.position;

            _roomManager.loadingSector.SetBool("inLoading", true);

            yield return new WaitForSeconds(0.5f);

            _playerStats.transform.position = _meditation.posPlayer.transform.position;
            CameraMovement.CallCamera(_meditation.transform.position, (time + 5f));
            _meditation.StartMeditation(time, this);

            yield return new WaitForSeconds(0.65f);

            _roomManager.loadingSector.SetBool("inLoading", false);

            _col2D.enabled = false;
        }
        else
        {
            // NO PUEDES MEDITAR
            _interactive.descContent = "44";
            _interactive.OpenWindow();
        }
    }
    public IEnumerator EndMeditate()
    {
        _roomManager.loadingSector.SetBool("inLoading", true);

        yield return new WaitForSeconds(0.5f);

        _playerStats.transform.position = _prevPosPlayer;
        CameraMovement.CancelCallCamera();

        yield return new WaitForSeconds(0.5f);

        _roomManager.loadingSector.SetBool("inLoading", false);
    }
}
