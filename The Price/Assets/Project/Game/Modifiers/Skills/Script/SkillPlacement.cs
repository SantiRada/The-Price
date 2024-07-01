using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ActionForControlPlayer;

public class SkillPlacement : MonoBehaviour {

    [Header("Content Skill")]
    [SerializeField] private SkillManager[] _skillPool;
    [SerializeField] private InteractiveSkill _objForScene;
    private static List<SkillManager> _localPool = new List<SkillManager>();

    [Header("Content UI")]
    public GameObject _objSlot;
    public float _distanceToMove;
    public float _forceRotate;
    public float _forceMovePerCard;
    private List<GameObject> _slots = new List<GameObject>();
    private List<Image> _slotImage = new List<Image>();

    [Header("Selection")]
    public Sprite _card;
    public Sprite _cardHover;
    public Sprite _cardSelect;
    [Space]
    [SerializeField] private GameObject _skillSelector;
    [SerializeField] private TextMeshProUGUI _nameSkill;
    [SerializeField] private TextMeshProUGUI _descSkill;
    private bool _inSelect = false, _canDetect = true;

    [Header("Audio")]
    public AudioClip _dropCard;
    public AudioClip _rotateCard;
    private AudioSource _audio;

    [Header("Movement")]
    [SerializeField, Range(0, 0.5f)] private float _deadTime;
    [SerializeField] private Vector3 _posSelected;
    [SerializeField, Tooltip("Tiempo entre aparición de cada carta")] private float _delayBetweenCard;
    [SerializeField, Tooltip("Velocidad de movimiento de la carta seleccionada")] private float _moveCardSelected;
    private int _index = 0, _prevIndex = 0;
    private bool _canMove = true;
    private Vector3 _prevPosition = Vector3.zero;

    [Header("Private Content")]
    private HorizontalLayoutGroup _layoutGroup;
    private CanvasGroup _canvasGroup;
    private PlayerStats _player;

    private static bool _startSkills = false, _initial = false;
    private SkillManager _skillSelected;

    private void Awake()
    {
        _layoutGroup = GetComponent<HorizontalLayoutGroup>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _audio = GetComponent<AudioSource>();

        _player = FindAnyObjectByType<PlayerStats>();
    }
    private void Start()
    {
        _localPool.AddRange(_skillPool);

        _skillSelector.SetActive(false);
    }
    public static void StartSkillsSelector()
    {
        _startSkills = true;
        _initial = true;
    }
    public IEnumerator InitialValues()
    {
        _startSkills = false;
        _canDetect = false;
        _canvasGroup.alpha = 1;
        _canvasGroup.interactable = true;
        _inSelect = false;

        Pause.StateChange = State.Interface;

        // CREAR LOS SLOTS
        for(int j = 0; j < _skillPool.Length; j++)
        {
            Vector3 posCreator = new Vector3(_posSelected.x + (Screen.width / 2 + 100), _posSelected.y, _posSelected.z);
            GameObject obj = Instantiate(_objSlot, posCreator, Quaternion.identity, transform);
            _slots.Add(obj);
            _slotImage.Add(obj.GetComponent<Image>());

            _audio.PlayOneShot(_dropCard);
            
            if (j > (_skillPool.Length - 4)) obj.GetComponent<Animator>().enabled = false;
            yield return new WaitForSeconds(_delayBetweenCard);
        }

        yield return new WaitForSeconds(0.005f);
        _layoutGroup.enabled = false;

        // APLICAR MOVIMIENTO Y ROTACION A LAS CARTAS
        for (int i = 0; i < _slots.Count; i++)
        {
            #region Rotacion
            float multiplierRotate = ((_slots.Count / 2) - i) * _forceRotate + _forceRotate;
            Quaternion rotation = Quaternion.Euler(_slots[i].transform.localRotation.x, _slots[i].transform.localRotation.y, multiplierRotate);

            StartCoroutine(ChangeRotation(i, rotation));
            #endregion
            #region Posicionamiento
            float newY = _slots[i].transform.localPosition.y - Factorial((int)(_slots.Count / 2) - i) * _forceMovePerCard;
            Vector3 position = new Vector3(_slots[i].transform.localPosition.x, newY, _slots[i].transform.localPosition.z);

            StartCoroutine(ChangePosition(i, position));
            #endregion
        }
        
        _audio.PlayOneShot(_rotateCard);

        yield return new WaitForSeconds(_deadTime);
        _canDetect = true;
        _index = -1;
        StartCoroutine(MoveSlot(1));
    }
    private IEnumerator ChangeRotation(int i,  Quaternion rotation)
    {
        do
        {
            _slots[i].transform.localRotation = Quaternion.Lerp(_slots[i].transform.localRotation, rotation, 0.6f);
            yield return new WaitForSeconds(0.05f);
        } while (Vector3.Distance(_slots[i].transform.localRotation.eulerAngles, rotation.eulerAngles) > 5f);

        _slots[i].transform.localRotation = rotation;
    }
    private IEnumerator ChangePosition(int i, Vector3 position)
    {
        do
        {
            _slots[i].transform.localPosition = Vector3.Lerp(_slots[i].transform.localPosition, position, 0.6f);
            yield return new WaitForSeconds(0.05f);
        } while (Vector3.Distance(_slots[i].transform.localPosition, position) > 5f);

        _slots[i].transform.localPosition = position;
    }
    private float Factorial(int value)
    {
        float number = 0;
        for(int i = 0; i < Mathf.Abs(value); i++) { number += i; }

        return number;
    }
    // ---- INITIAL ---- //
    private void Update()
    {
        if (_startSkills && _initial) StartCoroutine("InitialValues");
        else if(!_initial) return;

        if (Pause.state != State.Interface || !_canDetect) return;

        if (_inSelect)
        {
            if (Input.GetButtonDown("Fire1") || PlayerActionStates.IsUse) SelectCard();

            return;
        }

        float h = Input.GetAxis("Horizontal");
        if (h != 0 && _canMove) StartCoroutine(MoveSlot(h));

        if (!_canMove)
        {
            Vector3 newPos = new Vector3(_slots[_index].transform.localPosition.x, _slots[_index].gameObject.transform.localPosition.y + _distanceToMove, 0);
            _slots[_index].gameObject.transform.localPosition = Vector3.Lerp(_slots[_index].transform.localPosition, newPos, 1f * Time.deltaTime);
        }

        // SELECCIONAR UNA SKILL
        if (Input.GetButtonDown("Fire1") || PlayerActionStates.IsUse) StartCoroutine("Selected");
    }
    private IEnumerator MoveSlot(float dir)
    {
        _canMove = false;
        _prevIndex = _index;

        if(_prevPosition != Vector3.zero)
        {
            _slots[_prevIndex].gameObject.transform.localPosition = _prevPosition;
            _slotImage[_prevIndex].sprite = _card;
        }

        if (dir > 0) _index++;
        else _index--;

        if (_index >= _slots.Count) _index = 0;
        else if (_index < 0) _index = (_slots.Count - 1);

        _prevPosition = _slots[_index].transform.localPosition;
        _slotImage[_index].sprite = _cardHover;

        _slots[_index].transform.SetAsLastSibling();

        yield return new WaitForSeconds(0.25f);
        _canMove = true;
    }
    private IEnumerator Selected()
    {
        _inSelect = true;
        _canDetect = false;
        // MOVER TODAS LAS CARTAS HACIA ABAJO EXCEPTO LA SELECCIONADA
        for(int i = 0; i < _slots.Count; i++)
        {
            if(i != _index)
            {
                Vector3 targetPosition = _slots[i].transform.localPosition + new Vector3(0, -(Screen.height), 0);

                _audio.volume = 0.1f;
                _audio.PlayOneShot(_dropCard);
                do
                {
                    _slots[i].transform.localPosition = Vector3.Lerp(_slots[i].transform.localPosition, targetPosition, 0.75f);
                    yield return new WaitForSeconds(0.035f);
                } while (Vector3.Distance(_slots[i].transform.localPosition, targetPosition) > 5f);

                _slots[i].transform.localPosition = targetPosition;
            }
            else { StartCoroutine("MovementForCardSelected"); }
        }
    }
    private IEnumerator MovementForCardSelected()
    {
        // MOVER LA CARTA SELECCIONADA AL CENTRO
        _skillSelector.SetActive(true);
        _slots[_index].transform.SetParent(_skillSelector.transform);
        _slots[_index].transform.SetAsFirstSibling();
        do
        {
            _slots[_index].transform.localPosition = Vector3.Lerp(_slots[_index].transform.localPosition, _posSelected, 0.6f);
            _slots[_index].transform.localRotation = Quaternion.Lerp(_slots[_index].transform.localRotation, Quaternion.identity, 0.6f);
            yield return new WaitForSeconds(0.05f);
        } while (Vector3.Distance(_slots[_index].transform.localPosition, _posSelected) > 5f);

        _slots[_index].transform.localPosition = _posSelected;
        _slots[_index].transform.localRotation = Quaternion.identity;

        // APLICAR ANIMACIÓN DE GIRAR
        do
        {
            _slots[_index].transform.localScale = Vector3.Lerp(_slots[_index].transform.localScale, new Vector3(0, 1, 1), 0.6f);
            yield return new WaitForSeconds(0.05f);
        } while (Vector3.Distance(_slots[_index].transform.localScale, new Vector3(0, 1, 1)) > 0.05f);

        // CAMBIAR EL SPRITE DE LA CARTA SELECCIONADA
        _slots[_index].GetComponent<Image>().sprite = _cardSelect;

        do
        {
            _slots[_index].transform.localScale = Vector3.Lerp(_slots[_index].transform.localScale, new Vector3(1.15f, 1.15f, 1), 0.6f);
            yield return new WaitForSeconds(0.05f);
        } while (Vector3.Distance(_slots[_index].transform.localScale, new Vector3(1.15f, 1.15f, 1)) > 0.05f);
        _slots[_index].transform.localScale = new Vector3(1.15f, 1.15f, 1);

        _skillSelected = CalculateSkill();

        // ESTILOS PARA SU PRESENTACIÓN ----- //
        _slots[_index].GetComponent<Animator>().enabled = true;
        _slots[_index].GetComponent<Animator>().SetBool("Active", true);
        // ---------------------------------- //

        _nameSkill.text = LanguageManager.GetValue("Skill", _skillSelected.skillName);
        _descSkill.text = LanguageManager.GetValue("Skill", _skillSelected.descName);

        yield return new WaitForSeconds(_deadTime);
        _canDetect = true;

    }
    private SkillManager CalculateSkill()
    {
        SkillManager skill;
        bool canAdvance = true;
        int value = Random.Range(0, _localPool.Count);
        do
        {
            skill = _localPool[value];
            for (int i = 0; i < _player.skills.Count; i++)
            {
                if (_player.skills[i] == skill)
                {
                    canAdvance = false;
                    break;
                }

                if (_player.skills[i] != skill && i >= (_player.skills.Count - 1)) canAdvance = true;
            }

            if(!canAdvance) value = Random.Range(0, _localPool.Count);
        } while (!canAdvance);

        return skill;
    }
    private void SelectCard()
    {
        if(_player.skills.Count >= 2)
        {
            // CREO LA HABILIDAD EN EL SUELO
            InteractiveSkill sk = Instantiate(_objForScene.gameObject, _player.transform.position, Quaternion.identity).GetComponent<InteractiveSkill>();
            sk.isNew = true;
        }
        else
        {
            // AGREGO LA HABILIDAD AL JUGADOR
            _player.skills.Add(_skillSelected);
            FindAnyObjectByType<StatsInUI>().SetChangeSkillsInUI(_player.skills);
        }

        RestartContent();
    }
    private void RestartContent()
    {
        StopAllCoroutines();

        for(int i = 0; i< _slots.Count; i++)
        {
            Destroy(_slots[i].gameObject);
        }

        _slots.Clear();
        _slotImage.Clear();

        _layoutGroup.enabled = true;

        _canvasGroup.alpha = 0;
        _canvasGroup.interactable = false;

        _startSkills = false;
        _initial = false;
        _inSelect = false;
        _canDetect = false;
        _canMove = false;
        _index = 0;
        _prevIndex = 0;
        _prevPosition = Vector3.zero;
        _skillSelected = null;

        _nameSkill.text = "";
        _descSkill.text = "";
        _skillSelector.SetActive(false);

        Pause.StateChange = State.Game;
    }
    // ---- MANEJO DEL POOL ---- //
    public SkillManager RandomPool()
    {
        return _localPool[Random.Range(0, _localPool.Count)];
    }
    public SkillManager GetSkillPerID(int id)
    {
        for(int i = 0; i < _skillPool.Length; i++)
        {
            if (_skillPool[i].skillID == id)
            {
                return _skillPool[i];
            }
        }

        return null;
    }
}
