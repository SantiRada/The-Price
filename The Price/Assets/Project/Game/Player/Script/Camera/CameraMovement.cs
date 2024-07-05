using Unity.VisualScripting;
using UnityEngine;

public enum SizeCamera { normal, boss, specific }
public class CameraMovement : MonoBehaviour {

    [Header("Content Move")]
    public GameObject target;
    public float offset;

    [Header("Private Content")]
    private static Vector2 min, max;
    private Vector3 _initialPos;
    private static bool _diePlayer;

    [Header("Content Shake")]
    private static CameraMovement instance;
    private static Camera _cam;

    private float shakeDuration = 0f;
    private float shakeAmount = 0.7f;
    private float decreaseFactor = 1.0f;
    private static Vector3 _originalPos;
    private static bool _shake = false;

    [Header("Caller Camera")]
    public static float speedCall = 5f;
    private static Vector3 prevPosCamera;
    private static Vector2 posCaller;
    private static float timeToCall;
    private static bool inCall = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            _cam = Camera.main;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        _cam = GetComponent<Camera>();
        _diePlayer = false;

        _initialPos = transform.position;
    }
    private void Update()
    {
        if (inCall)
        {
            if (timeToCall <= 0)
            {
                _cam.transform.position = Vector3.Lerp(_cam.transform.position, prevPosCamera, 0.5f * speedCall * Time.deltaTime);

                if(Vector3.Distance(_cam.transform.position, prevPosCamera) <= 5) inCall = false;
            }
            else
            {
                timeToCall -= Time.deltaTime;

                _cam.transform.position = Vector3.Lerp(_cam.transform.position, new Vector3(posCaller.x, posCaller.y, transform.position.z), 0.5f * speedCall * Time.deltaTime);
            }

            return;
        }

        if (_shake)
        {
            if (shakeDuration > 0)
            {
                _cam.transform.localPosition = _originalPos + Random.insideUnitSphere * shakeAmount;

                shakeDuration -= Time.deltaTime * decreaseFactor;
            }
            else
            {
                shakeDuration = 0f;
                _cam.transform.localPosition = _originalPos;
                _shake = false;
            }
        }

        if (_diePlayer) return;

        Vector3 newPos = new Vector3(Mathf.Clamp(target.transform.position.x, min.x, max.x), Mathf.Clamp(target.transform.position.y, min.y, max.y), _initialPos.z);
        transform.position = Vector3.Lerp(transform.position, newPos, offset * Time.deltaTime);
    }
    public static void SetMinMax(Vector2 minValues, Vector2 maxValues)
    {
        min = minValues;
        max = maxValues;
    }
    public static void SetDie()
    {
        _diePlayer = true;
        _cam.cullingMask = (1 << 5) | (1 << 7);
    }
    public static void Shake(float intensity, float duration)
    {
        _originalPos = _cam.transform.localPosition;

        if (instance != null)
        {
            _shake = true;
            instance.shakeAmount = intensity;
            instance.shakeDuration = duration;
        }
    }
    public static void CallCamera(Vector2 pos, float time)
    {
        prevPosCamera = _cam.transform.position;
        timeToCall = time;
        posCaller = pos;
        inCall = true;
    }
    public static void SetSize(SizeCamera type)
    {
        switch (type)
        {
            case SizeCamera.specific: _cam.orthographicSize = 3; break;
            case SizeCamera.normal: _cam.orthographicSize = 5; break;
            case SizeCamera.boss: _cam.orthographicSize = 8; break;
        }
    }
}