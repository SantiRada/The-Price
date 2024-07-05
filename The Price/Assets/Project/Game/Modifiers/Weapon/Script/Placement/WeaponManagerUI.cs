using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static ActionForControlPlayer;

public class WeaponManagerUI : MonoBehaviour {

    [Header("Content Weapon")]
    public List<WeaponSystem> weapons = new List<WeaponSystem>();
    [Space]
    [Tooltip("El arma exacta que aparecerá en pantalla")] public WeaponSystem weapon;
    public InteractiveWeapon interactiveObj;

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
    [Range(0, 0.3f), Tooltip("Tiempo muerto desde la aparición hasta que puedas moverte")] public float deadTime;
    [Range(0, 0.3f), Tooltip("Delay entre movimientos")] public float timeBetweenMove;
    private int _index = 0;
    private bool _inSelect = false;
    private bool _canMove = false;

    [Header("Private Content")]
    private PlayerStats _playerStats;

    private void Awake()
    {
        _canvas = GetComponent<CanvasGroup>();
        _playerStats = FindAnyObjectByType<PlayerStats>();
    }
    private void Start()
    {
        ResetValues();
        RoomManager.advanceRoom += ResetWeapon;
    }
    private void Update()
    {
        if(_inSelect)
        {
            if (_canMove)
            {
                float v = Input.GetAxisRaw("Vertical");

                if (v != 0) StartCoroutine(MoveSlot(v));
            }

            if (Input.GetButtonDown("Fire1") || PlayerActionStates.IsUse || PlayerActionStates.IsDashing) Select();
        }

    }
    public WeaponSystem RandomPool()
    {
        if(weapon == null)
        {
            int rnd;
            bool canAdvance;
            do
            {
                canAdvance = true;
                rnd = Random.Range(0, weapons.Count);

                for(int i = 0; i < _playerStats.weapons.Count; i++)
                {
                    if (weapons[rnd].weaponID == _playerStats.weapons[i].weaponID)
                    {
                        canAdvance = false;
                        break;
                    }
                }

            } while (!canAdvance);

            weapon = weapons[rnd];
        }

        return weapon;
    }
    public IEnumerator OpenWindow()
    {
        _canvas.alpha = 1;
        _canvas.interactable = true;

        Pause.StateChange = State.Interface;

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

        yield return new WaitForSeconds(deadTime);

        _canMove = true;
        _inSelect = true;
    }
    private IEnumerator MoveSlot(float dir)
    {
        _canMove = false;

        #region MovementBasic
        if (dir > 0) _index--;
        else if(dir < 0) _index++;

        if(_index >= positions.Length) _index = 0;
        else if (_index < 0) _index = (positions.Length - 1);
        #endregion

        objNewWeapon.transform.position = positions[_index].transform.position;

        yield return new WaitForSeconds(timeBetweenMove);

        _canMove = true;
    }
    private void Select()
    {
        int prevWeapon = _playerStats.SetWeapon(_index, weapon);
        
        for(int i = 0; i < weapons.Count; i++) { if (weapons[i].weaponID == prevWeapon) { weapon = weapons[i]; break; } }

        Instantiate(interactiveObj.gameObject, _playerStats.transform.position, Quaternion.identity);

        ResetValues();

        Pause.StateChange = State.Game;
    }
    private void ResetValues()
    {
        _index = 0;
        _canvas.alpha = 0;
        _canvas.interactable = false;
        _inSelect = false;

        objNewWeapon.transform.position = positions[0].transform.position;
    }
    private void ResetWeapon() { weapon = null; }
}