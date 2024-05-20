using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Menu = 0, Options = 1, Gameplay = 2, Screen = 3, Audio = 4, Controls = 5, Accesibility = 6, SelectedPlayer = 7
public class MenuUI : MonoBehaviour {

    private Animator anim;
    private int _currentWindow = 0;

    [SerializeField] private Selectable[] _selectableForSector;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    private void Start()
    {
        
    }
    private void Update()
    {
        if (LoadingScreen.InLoading) return;

        // FUNCTION BACK ----------------------------- >>>>>>>>>>
        if (Input.GetButtonDown("Fire2"))
        {
            int window = 0;
            if(_currentWindow == 1 || _currentWindow == 7 || _currentWindow == 0)
            {
                if (_currentWindow == 0)
                {
                    QuitGame();
                    return;
                }
                else { window = 0; }
            }
            else { window = 1; }

            SetDirection(window);
        }
    }
    public void SetDirection(int window)
    {
        _currentWindow = window;

        anim.SetInteger("Position", _currentWindow);

        // Seleccionar el primer elemento de la sección en la que se acaba de entrar
        _selectableForSector[_currentWindow].Select();
    }
    public void StartCredits() { SceneManager.LoadScene("Credits"); }
    public void QuitGame()
    {
        Debug.Log("Quit Game...");
        Application.Quit();
    }
}
