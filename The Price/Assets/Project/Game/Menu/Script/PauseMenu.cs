using UnityEngine;

public class PauseMenu : MenuController {

    private void Update()
    {
        if (!inPause) return;

        if(Input.GetButtonDown("Submit") || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape)) ChangeStateMenu(true);
    }
    // ---- BUTTONS ---- //
    public void ContinueGame()
    {
        inPause = false;
        ChangeStateMenu(false);
    }
    public static void SetPause(bool pause) { inPause = pause; }
}