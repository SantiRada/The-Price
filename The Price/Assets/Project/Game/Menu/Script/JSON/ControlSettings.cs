using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlSettings : MonoBehaviour {

    [SerializeField] private string _dataPlayer;
    public SettingsPlayer settings = new SettingsPlayer();

    [Header("Data Gameplay")]
    [SerializeField] private Toggle _shakeScreen;
    [SerializeField] private Toggle _vibration;
    [SerializeField] private Toggle _aimAssistant;
    [SerializeField] private Slider _helpAssistant;
    [SerializeField] private Toggle _playerIndicators;
    [SerializeField] private Toggle _timerToRun;
    [SerializeField] private Toggle _DamageNumbers;
    [SerializeField] private Toggle _healthbarEnemy;
    [SerializeField] private TMP_Dropdown _language;

    [Header("Data Screen")]
    [SerializeField] private TMP_Dropdown _resolution;
    [SerializeField] private TMP_Dropdown _screenMode;
    [SerializeField] private Toggle _lockCursor;
    [SerializeField] private Toggle _vSync;

    [Header("Data Audio")]
    [SerializeField] private Slider _generalSound;
    [SerializeField] private Slider _musicSound;
    [SerializeField] private Slider _effectSound;

    [Header("Data Controllers")]
    [HideInInspector] public string[] _controlUse = new string[2];
    [HideInInspector] public string[] _controlAttack = new string[2];
    [HideInInspector] public string[] _controlDash = new string[2];
    [HideInInspector] public string[] _controlSkillOne = new string[2];
    [HideInInspector] public string[] _controlSkillTwo = new string[2];
    [HideInInspector] public string[] _controlStats = new string[2];
    [HideInInspector] public string[] _controlPause = new string[2];
    [HideInInspector] public string[] _controlStaticAim = new string[2];

    private void Awake()
    {
        _dataPlayer = Application.dataPath + "/Settings.json";
    }
    private void Start()
    {
        Invoke("LoadData", 1f);
    }
    // ---- JSON ---- //
    public void SaveData()
    {
        SettingsPlayer newData = new SettingsPlayer()
        {
            shakeScreen = _shakeScreen.isOn,
            vibration = _vibration.isOn,
            aimAssistant = _aimAssistant.isOn,
            levelHelpAssistant = _helpAssistant.value,
            playerIndicators = _playerIndicators.isOn,
            timerToRun = _timerToRun.isOn,
            damageNumbers = _DamageNumbers.isOn,
            healthbarOfEnemys = _healthbarEnemy.isOn,
            language = _language.value,
            resolution = _resolution.value,
            screenMode = _screenMode.value,
            lockCursor = _lockCursor.isOn,
            vSync = _vSync.isOn,
            generalSound = _generalSound.value,
            musicSound = _musicSound.value,
            effectSound = _effectSound.value,
            controlUse = _controlUse[0],
            controlAttack = _controlAttack[0],
            controlDash = _controlDash[0],
            controlSkillOne = _controlSkillOne[0],
            controlSkillTwo = _controlSkillTwo[0],
            controlStats = _controlStats[0],
            controlPause = _controlPause[0],
            controlStaticAim = _controlStaticAim[0],
            gamepadUse = _controlUse[1],
            gamepadAttack = _controlAttack[1],
            gamepadDash = _controlDash[1],
            gamepadSkillOne = _controlSkillOne[1],
            gamepadSkillTwo = _controlSkillTwo[1],
            gamepadStats = _controlStats[1],
            gamepadPause = _controlPause[1],
            gamepadStaticAim = _controlStaticAim[1]

        };
        string stringJSON = JsonUtility.ToJson(newData);
        File.WriteAllText(_dataPlayer, stringJSON);
    }
    private void LoadData()
    {
        if (File.Exists(_dataPlayer))
        {
            string contain = File.ReadAllText(_dataPlayer);
            settings = JsonUtility.FromJson<SettingsPlayer>(contain);

            ChangedData(settings);
        }
    }
    private void ChangedData(SettingsPlayer settings)
    {
        // GENERAL
        _shakeScreen.isOn = settings.shakeScreen;
        _vibration.isOn = settings.vibration;
        _aimAssistant.isOn = settings.aimAssistant;
        _helpAssistant.value = settings.levelHelpAssistant;
        _playerIndicators.isOn = settings.playerIndicators;
        _timerToRun.isOn = settings.timerToRun;
        _DamageNumbers.isOn = settings.damageNumbers;
        _healthbarEnemy.isOn = settings.healthbarOfEnemys;
        _language.value = settings.language;
        // PANTALLA
        _resolution.value = settings.resolution;
        _screenMode.value = settings.screenMode;
        _lockCursor.isOn = settings.lockCursor;
        _vSync.isOn = settings.vSync;
        // SONIDO
        _generalSound.value = settings.generalSound;
        _musicSound.value = settings.musicSound;
        _effectSound.value = settings.effectSound;
        // CONTROLES
        _controlUse[0] = settings.controlUse;
        _controlAttack[0] = settings.controlAttack;
        _controlDash[0] = settings.controlDash;
        _controlSkillOne[0] = settings.controlSkillOne;
        _controlSkillTwo[0] = settings.controlSkillTwo;
        _controlStats[0] = settings.controlStats;
        _controlPause[0] = settings.controlPause;
        _controlStaticAim[0] = settings.controlStaticAim;

        _controlUse[1] = settings.gamepadUse;
        _controlAttack[1] = settings.gamepadAttack;
        _controlDash[1] = settings.gamepadDash;
        _controlSkillOne[1] = settings.gamepadSkillOne;
        _controlSkillTwo[1] = settings.gamepadSkillTwo;
        _controlStats[1] = settings.gamepadStats;
        _controlPause[1] = settings.gamepadPause;
        _controlStaticAim[1] = settings.gamepadStaticAim;
    }
}
