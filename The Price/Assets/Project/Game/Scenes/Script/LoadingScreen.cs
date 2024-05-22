using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour {

    [Header("Content UI")]
    [SerializeField] private Slider _loadingBar;
    [SerializeField] private float _timeToLoad;
    public static bool inLoading = true;

    private void Start()
    {
        StartCoroutine("Loading");
    }
    private IEnumerator Loading()
    {
        inLoading = true;
        do
        {
            float rnd = Random.Range(5, (_loadingBar.maxValue / 5));
            _loadingBar.value += rnd;
            yield return new WaitForSeconds(_timeToLoad);
        } while (_loadingBar.value < _loadingBar.maxValue);

        CloseLoading();
    }
    private void CloseLoading()
    {
        inLoading = false;
        gameObject.SetActive(false);
    }
}
