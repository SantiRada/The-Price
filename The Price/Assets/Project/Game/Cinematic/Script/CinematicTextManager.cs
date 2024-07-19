using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static ActionForControlPlayer;

public class CinematicTextManager : MonoBehaviour {

    [Header("Texts")]
    public int index;
    public TextMeshProUGUI textInScene;

    [Header("UI Skip")]
    public GameObject sectorSkip;
    public Image wheelHold;

    [Header("Skip Manager")]
    public float timeToSkip;
    private float timeToHold;
    private bool showSkip;

    private void Start()
    {
        textInScene.text = LanguageManager.GetValue("Menu", index);
    }
    private void Update()
    {
        if (Input.GetButtonDown("Fire1")) { showSkip = true; }
        if (Input.GetButtonUp("Fire1")) { showSkip = false; }

        if (showSkip)
        {
            sectorSkip.gameObject.SetActive(true);
            wheelHold.fillAmount = (timeToSkip - timeToHold);

            timeToHold += Time.deltaTime;

            if(timeToHold >= timeToSkip) { AdvanceCinematic(); }
        }
        else
        {
            wheelHold.fillAmount = 1;
            sectorSkip.gameObject.SetActive(false);
            timeToHold = 0;
        }
    }
    // ---- EVENTO DEL ANIMATOR ---- //
    public void ChangeIndexText()
    {
        index++;
        textInScene.text = LanguageManager.GetValue("Menu", index);
    }
    public void AdvanceCinematic()
    {
        if (FindAnyObjectByType<DeadSystem>())
        {
            FindAnyObjectByType<DeadSystem>().DiePlayer();
        }
        else { SceneManager.LoadScene("Terrenal"); }
    }
}
