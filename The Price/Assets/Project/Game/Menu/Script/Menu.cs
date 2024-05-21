using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MenuController {

    private void Start()
    {
        ChangeStateMenu(true);
    }
    public void OpenCredits()
    {
        Debug.Log("Abriendo los cr�ditos...");
        SceneManager.LoadScene("Credits");
    }
}
