using UnityEngine;
using UnityEngine.UI;

public class DataPlayerHUD : MonoBehaviour {

    [Header("Data Opacity")]
    [SerializeField] private float _delayColor;
    private bool _canChangeColor = false;
    private Image[] _contentHUD;
    private Color _base, _new;

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

        _contentHUD = FindObjectsByType<Image>(FindObjectsSortMode.None);
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
    private void Update()
    {
        if (!_canChangeColor) return;

        for (int i = 0; i < _contentHUD.Length; i++)
        {
            _base = _contentHUD[i].color;
            _contentHUD[i].color = Color.Lerp(_base, _new, _delayColor * Time.deltaTime);
        }
    }
    public void DecreaseOpacity()
    {
        _new = new Color(1, 1, 1, 0.1f);
        _canChangeColor = true;
    }
    public void IncreaseOpacity()
    {
        _new = new Color(1, 1, 1, 1f);
        _canChangeColor = true;
    }
}
