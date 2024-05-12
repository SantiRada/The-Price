using UnityEngine;

[System.Serializable]
public class SettingsDataPlayer
{
    public bool shakeScreen;
    public bool vibration;
    public bool aimAssistant;
    public float levelHelpAssistant;
    public bool playerIndicators;
    public bool timerToRun;
    public bool damageNumbers;
    public bool healthbarOfEnemys;
    public int language;
    // ---- SCREEN ---- //
    public int resolution;
    public int screenMode;
    public bool lockCursor;
    public bool vSync;
    // ---- AUDIO ---- //
    public float generalSound;
    public float musicSound;
    public float effectSound;
}