using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectorUI : MonoBehaviour {

    [Header("Data UI")]
    [SerializeField] private TextMeshProUGUI[] _name;
    [SerializeField] private TextMeshProUGUI[] _description;

    [Header("Data UI: Skills")]
    [SerializeField] private TextMeshProUGUI[] _type;
    [SerializeField] private GameObject[] _sectionLoaders;
    [SerializeField] private TextMeshProUGUI[] _loaders;
    [SerializeField] private GameObject[] _sectionFragments;
    [SerializeField] private TextMeshProUGUI[] _fragments;
    [SerializeField] private GameObject[] _sectionDamage;
    [SerializeField] private TextMeshProUGUI[] _damage;

    [Header("Data Confirmation Skill")]
    [SerializeField] private GameObject _windowConfirm;

    [Header("Data Movement")]
    [SerializeField] private Image[] _cardSelector;
    [SerializeField] private Sprite[] _normalCard, _goldCard;
    [SerializeField, Tooltip("Tiempo que se espera para poder volver a moverse")] private float _delayMovement;
    [SerializeField, Tooltip("Tiempo que tarda la animación de mover cada tarjeta")] private float _delayAnimation;
    [SerializeField] private Vector3 _distanceToCenter;

    [Header("Data Extra")]
    [SerializeField] private GameObject _featuredUsedSector;
    [SerializeField] private GameObject _infoExtraSector;
    [SerializeField] private TextMeshProUGUI _infoExtra;
    [SerializeField] private TextMeshProUGUI _featuredUsed;
    private Vector3[] _featuredAndInfoPosition = new Vector3[2];

    [Header("Private Data")]
    private List<Vector3> _cardPosition = new List<Vector3>();
    private int _posCurrent = 0, _prevPosition = 0;
    private bool _canMove = true, _canDetect = false;
    private List<string> _featuredPosition = new List<string>();
    private List<string> _infoPosition = new List<string>();
    
    [Header("Selectioner")]
    private List<GameObject> _selectableObject = new List<GameObject>();
    private ActionForControlPlayer _dataPlayer;
    private int _select = -1;

    private void Start()
    {
        InitialValues();
    }
    private void InitialValues()
    {
        for (int i = 0; i < _cardSelector.Length; i++)
        {
            _cardPosition.Add(_cardSelector[i].gameObject.transform.position);
        }

        _cardSelector[0].gameObject.transform.position = _cardPosition[0] + _distanceToCenter;
        _cardSelector[0].sprite = _normalCard[1];

        _canMove = true;
        _canDetect = false;
        _select = -1;

        _featuredAndInfoPosition[0] = _featuredUsedSector.transform.position;
        _featuredAndInfoPosition[1] = _infoExtraSector.transform.position;
    }
    public void WaitForMove()
    {
        _canDetect = true;
    }
    private void Update()
    {
        if (LoadingScreen.InLoading || _select != -1 || !_canDetect) return;

        if(Input.GetAxis("Horizontal") != 0 && _canMove) StartCoroutine(Move(Input.GetAxis("Horizontal")));

        MoveValues();

        if (Input.GetButtonDown("Fire1") && _canDetect) Select();
    }
    private void Select()
    {
        _canMove = false;
        _canDetect = false;
        _select = _posCurrent;

        _windowConfirm.SetActive(true);
    }
    private void SelectElement()
    {
        // Skills
        GameObject obj = Instantiate(_selectableObject[_select], _dataPlayer.transform.position, Quaternion.identity, _dataPlayer.transform);
        _dataPlayer.SetSkill(obj, true);

        // Quitar la pausa y ocultar el elemento de UI
        PauseMenu.inPause = false;
        gameObject.SetActive(false);
    }
    public int GetSelect()
    {
        return _select;
    }
    private void MoveValues()
    {
        if (!_canMove)
        {
            Vector3 _elementPrev = _cardSelector[_prevPosition].gameObject.transform.position;
            Vector3 _elementCurrent = _cardSelector[_posCurrent].gameObject.transform.position;

            Vector3 _newPosition = _cardPosition[_posCurrent] + _distanceToCenter;

            // Devolver a su posición base al elemento seleccionado anteriormente
            _cardSelector[_prevPosition].gameObject.transform.position = Vector3.Lerp(_elementPrev, _cardPosition[_prevPosition], _delayAnimation * Time.deltaTime);

            // Seleccionar el nuevo elemento
            _cardSelector[_posCurrent].gameObject.transform.position = Vector3.Lerp(_elementCurrent, _newPosition, _delayAnimation * Time.deltaTime);

            // Movimiento de los objetos Featured && Extra
            _featuredUsedSector.transform.position = Vector3.Lerp(_featuredUsedSector.transform.position, _featuredAndInfoPosition[0], _delayAnimation * Time.deltaTime);
            _infoExtraSector.transform.position = Vector3.Lerp(_infoExtraSector.transform.position, _featuredAndInfoPosition[1], _delayAnimation * Time.deltaTime);
        }
    }
    private IEnumerator Move(float dir)
    {
        _canMove = false;

        _prevPosition = _posCurrent;
        _cardSelector[_posCurrent].sprite = _normalCard[0];
        
        // Establecer nueva posición según el movimiento del jugador ------- //
        _posCurrent = ChangePosition(dir);
        _cardSelector[_posCurrent].sprite = _normalCard[1];

        // COLOCAR FEATURED & INFO-EXTRA CORRECTOS ------------------------- //
        ChangesInExtra();
        yield return new WaitForSeconds(_delayMovement);
        _canMove = true;
    }
    private int ChangePosition(float dir)
    {
        int pos = _posCurrent;

        if (dir > 0) pos++;
        else pos--;

        if (pos  >= 3) pos = 0;
        if (pos  < 0) pos = 2;

        return pos;
    }
    private void ChangesInExtra()
    {
        _featuredUsed.text = _featuredPosition[_posCurrent];
        if (_infoPosition[_posCurrent] != "")
        {
            _infoExtraSector.SetActive(true);
            _infoExtra.text = _infoPosition[_posCurrent];
        }
        else { _infoExtraSector.SetActive(false); }

        // Movimiento de los objetos Featured && Extra
        Vector3 distance = new Vector3(50, 0, 0);

        _featuredUsedSector.transform.position = (_featuredAndInfoPosition[0] - distance);
        _infoExtraSector.transform.position = (_featuredAndInfoPosition[1] - distance);
    }
    public void ShowInUI(GameObject obj, List<string> values, int index, GameObject player)
    {
        _name[index].text = values[0];
        _description[index].text = values[1];
        _type[index].text = values[3];
        _loaders[index].text = values[5];
        if (values[6] == "1")
        {
            _sectionFragments[index].SetActive(true);
            _fragments[index].text = values[7];
        }
        else { _sectionFragments[index].SetActive(false); }
        if (values[8] != "0")
        {
            _sectionDamage[index].SetActive(true);
            _damage[index].text = values[8].ToString();
        }
        else { _sectionDamage[index].SetActive(false); }

        _featuredPosition.Add(values[2]);
        _infoPosition.Add(values[9]);

        _selectableObject.Add(obj);
        _dataPlayer = player.GetComponent<ActionForControlPlayer>();
    }
}