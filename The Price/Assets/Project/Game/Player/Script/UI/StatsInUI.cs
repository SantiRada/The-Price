using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ActionForControlPlayer;

public enum TypeUI { menu = 0, stats = 1, skills = 2, objects = 3, history = 4 }
public class StatsInUI : MonoBehaviour {

    [Header("Content UI")]
    public GameObject dieUI;
    public GameObject statsWindow;
    public CanvasGroup[] sectioners;
    public GameObject[] guides;

    [Header("Content Menu UI")]
    public GameObject sectorMenu;
    public Image[] menuOptions;
    public Sprite selectSprite, unselectSprite;

    [Header("Content Stats UI")]
    public TextMeshProUGUI[] textStats;
    public GameObject[] parentWeapon;
    public Image[] weapons;

    [Header("Content Skills UI")]
    public Image[] skillOptions;
    public Image selectorSkill;
    public Sprite[] loadType;
    [Space]
    public GameObject sectorSkills;
    public TextMeshProUGUI[] _nameSkills;
    public TextMeshProUGUI[] _descSkills;
    public TextMeshProUGUI[] _loaderSkills;
    public Image[] _loaderSkillsIcon;
    [Space]
    public Image _iconSkill;
    public TextMeshProUGUI _titleSkillSelect;
    public TextMeshProUGUI _descSkillSelect;
    public TextMeshProUGUI _dataSkillSelect;

    [Header("Content Objects UI")]
    public GameObject sectorObject;
    public GameObject selectorObj;
    public Image[] _imgObject;
    public TextMeshProUGUI _titleObject;
    public TextMeshProUGUI _descObject;
    public TextMeshProUGUI _dataObject;
    public TextMeshProUGUI _temporalityObject;

    [Header("Private Content")]
    private PlayerStats _player;
    private HUD _hud;
    private CanvasGroup _group;

    [Header("Movement in UI")]
    private int _index = 0;
    private int _prevIndex = 0;
    private int _indexWindow = 0;
    private bool _canMove = false;

    private void Awake()
    {
        _hud = FindAnyObjectByType<HUD>();
        _group = GetComponent<CanvasGroup>();
        _player = FindAnyObjectByType<PlayerStats>();
    }
    private void Start() { InitialValues(); }
    private void InitialValues()
    {
        dieUI.gameObject.SetActive(false);
        SetChangeSkillsInUI(_player.skills);
        ChangeValueInUI(-1);

        #region OffElements
        for (int i = 0; i < _nameSkills.Length; i++)
        {
            _nameSkills[i].gameObject.SetActive(false);
            _descSkills[i].gameObject.SetActive(false);
            _loaderSkills[i].gameObject.SetActive(false);
            _loaderSkillsIcon[i].gameObject.SetActive(false);
        }
        statsWindow.SetActive(false);
        #endregion

        for(int i = 0; i < sectioners.Length; i++) { sectioners[i].alpha = 0.35f; }

        CloseUI();
    }
    private void Update()
    {
        if (LoadingScreen.inLoading || Pause.state != State.Pause || !_canMove) return;

        // MOVIMIENTO ENTRE VENTANAS --------- //
        if (PlayerActionStates.leftUI) StartCoroutine(MoveSector(-1));
        if (PlayerActionStates.rightUI) StartCoroutine(MoveSector(1));

        // FUNCIONES ESPECIALES -------------- //
        if (PlayerActionStates.otherFunctionUI && _indexWindow == 1) StartCoroutine("ChangePosWeapon");

        // MOVIMIENTO INTERNO DE LA UI ------- //
        if (Input.GetAxis("Vertical") != 0) StartCoroutine(Move(Input.GetAxis("Vertical")));
    }
    // ---- HUD ---- //
    public void SetHUD(int pos, float value, float valueMax)
    {
        if(pos == 0) _hud.SetHealthbar(value, valueMax);
        else _hud.SetConcentracion(value, valueMax);
    }
    public void SetWeaponInHUD(int index, Sprite spr)
    {
        weapons[index].sprite = spr;

        _hud.SetWeapon(index, spr);
    }
    // ---- WINDOW OBJECTS ---- //
    public void AddObjectInUI()
    {
        _imgObject[_player.objects.Count - 1].sprite = _player.objects[_player.objects.Count - 1].icon;
        _imgObject[_player.objects.Count - 1].color = Color.white;
    }
    private void SetDataObject()
    {
        if(_player.objects.Count <= _index)
        {
            _titleObject.text = "";
            _descObject.text = "";
            return;
        }

        if (_player.objects[_index] == null) return;

        _titleObject.text = LanguageManager.GetValue("Object", _player.objects[_index].itemName);
        _descObject.text = LanguageManager.GetValue("Object", _player.objects[_index].description);
    }
    // ---- WINDOW SKILLS ---- //
    public void SetChangeSkillsInUI(List<SkillManager> skills)
    {
        for (int i = 0; i < skills.Count; i++)
        {
            _nameSkills[i].gameObject.SetActive(true);
            _descSkills[i].gameObject.SetActive(true);
            _loaderSkills[i].gameObject.SetActive(true);
            _loaderSkillsIcon[i].gameObject.SetActive(true);

            _nameSkills[i].text = LanguageManager.GetValue("Skill", skills[i].skillName);
            _descSkills[i].text = LanguageManager.GetValue("Skill", skills[i].descName);
            _loaderSkills[i].text = skills[i].amountFuel.ToString();

            switch (skills[i].loadType)
            {
                case LoadTypeSkill.kills: _loaderSkillsIcon[i].sprite = loadType[0]; break;
                case LoadTypeSkill.concentration: _loaderSkillsIcon[i].sprite = loadType[1]; break;
                case LoadTypeSkill.damage: _loaderSkillsIcon[i].sprite = loadType[2]; break;
                case LoadTypeSkill.receiveDamage: _loaderSkillsIcon[i].sprite = loadType[3]; break;
            }

            _hud.SetSkills(i, skills[i].icon);
        }
    }
    private void SetDataSkill()
    {
        if (_player.skills.Count <= _index)
        {
            _titleSkillSelect.text = "";
            _descSkillSelect.text = "";
            return;
        }

        if (_player.skills[_index] == null) return;

        _titleSkillSelect.text = LanguageManager.GetValue("Skill", _player.skills[_index].skillName);
        _descSkillSelect.text = LanguageManager.GetValue("Skill", _player.skills[_index].descName);
    }
    // ---- WINDOW PAUSE ---- //
    public void ShowWindowedPause()
    {
        if (Pause.state == State.Interface)
        {
            Pause.StateChange = State.Game;
            CloseUI();
        }
        else
        {
            Pause.StateChange = State.Pause;
            OpenUI(TypeUI.menu);
        }
    }
    // ---- WINDOW STATS ---- //
    public void ShowWindowedStats()
    {
        Pause.StateChange = State.Pause;
        OpenUI(TypeUI.stats);
    }
    public void ChangeValueInUI(int type)
    {
        if (type == -1) { for (int i = 0; i < 11; i++) { ChangeStatsInUI(i, _player.GetterStats(i, false), _player.GetterStats(i, true)); } }
        else { ChangeStatsInUI(type, _player.GetterStats(type, false), _player.GetterStats(type, true)); }
    }
    private void ChangeStatsInUI(int i, float value, float valueMax)
    {
        if (i == 0 || i == 1 || i == 10)
            textStats[i].text = value.ToString() + "/" + valueMax.ToString();
        else if (i == 2 || i == 3 || i == 6 || i == 7 || i == 8 || i == 9)
            textStats[i].text = valueMax.ToString() + "%";
        else if (i == 4 || i == 5)
            textStats[i].text = valueMax.ToString();
    }
    private IEnumerator ChangePosWeapon()
    {
        PlayerActionStates.otherFunctionUI = false;
        Image[] content = new Image[3];
        WeaponSystem[] contentWeapon = new WeaponSystem[_player.weapons.Count];
        WeaponSystem[] contentWeaponScene = new WeaponSystem[_player.weaponInScene.Count];

        int newPos = 0;

        for (int i = 0; i < parentWeapon.Length; i++)
        {
            content[i] = weapons[i];
            if(_player.weapons.Count > i)
            {
                if (_player.weapons[i] != null) contentWeapon[i] = _player.weapons[i];
                else contentWeapon[i] = null;
            }
            if (_player.weaponInScene.Count > i)
            {
                if (_player.weaponInScene[i] != null) contentWeaponScene[i] = _player.weaponInScene[i];
                else contentWeaponScene[i] = null;
            }

            newPos = i + 1;
            if (newPos >= weapons.Length) newPos = 0;

            Vector3 newPosition = new Vector3(parentWeapon[newPos].transform.position.x, weapons[i].transform.position.y, 0);
            StartCoroutine(OffsetMove(weapons[i].gameObject, newPosition));
            yield return new WaitForSeconds(0.1f);
        }

        for (int i = 0; i < weapons.Length; i++)
        {
            newPos = i - 1;
            if (newPos < 0) newPos = weapons.Length - 1;

            // SETTEAR CAMBIOS EN STATS
            weapons[i] = content[newPos];

            // SETTEAR CAMBIOS EN PLAYER-STATS
            if (_player.weapons.Count > i) _player.weapons[i] = contentWeapon[newPos];
            if (_player.weaponInScene.Count > i) _player.weaponInScene[i] = contentWeaponScene[newPos];

            // SETTEAR CAMBIOS EN ACTION-FOR-CONTROL-PLAYER
            _player.UpdateWeaponInAction();

            // SETTEAR CAMBIOS EN EL HUD
            Sprite spr = _player.weapons[i] != null ? _player.weapons[i].spr : null;
            SetWeaponInHUD(i, spr);
        }

        // CAMBIAR LAS POSICIONES EN EL HUD
        // CAMBIAR SU POSICION EN LOS ARRAYS ORIGINALES - playerStats - 
    }
    private IEnumerator OffsetMove(GameObject obj, Vector3 newPos)
    {
        do
        {
            obj.transform.position = Vector3.Slerp(obj.transform.position, newPos, 0.2f * Time.deltaTime);
            yield return new WaitForSeconds(0.15f);
        } while (Vector3.Distance(obj.transform.position, newPos) < 5f);

        obj.transform.position = newPos;
    }
    // ---- MOVEMENT IN UI ---- //
    private void OpenUI(TypeUI typeUI)
    {
        _group.alpha = 1;
        _group.interactable = true;

        ResetData();
        sectioners[(int)typeUI].alpha = 1f;

        LoadSector((int)typeUI);

        _canMove = true;
    }
    private void LoadSector(int type, bool prevTypes = false)
    {
        if (prevTypes)
        {
            sectorMenu.SetActive(false);
            statsWindow.SetActive(false);
            sectorSkills.SetActive(false);
            sectorObject.SetActive(false);
        }

        if(type == 0) sectorMenu.SetActive(true);
        else if(type == 1) statsWindow.SetActive(true);
        else if(type == 2) sectorSkills.SetActive(true);
        else if (type == 3) sectorObject.SetActive(true);

        _index = 0;
        _prevIndex = 0;
        _indexWindow = type;

        SetDataSkill();
        SetDataObject();
    }
    public void CloseUI()
    {
        _group.alpha = 0;
        _group.interactable = false;
        _canMove = false;

        sectorMenu.SetActive(false);
        statsWindow.SetActive(false);
        sectorSkills.SetActive(false);
        sectorObject.SetActive(false);
        sectioners[0].alpha = 1f;

        ResetData();

        Pause.StateChange = State.Game;
    }
    // ------------------------ //
    private IEnumerator Move(float dir)
    {
        _canMove = false;

        #region Manager Move Per Direction Axis
        if (dir > 0) _index--;
        else _index++;

        int limit = 0;
        if (_indexWindow == 0 || _indexWindow == 2) limit = 3;
        else if (_indexWindow == 3) limit = 36;

        if (_index >= limit) _index = 0;
        if (_index < 0) _index = (limit - 1);
        #endregion

        if (_indexWindow == 0)
        {
            menuOptions[_prevIndex].sprite = unselectSprite;
            menuOptions[_index].sprite = selectSprite;
        }
        else if (_indexWindow == 2)
        {
            selectorSkill.transform.position = skillOptions[_index].transform.position;
            SetDataSkill();
        }
        else if (_indexWindow == 3)
        {
            selectorObj.transform.position = _imgObject[_index].transform.position;
            SetDataObject();
        }

        _prevIndex = _index;

        yield return new WaitForSeconds(0.19f);
        _canMove = true;
    }
    private IEnumerator MoveSector(int dir)
    {
        #region Move
        _canMove = false;
        if (dir > 0) _indexWindow++;
        else _indexWindow--;

        if (_indexWindow >= 5) _indexWindow = 0;
        else if (_indexWindow < 0) _indexWindow = 4;
        #endregion

        LoadSector(_indexWindow, true);

        PlayerActionStates.leftUI = false;
        PlayerActionStates.rightUI = false;
        ResetData();

        // MOSTRAR LA INFO DEL PRIMER OBJETO/SKILL
        SetDataSkill();
        SetDataObject();

        // PINTAR LOS SECTIONERS SEGUN EL SECTOR ACTUAL
        for (int i = 0; i < sectioners.Length; i++) { sectioners[i].alpha = 0.5f; }
        sectioners[_indexWindow].alpha = 1f;

        yield return new WaitForSeconds(0.15f);

        _canMove = true;
    }
    // ------------------------ //
    private void ResetData()
    {
        // SECTIONERS 
        for (int i = 0; i < sectioners.Length; i++) { sectioners[i].alpha = 0.35f; }

        // ---- MENU ---- //
        menuOptions[0].sprite = selectSprite;
        menuOptions[1].sprite = unselectSprite;
        menuOptions[2].sprite = unselectSprite;
        // ---- SKILLS ---- /
        selectorSkill.transform.position = skillOptions[0].transform.position;
        _titleSkillSelect.text = "";
        _descSkillSelect.text = "";
        // ---- OBJECTS ---- //
        selectorObj.transform.position = _imgObject[0].transform.position;
        _titleObject.text = "";
        _descObject.text = "";
    }
}