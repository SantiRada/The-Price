using System.Collections;
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
        };
        string stringJSON = JsonUtility.ToJson(newData);
        File.WriteAllText(_dataPlayer, stringJSON);

        Debug.Log("Archivo guardado");
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
        _shakeScreen.isOn = settings.shakeScreen;
        _vibration.isOn = settings.vibration;
        _aimAssistant.isOn = settings.aimAssistant;
        _helpAssistant.value = settings.levelHelpAssistant;
        _playerIndicators.isOn = settings.playerIndicators;
        _timerToRun.isOn = settings.timerToRun;
        _DamageNumbers.isOn = settings.damageNumbers;
        _healthbarEnemy.isOn = settings.healthbarOfEnemys;
        _language.value = settings.language;
        _resolution.value = settings.resolution;
        _screenMode.value = settings.screenMode;
        _lockCursor.isOn = settings.lockCursor;
        _vSync.isOn = settings.vSync;
        _generalSound.value = settings.generalSound;
        _musicSound.value = settings.musicSound;
        _effectSound.value = settings.effectSound;
    }
}
