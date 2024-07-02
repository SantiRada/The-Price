using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ActionForControlPlayer;

public class InteractiveWeapon : MonoBehaviour {

    public WeaponSystem weapon;

    [Header("UI Content")]
    public GameObject objNewWeapon;
    public Image imgWeapon;
    public TextMeshProUGUI nameWeapon;
    public TextMeshProUGUI descWeapon;
    public TextMeshProUGUI dataWeapon;
    [Space]
    public Image[] currentImgWeapon;
    public TextMeshProUGUI[] currentNameWeapon;
    public TextMeshProUGUI[] currentDescWeapon;
    public TextMeshProUGUI[] currentDataWeapon;
    private CanvasGroup _canvas;

    [Header("Movement")]
    public GameObject[] positions;
    private int _index = 0;
    private bool _inSelect = false;

    [Header("Private Content")]
    private WeaponPool _weaponPool;
    private PlayerStats _playerStats;

    private void Awake()
    {
        _canvas = GetComponent<CanvasGroup>();
        _playerStats = FindAnyObjectByType<PlayerStats>();
    }
    private void OnEnable() { RandomPool(); }
    private void Start() { _weaponPool = FindAnyObjectByType<WeaponPool>(); }
    // --------------------------- //
    private void Update()
    {
        if(_inSelect)
        {
            float v = Input.GetAxisRaw("Vertical");

            if(v != 0) MoveSlot(v);

            if (Input.GetButtonDown("Fire1") || PlayerActionStates.IsUse || PlayerActionStates.IsDashing) Select();
        }

    }
    public void RandomPool() { weapon = _weaponPool.RandomPool(); }
    public void OpenWindow()
    {
        _canvas.alpha = 1;
        _canvas.interactable = true;
        _inSelect = true;

        // NEW CONTENT
        imgWeapon.sprite = weapon.spr;
        nameWeapon.text = LanguageManager.GetValue("Menu", weapon.nameWeapon);
        descWeapon.text = LanguageManager.GetValue("Menu", weapon.descWeapon);
        dataWeapon.text = weapon.damageWeapon + " > " + weapon.damageWeapon +  " > <color=yellow>" + weapon.damageFinalHit + "</color>";

        // CURRENT CONTENT
        for(int i = 0; i < currentImgWeapon.Length; i++)
        {
            currentImgWeapon[i].sprite = _playerStats.weapons[i].spr;
            currentNameWeapon[i].text = LanguageManager.GetValue("Menu", _playerStats.weapons[i].nameWeapon);
            currentDescWeapon[i].text = LanguageManager.GetValue("Menu", _playerStats.weapons[i].descWeapon);
            currentDataWeapon[i].text = _playerStats.weapons[i].damageWeapon + " > " + _playerStats.weapons[i].damageWeapon + " > <color=yellow>" + _playerStats.weapons[i].damageFinalHit + "</color>";
        }
    }
    private void MoveSlot(float dir)
    {
        #region MovementBasic
        if (dir > 0) _index++;
        else if(dir < 0) _index--;

        if(_index >= positions.Length) _index = 0;
        else if (_index < 0) _index = (positions.Length - 1);
        #endregion

        objNewWeapon.transform.position = positions[_index].transform.position;
    }
    private void Select() { _playerStats.SetWeapon(_index, weapon); }
}
// ES PROBLEMÁTICO ------- NO TOMA EN CUENTA QUE PASA CUANDO CAMBIAS UN ARMA ------ DEBERÍA SOLTARLA AL SUELO PARA REPETIR EL SISTEMA