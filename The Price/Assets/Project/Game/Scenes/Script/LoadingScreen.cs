using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingScreen : MonoBehaviour {

    [Header("Elements of the UI")]
    [SerializeField] private Slider _progressBar;
    [SerializeField] private float _speedForLoad = 1f;

    [Header("Static Elements")]
    private static bool _inLoading { get; set; }

    [Header("Data for Scene")]
    private static int _countElementInScene { get; set; }
    private int _countTotalElement;

    private CollectableSelectable _collectable;

    private void Awake()
    {
        _collectable = FindAnyObjectByType<CollectableSelectable>();
    }
    private void Start()
    {
        InitialValues();
    }
    private void InitialValues()
    {
        InLoading = true;

        switch (SceneManager.GetActiveScene().name)
        {
            case "Menu": _countTotalElement = 5; break;
            case "Game": _countTotalElement = 6; break;
        }

        _progressBar.value = 0;
        _progressBar.maxValue = 100;

        StartCoroutine("ChangeValues");
    }
    private void Update()
    {
        if (!InLoading) Invoke("DestroyElement", 1f);
    }
    private void DestroyElement()
    {
        if(_collectable != null) _collectable.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
    private IEnumerator ChangeValues()
    {
        int randomValue = 0;
        do
        {
            randomValue = Random.Range(0, 25);
            _progressBar.value += randomValue;

            if (_progressBar.value >= _progressBar.maxValue) InLoading = false;

            yield return new WaitForSeconds(Random.Range(0.1f, _speedForLoad));

        } while (InLoading || CountElement < _countTotalElement);
    }
    // ---- SETTERS & GETTERS ---- //
    public static int CountElement
    {
        get { return _countElementInScene; }
        set {
            _countElementInScene = value;
        }
    }
    public static bool InLoading
    {
        get { return _inLoading; }
        set { _inLoading = value; }
    }
}
