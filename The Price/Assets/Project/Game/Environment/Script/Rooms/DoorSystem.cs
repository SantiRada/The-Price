using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorSystem : MonoBehaviour {

    [Header("Data")]
    [Tooltip("A dónde me lleva esta puerta")] public Worlds whereItTakesMe;
    [Range(0f, 2f)] public float timerToLoadScene;
    public bool isActive = true;

    private RoomManager _roomManager;
    private LunarCycle _lunarCycle;

    private void Start()
    {
        _roomManager = FindAnyObjectByType<RoomManager>();
        _lunarCycle = FindAnyObjectByType<LunarCycle>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (isActive)
            {
                StartCoroutine("ChangeScene");
            }
            else { VoiceSystem.StartDialogue(40); }
        }
    }
    private IEnumerator ChangeScene()
    {
        _lunarCycle.isActive = false;
        _lunarCycle.StartCoroutine("AddRoom");

        _roomManager.loadingSector.SetBool("inLoading", true);
        Pause.SetPause(true);

        yield return new WaitForSeconds(timerToLoadScene);

        SceneManager.LoadScene(whereItTakesMe.ToString());
    }
}
