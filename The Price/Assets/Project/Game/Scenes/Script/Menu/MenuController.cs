using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

    public int indexSector = 0;
    public int subIndexSector = 0;

    [Header("Index 1: Saved")]
    public TextMeshProUGUI[] optionSavedText;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) { PlayerPrefs.DeleteAll(); }
    }
    // ---- INDEX 0 ---- //
    public void StartGame(int pos)
    {
        if(PlayerPrefs.GetString("Position-" + pos, "") == "")
        {
            File.Delete(Application.persistentDataPath + "/World-" + pos.ToString() + ".json");
            File.Delete(Application.persistentDataPath + "/Player-" + pos.ToString() + ".json");
        }

        PlayerPrefs.SetString(("Position-" + pos), "Si");
        PlayerPrefs.SetInt("PositionGame", pos);

        SceneManager.LoadScene("Terrenal");
    }
    public void QuitGame() { Application.Quit(); }
    // ---- INDEX 1 ---- //
    public void VerifySaved()
    {
        for(int i = 0; i < 3; i++)
        {
            if(PlayerPrefs.GetString("Position-" + i, "") != "") { optionSavedText[i].text = LanguageManager.GetValue("Menu", 2); }
            else { optionSavedText[i].text = LanguageManager.GetValue("Menu", 3); }
        }
    }
}
