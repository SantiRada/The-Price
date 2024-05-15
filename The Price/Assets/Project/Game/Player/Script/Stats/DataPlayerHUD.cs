using UnityEngine;
using UnityEngine.UI;

public class DataPlayerHUD : MonoBehaviour {

    [Header("HUD")]
    public Image healthBar;
    public Image energyBar;
    public Image[] skillsSpr;

    [Header("Inputs")]
    [SerializeField] private Image[] _inputs;

    private InputManager _inputManager;

    private void Awake()
    {
        _inputManager = FindAnyObjectByType<InputManager>();
    }
    private void OnEnable()
    {
        InputManager._InitializateValues += InitialValues;
    }
    private void InitialValues()
    {
        for (int i = 0; i < _inputs.Length; i++)
        {
            string[] data = _inputs[i].name.Split('[');
            string[] subdata = data[1].Split("]");

            string _value = subdata[0];
            _inputs[i].sprite = _inputManager.GetInput(_inputs[i].tag, _value);
        }
    }
}
