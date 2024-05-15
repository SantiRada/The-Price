using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DataPlayerStats : MonoBehaviour {

    [Header("Stats")]
    private bool _inStats { get; set; }
    [Space]
    public TextMeshProUGUI _textHealth;
    public TextMeshProUGUI _textEnergy;
    public TextMeshProUGUI _textSpeed;
    public TextMeshProUGUI _textDamage;
    public TextMeshProUGUI _textDamageSkills;
    public TextMeshProUGUI _textCritical;
    [Space]
    public TextMeshProUGUI _textTitleData;
    public TextMeshProUGUI _textDescriptionData;

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
    // SETTERS & GETTERS //
    public bool InStats
    {
        get { return _inStats; }
        set { _inStats = value; }
    }
}
