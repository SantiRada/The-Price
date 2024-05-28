using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour {

    [Header("UI Content")]
    [SerializeField] private GameObject _windowUI;
    [SerializeField] private TextMeshProUGUI _nameContent;
    [SerializeField] private TextMeshProUGUI _descContent;
    [SerializeField] private TextMeshProUGUI _clickToContinue;
    [SerializeField] private Image _characterDialogue;

    [Header("Movement")]
    [SerializeField, Range(0, 0.5f)] private float _appearanceSpeed;
    [SerializeField] private float _deadTime;
    [Space]
    [SerializeField] private Vector3 _posFinal;
    private Vector3 _initialPos;

    private bool _finishLoad = false;
    private bool _canDetect = true;
    private int[] _allContent;
    private int _index = 0;

    private void Start()
    {
        _initialPos = _windowUI.transform.position;
        _windowUI.SetActive(false);
    }
    private void Update()
    {
        if (!_canDetect)
        {
            StartCoroutine("MoveDetector");
            return;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            if (_finishLoad)
            {
                _index++;

                if(_index >= _allContent.Length)
                {
                    StartCoroutine("FinishDialogue");
                    return;
                }
                StartCoroutine(ShowPhrase(_allContent[_index]));
            }
            else
            {
                StopAllCoroutines();

                _descContent.text = LanguageManager.GetValue("Game", _allContent[_index]);
                _clickToContinue.gameObject.SetActive(true);
                _finishLoad = true;

                _canDetect = false;
            }
        }
    }
    public void Active(int name, int[] phrases)
    {
        Pause.StateChange = State.Interface;

        StartCoroutine("Appearance");

        _allContent = phrases;
        _nameContent.text = LanguageManager.GetValue("Game", name);

        StartCoroutine(ShowPhrase(phrases[_index]));
    }
    private IEnumerator Appearance()
    {
        _windowUI.SetActive(true);
        _windowUI.transform.position = _posFinal;

        do
        {
            _windowUI.transform.position = Vector3.Lerp(_windowUI.transform.position, _initialPos, 0.6f);
            yield return new WaitForSeconds(0.05f);
        } while (Vector3.Distance(_windowUI.transform.position, _initialPos) > 5f);

        _windowUI.transform.position = _initialPos;
    }
    private IEnumerator ShowPhrase(int phrase)
    {
        _finishLoad = false;
        _canDetect = false;
        string phraseFinal = LanguageManager.GetValue("Game", phrase);

        for (int i = 0; i < phraseFinal.Length; i++)
        {
            _descContent.text += phraseFinal[i];
            yield return new WaitForSeconds(_appearanceSpeed);
        }

        _finishLoad = true;
        _clickToContinue.gameObject.SetActive(true);
    }
    private IEnumerator MoveDetector()
    {
        yield return new WaitForSeconds(_deadTime);
        _canDetect = true;
    }
    private IEnumerator FinishDialogue()
    {
        do
        {
            _windowUI.transform.position = Vector3.Lerp(_windowUI.transform.position, _posFinal, 0.6f);
            yield return new WaitForSeconds(0.05f);
        } while (Vector3.Distance(_windowUI.transform.position, _posFinal) > 5f);

        Pause.StateChange = State.Game;
    }
}
