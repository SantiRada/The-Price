using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SkillPlacement : MonoBehaviour {

    [Header("Content Skill")]
    [SerializeField] private SkillManager[] _skillPool;
    private static List<SkillManager> _localPool = new List<SkillManager>();

    [Header("Content UI")]
    [SerializeField] private GameObject _objSlot;
    [SerializeField] private float _distanceToMove;
    [SerializeField] private float _forceRotate;
    [SerializeField] private float _forceMovePerCard;
    private List<Selectable> _slots = new List<Selectable>();

    [Header("Movement")]
    private int _index = 0, _prevIndex = 0;
    private bool _canMove = true;
    private Vector3 _prevPosition = Vector3.zero;

    [Header("Private Content")]
    private HorizontalLayoutGroup _layoutGroup;
    private CanvasGroup _canvasGroup;
    private PlayerMovement _player;

    private void Awake()
    {
        _layoutGroup = GetComponent<HorizontalLayoutGroup>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }
    private void Start()
    {
        _localPool.AddRange(_skillPool);
    }
    public void InitialValues()
    {
        _canvasGroup.alpha = 1;
        _canvasGroup.interactable = true;

        Pause.StateChange = State.Interface;

        // CREAR LOS SLOTS
        for(int j = 0; j < _skillPool.Length; j++)
        {
            Selectable obj = Instantiate(_objSlot, Vector3.zero, Quaternion.identity, transform).GetComponent<Selectable>();
            _slots.Add(obj);
        }

        _layoutGroup.enabled = false;

        // APLICAR MOVIMIENTO Y ROTACION A LAS CARTAS
        for (int i = 0; i < _slots.Count; i++)
        {
            #region Rotacion
            float multiplierRotate = ((_slots.Count / 2) - i) * _forceRotate + _forceRotate;
            _slots[i].transform.localRotation = Quaternion.Euler(_slots[i].transform.localRotation.x, _slots[i].transform.localRotation.y, multiplierRotate);
            #endregion
            #region Posicionamiento
            float newY = _slots[i].transform.localPosition.y - Factorial((int)(_slots.Count / 2) - i) * _forceMovePerCard;

            _slots[i].transform.localPosition = new Vector3(_slots[i].transform.localPosition.x, newY, _slots[i].transform.localPosition.z);
            #endregion
        }
        _slots[0].Select();
    }
    private float Factorial(int value)
    {
        float number = 0;
        for(int i = 0; i < Mathf.Abs(value); i++) { number += i; }

        return number;
    }
    private void Update()
    {
        if (Pause.state != State.Interface) return;

        float h = Input.GetAxis("Horizontal");
        if (h != 0 && _canMove) StartCoroutine(MoveSlot(h));

        if (!_canMove)
        {
            Vector3 newPos = new Vector3(_slots[_index].transform.localPosition.x, _slots[_index].gameObject.transform.localPosition.y + _distanceToMove, 0);
            _slots[_index].gameObject.transform.localPosition = Vector3.Lerp(_slots[_index].transform.localPosition, newPos, 1f * Time.deltaTime);
        }

        // SELECCIONAR UNA SKILL
        if (Input.GetButtonDown("Fire1")) StartCoroutine("Selected");
    }
    private IEnumerator MoveSlot(float dir)
    {
        _canMove = false;
        _prevIndex = _index;

        if(_prevPosition != Vector3.zero) _slots[_prevIndex].gameObject.transform.localPosition = _prevPosition;

        if (dir > 0) _index++;
        else _index--;

        if (_index >= _slots.Count) _index = 0;
        else if (_index < 0) _index = (_slots.Count - 1);

        _prevPosition = _slots[_index].transform.localPosition;

        _slots[_index].Select();
        _slots[_index].transform.SetAsLastSibling();
        yield return new WaitForSeconds(0.25f);
        _canMove = true;
    }
    private IEnumerator Selected()
    {
        if(_player.skills.Count >= 3)
        {
            // MODIFICANDO UNA SKILL ACTUAL
        }
        else
        {
            // AGREGANDO UNA NUEVA SKILL
        }
        yield return new WaitForSeconds(0.25f);
        Pause.StateChange = State.Game;
    }
}
