using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public GameObject arrowUp;
    public GameObject arrowDown;
    [Space]
    public GameObject objNewWeapon;
    public Image imgWeapon;
    public TextMeshProUGUI nameWeapon;
    public TextMeshProUGUI descWeapon;
    public TextMeshProUGUI dataWeapon;
    [Space]
    public GameObject[] currentWeapon;
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

            if (PlayerActionStates.IsUse) StartCoroutine("Select");
        }

    }
    public void SetWeaponPool(WeaponSystem weaponPrev) { weapon = weaponPrev; }
    public void RandomPool()
    {
        if(weapon == null)
        {
            print("Randomizando...");

            int rnd;
            bool canAdvance;
            do
            {
                canAdvance = true;
                rnd = Random.Range(0, weapons.Count);

                for(int i = 0; i < _playerStats.weapons.Count; i++)
                {
                    if (_playerStats.weapons[i] != null)
                    {
                        if (weapons[rnd].weaponID == _playerStats.weapons[i].weaponID)
                        {
                            canAdvance = false;
                            break;
                        }
                    }
                }

            } while (!canAdvance);

            weapon = weapons[rnd];
        }
    }
    public IEnumerator OpenWindow()
    {
        RandomPool();

        _canvas.alpha = 1;
        _canvas.interactable = true;

        Pause.StateChange = State.Interface;

        // NEW CONTENT
        imgWeapon.sprite = weapon.spr;
        nameWeapon.text = LanguageManager.GetValue("Menu", weapon.nameWeapon);
        descWeapon.text = LanguageManager.GetValue("Menu", weapon.descWeapon);

        if(weapon.damageWeapon != 0) dataWeapon.text = weapon.damageWeapon + " > " + weapon.damageWeapon +  " > <color=yellow>" + weapon.damageFinalHit + "</color>";
        else dataWeapon.text = LanguageManager.GetValue("Menu", 81);

        // CURRENT CONTENT
        for (int i = 0; i < currentImgWeapon.Length; i++)
        {
            if (_playerStats.weapons.Count > i)
            {
                if (_playerStats.weapons[i] != null)
                {
                    currentImgWeapon[i].sprite = _playerStats.weapons[i].spr;
                    currentNameWeapon[i].text = LanguageManager.GetValue("Menu", _playerStats.weapons[i].nameWeapon);
                    currentDescWeapon[i].text = LanguageManager.GetValue("Menu", _playerStats.weapons[i].descWeapon);
                    if (_playerStats.weapons[i].damageWeapon != 0)
                        currentDataWeapon[i].text = _playerStats.weapons[i].damageWeapon + " > " + _playerStats.weapons[i].damageWeapon + " > <color=yellow>" + _playerStats.weapons[i].damageFinalHit + "</color>";
                    else
                        currentDataWeapon[i].text = LanguageManager.GetValue("Menu", 81);
                }
                else
                {
                    currentNameWeapon[i].text = "";
                    currentDescWeapon[i].text = "";
                    currentDataWeapon[i].text = "";
                }
            }
            else
            {
                currentNameWeapon[i].text = "";
                currentDescWeapon[i].text = "";
                currentDataWeapon[i].text = "";
            }
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

        VerifyArrows();

        yield return new WaitForSeconds(timeBetweenMove);

        _canMove = true;
    }
    private IEnumerator Select()
    {
        _inSelect = false;
        _canvas.interactable = false;
        yield return new WaitForSeconds(0.25f);

        do
        {
            objNewWeapon.transform.position = Vector3.Lerp(objNewWeapon.transform.position, currentWeapon[_index].transform.position, 1f * Time.deltaTime);
            yield return new WaitForSeconds(0.005f);
        } while (Vector3.Distance(objNewWeapon.transform.position, currentDataWeapon[_index].transform.position) <= 10);

        objNewWeapon.transform.position = currentWeapon[_index].transform.position;

        int prevWeapon = _playerStats.SetWeapon(_index, weapon);

        // INSTACIAR EL ARMA QUE SE DESCARTA EN LA ESCENA SI ES ESE EL CASO
        if (prevWeapon != -1)
        {
            WeaponSystem dataWeapon = null;
            for (int i = 0; i < weapons.Count; i++)
            {
                if (weapons[i].weaponID == prevWeapon)
                {
                    dataWeapon = weapons[i];
                    break;
                }
            }

            InteractiveWeapon interactive = Instantiate(interactiveObj.gameObject, _playerStats.transform.position, Quaternion.Euler(0, 0, 0)).GetComponent<InteractiveWeapon>();
            interactive.SetWeapon(dataWeapon);
        }

        ResetValues();

        Pause.StateChange = State.Game;
    }
    private void ResetValues()
    {
        _index = 0;
        _canvas.alpha = 0;
        _canvas.interactable = false;
        _inSelect = false;
        weapon = null;

        objNewWeapon.transform.position = positions[0].transform.position;

        VerifyArrows();
    }
    private void ResetWeapon() { weapon = null; }
    private void VerifyArrows()
    {
        if (_index == 0)
        {
            arrowUp.SetActive(false);
            arrowDown.SetActive(true);
        }
        if (_index == 1)
        {
            arrowUp.SetActive(true);
            arrowDown.SetActive(true);
        }
        if (_index == 2)
        {
            arrowUp.SetActive(true);
            arrowDown.SetActive(false);
        }
    }
}
