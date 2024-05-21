using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// indexWindow / Menu = 0, Options = 1, SelectedPlayer = 2
// indexSection / Gameplay = 0, Screen = 1, Audio = 2, Controls = 3, Accesibility = 4
public class MenuController : MonoBehaviour {

    [Header("General Data")]
    [SerializeField] private GameObject[] _window;
    [SerializeField] private List<Selectable> _firstSelectable;
    [Tooltip("La sección en la cuál estamos (Menu, Opciones, SelectPlayer)")] private int _indexWindow = 0;

    [Header("Content Settings")]
    [SerializeField] private Sprite selectedItem;
    [SerializeField] private Color unselectedText;
    [SerializeField] private Color selectedText;
    [Space]
    [SerializeField] private GameObject[] _sections;
    [SerializeField] private Image[] _optionsIMG;
    [SerializeField] private Selectable[] _optionsSelectable;
    [SerializeField] private int[] _countPerSection;
    [SerializeField] private float _delayMovement;
    [Tooltip("La sección en la cuál estamos (Juego, Pantalla, Etc)")] private int _indexSection = 0;
    [Tooltip("La posición de Selectable seleccionado")] private int _posSection = 0;
    public static bool inPause = false;
    private bool _inSettings = false;
    private bool canMove = false;

    [Header("Confirmation Window")]
    [SerializeField] private GameObject _confirmWindow;
    [SerializeField] private TextMeshProUGUI _textConfirm;
    private int typeConfirm = 0;

    private void Update()
    {
        if (LoadingScreen.InLoading || !inPause) return;

        if (Input.GetButtonDown("Fire2") || Input.GetKeyDown(KeyCode.Escape))
        {
            if (_indexWindow != 0) CloseWindow();
            else WindowConfirmation(1);
        }

        if (!_inSettings) return;

        float y = Input.GetAxis("Vertical");
        if (y != 0 && canMove) StartCoroutine(MoveInSection(y));
    }
    protected void ChangeStateMenu(bool state)
    {
        inPause = state;

        _window[0].SetActive(state);
        for (int i = 1; i< _window.Length; i++)
        {
            _window[i].SetActive(false);
        }

        if(inPause) _firstSelectable[0].Select();
    }
    // ------- VENTANA DE CONFIGURACIÓN ------- //
    public void OpenConfig()
    {
        canMove = true;
        _inSettings = true;
        _window[_indexWindow].SetActive(false);

        _indexWindow = 1;
        _window[_indexWindow].SetActive(true);

        _posSection = 0;
        _indexSection = 0;
        ModifyOption(true);
    }
    private void CloseWindow()
    {
        _inSettings = false;
        _window[_indexWindow].SetActive(false);

        _indexWindow = 0;
        _window[_indexWindow].SetActive(true);
    }
    // ------- SECCIONES DE CONFIGURACIÓN ----- //
    public void MoveToSection(int id)
    {
        ModifyOption(false);
        _sections[_indexSection].SetActive(false);

        _indexSection = id;
        _sections[_indexSection].SetActive(true);

        _posSection = 0;
        _firstSelectable[3 + _indexSection].Select();
        ModifyOption(true);
    }
    private IEnumerator MoveInSection(float id)
    {
        canMove = false;
        ModifyOption(false);

        if (id > 0) _posSection--;
        else if (id < 0) _posSection++;

        if (_posSection < 0) _posSection = CalculateCountPerThisSection();
        if (_posSection > _countPerSection[_indexSection]) _posSection = 0;

        _optionsSelectable[_posSection].Select();

        ModifyOption(true);
        yield return new WaitForSeconds(_delayMovement);
        canMove = true;
    }
    private void ModifyOption(bool state)
    {
        int pos = _posSection;

        if (_indexSection > 0) pos += CalculateCountPerThisSection();

        if (!state)
        {
            _optionsIMG[pos].GetComponentInChildren<TextMeshProUGUI>().color = unselectedText;
            _optionsIMG[pos].color = new Color(1, 1, 1, 0);
            _optionsIMG[pos].sprite = null;
        }
        else
        {
            _optionsIMG[pos].GetComponentInChildren<TextMeshProUGUI>().color = selectedText;
            _optionsIMG[pos].color = new Color(1, 1, 1, 1);
            _optionsIMG[pos].sprite = selectedItem;
        }
    }
    private int CalculateCountPerThisSection()
    {
        int pos = 0;
        for (int i = 0; i < _indexSection; i++)
        {
            pos += _countPerSection[i];
        }

        return pos;
    }
    // ------- VENTANA DE CONFIRMACIÓN -------- //
    public void WindowConfirmation(int id)
    {
        typeConfirm = id;

        if(id == 0)
        {
            // CONFIRMA VOLVER AL MENU
            _confirmWindow.SetActive(true);
            _textConfirm.text = LanguageManager.GetValue(70);
        }
        else
        {
            // CONFIRMA CERRAR EL JUEGO
            _confirmWindow.SetActive(true);
            _textConfirm.text = LanguageManager.GetValue(80);
        }

        _confirmWindow.GetComponentInChildren<Selectable>().Select();
    }
    public void ConfirmClose()
    {
        if (typeConfirm == 0) BackToMenu();
        else QuitGame();
    }
    public void CloseConfirmation()
    {
        _confirmWindow.SetActive(false);
        _firstSelectable[0].Select();
    }
    // ------- VALORES POS-CONFIRMACIÓN ------- //
    private void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
    private void QuitGame()
    {
        Debug.Log("Se cerró el juego");
        Application.Quit();
    }
}
