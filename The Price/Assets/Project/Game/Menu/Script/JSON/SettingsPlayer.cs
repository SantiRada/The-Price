using UnityEngine;

[System.Serializable]
public class SettingsPlayer
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
    // --- CONTROL --- //
    public string controlUse;
    public string controlAttack;
    public string controlDash;
    public string controlSkillOne;
    public string controlSkillTwo;
    public string controlStats;
    public string controlPause;
    public string controlStaticAim;

    public string gamepadUse;
    public string gamepadAttack;
    public string gamepadDash;
    public string gamepadSkillOne;
    public string gamepadSkillTwo;
    public string gamepadStats;
    public string gamepadPause;
    public string gamepadStaticAim;
}