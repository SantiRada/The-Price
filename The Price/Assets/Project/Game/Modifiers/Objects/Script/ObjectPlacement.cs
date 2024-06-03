using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ActionForControlPlayer;

public class ObjectPlacement : MonoBehaviour {

    [Header("Content Object")]
    [SerializeField] private TypeObject _typeObject;
    [SerializeField] private Object[] _poolBasic;
    [SerializeField] private Object[] _poolEpic;
    [SerializeField] private Object[] _poolLegendary;
    [SerializeField] private Object[] _poolMythical;
    private static List<Object> _localPool = new List<Object>();
    private List<Object> _objectCreated = new List<Object>();

    [Header("Content UI")]
    public List<Image> _slots = new List<Image>();
    public List<Image> _slotIcon = new List<Image>();
    public List<TextMeshProUGUI> _slotName = new List<TextMeshProUGUI>();
    public List<TextMeshProUGUI> _slotDesc = new List<TextMeshProUGUI>();

    [Header("Selection")]
    public Sprite cardBase;
    public Sprite cardSelect;
    [SerializeField] private int[] prioritiesForType = { 65, 20, 10, 5 };

    [Header("Movement")]
    [SerializeField] private float _timeBetweenCards;
    private int _index = 0, _prevIndex = 0;
    private bool _canMove = true;

    [Header("Private Content")]
    private CanvasGroup _canvasGroup;
    private PlayerStats _player;

    private static bool _startObject = false, _initial = false;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();

        _player = FindAnyObjectByType<PlayerStats>();
    }
    private void Start()
    {
        _canvasGroup.interactable = false;
        _canvasGroup.alpha = 0;

        CalculateLocalObject();
    }
    private List<Object> GetListObject()
    {
        List<Object> testFor = new List<Object>();

        if (_typeObject == TypeObject.basic) testFor.AddRange(_poolBasic);
        if (_typeObject == TypeObject.epic) testFor.AddRange(_poolEpic);
        if (_typeObject == TypeObject.legendary) testFor.AddRange(_poolLegendary);
        if (_typeObject == TypeObject.mythical) testFor.AddRange(_poolMythical);

        return testFor;
    }
    private void CalculateLocalObject()
    {
        List<Object> testFor = GetListObject();

        if (_player.objects.Count != 0)
        {
            bool canUse = true;

            for (int i = 0; i < testFor.Count; i++)
            {
                for(int j = 0; j < _player.objects.Count; j++)
                {
                    if (_player.objects[j] == testFor[i])
                    {
                        canUse = false;
                        break;
                    }
                }
                if(canUse) _localPool.Add(testFor[i]);
            }
        }
        else { _localPool = testFor; }
    }
    public static void StartObjectSelector()
    {
        _startObject = true;
        _initial = true;
    }
    public IEnumerator InitialValues()
    {
        _startObject = false;
        _canvasGroup.alpha = 1;
        _canvasGroup.interactable = true;

        Pause.StateChange = State.Interface;

        // APLICAR VALORES A CADA CARTA
        /*List<Object> values = GetListObject();
        int rnd = 0;
        bool canUse = true;
        do
        {
            rnd = Random.Range(0, values.Count);

            for (int i = 0; i < _objectCreated.Count; i++)
            {
                if (_objectCreated[i] == values[rnd])
                {
                    canUse = false;
                    break;
                }
            }

            if (canUse) _objectCreated.Add(values[rnd]);

        } while (_objectCreated.Count < 3);*/

        _objectCreated.Add(_poolBasic[0]);
        _objectCreated.Add(_poolBasic[1]);
        _objectCreated.Add(_poolBasic[2]);

        for (int i = 0; i < _slots.Count; i++)
        {
            _slots[i].gameObject.SetActive(true);
            _slots[i].sprite = cardBase;

            // _slotIcon[i].sprite = _objectCreated[i].icon;
            _slotName[i].text = LanguageManager.GetValue("Object", _objectCreated[i].itemName);
            _slotDesc[i].text = LanguageManager.GetValue("Object", _objectCreated[i].description);

            yield return new WaitForSeconds(_timeBetweenCards);
        }

        yield return new WaitForSeconds(0.19f);
        _index = 1;
        StartCoroutine(MoveSlot(1));
    }
    // ---- INITIAL ---- //
    private void Update()
    {
        if (_startObject && _initial) StartCoroutine("InitialValues");
        else if (!_initial) return;

        if (Pause.state != State.Interface) return;

        float h = Input.GetAxis("Horizontal");
        if (h != 0 && _canMove) StartCoroutine(MoveSlot(h));

        // SELECCIONAR UNA SKILL
        if (Input.GetButtonDown("Fire1") || PlayerActionStates.IsUse) StartCoroutine("Selected");
    }
    private IEnumerator MoveSlot(float dir)
    {
        _canMove = false;
        _prevIndex = _index;

        if (dir > 0) _index++;
        else _index--;

        if (_index >= _slots.Count) _index = 0;
        else if (_index < 0) _index = (_slots.Count - 1);

        _slots[_prevIndex].sprite = cardBase;
        _slots[_index].sprite = cardSelect;

        yield return new WaitForSeconds(0.19f);
        _canMove = true;
    }
    private IEnumerator Selected()
    {
        // MOVER TODAS LAS CARTAS HACIA ABAJO EXCEPTO LA SELECCIONADA
        for (int i = 0; i < _slots.Count; i++)
        {
            if (i != _index)
            {
                Vector3 targetPosition = _slots[i].transform.localPosition + new Vector3(0, -(Screen.height), 0);

                do
                {
                    _slots[i].transform.localPosition = Vector3.Lerp(_slots[i].transform.localPosition, targetPosition, 0.75f);
                    yield return new WaitForSeconds(0.035f);
                } while (Vector3.Distance(_slots[i].transform.localPosition, targetPosition) > 5f);

                _slots[i].transform.localPosition = targetPosition;
            }
        }
        yield return new WaitForSeconds(0.19f);

        Vector3 targetIndex = _slots[_index].transform.localPosition + new Vector3(0, -(Screen.height), 0);

        do
        {
            _slots[_index].transform.localPosition = Vector3.Lerp(_slots[_index].transform.localPosition, targetIndex, 0.75f);
            yield return new WaitForSeconds(0.035f);
        } while (Vector3.Distance(_slots[_index].transform.localPosition, targetIndex) > 5f);

        _slots[_index].transform.localPosition = targetIndex;

        yield return new WaitForSeconds(0.19f);

        // AGREGO EL OBJETO AL JUGADOR
        _player.AddObject(_objectCreated[_index]);
        // _player.GetComponent<PlayerStats>().SetChangeObject(); AUN NO EXISTE ESTA FUNCION

        RestartContent();
    }
    private void RestartContent()
    {
        StopAllCoroutines();

        for (int i = 0; i < _objectCreated.Count; i++)
        {
            _slotName[i].text = "";
            _slotDesc[i].text = "";
            _slotIcon[i].sprite = null;
            _slots[i].gameObject.SetActive(false);
        }

        _objectCreated.Clear();

        _canvasGroup.alpha = 0;
        _canvasGroup.interactable = false;

        _startObject = false;
        _initial = false;
        _canMove = false;
        _index = 0;
        _prevIndex = 0;

        Pause.StateChange = State.Game;
    }
    // ---- MANEJO DEL POOL ---- //
    public Object RandomPool()
    {
        int rnd = Random.Range(0, 101);

        if(rnd < prioritiesForType[3]) return _poolMythical[Random.Range(0, _poolMythical.Length)];
        else if (rnd >= prioritiesForType[3] && rnd < prioritiesForType[2]) return _poolLegendary[Random.Range(0, _poolLegendary.Length)];
        else if (rnd >= prioritiesForType[2] && rnd < prioritiesForType[1]) return _poolEpic[Random.Range(0, _poolEpic.Length)];
        else return _poolBasic[Random.Range(0, _poolBasic.Length)];
    }
}
