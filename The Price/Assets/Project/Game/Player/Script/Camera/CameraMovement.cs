using UnityEngine;

public enum SizeCamera { normal, boss, specific }
public class CameraMovement : MonoBehaviour {

    [Header("Content Move")]
    public GameObject target;
    public float offset;
    public GameObject darknessBG;
    private static GameObject _overlayOBJ;

    [Header("Private Content")]
    private static Vector2 min, max;
    private static bool _diePlayer;
    private Vector3 _initialPos;

    [Header("Camera Content")]
    private static CameraMovement instance;
    private static Camera _cam;

    [Header("Content Shake")]
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

    [Header("Battle System")]
    [Tooltip("Velocidad del cambio de tamaño de la cámara cuando el jugador se aleja o acerca al enemigo")] public float zoomSpeed;
    private static BossSystem _boss;

    [Header("Sizing")]
    private static int _newPerspective;
    private static bool _inChangeSizing;
    private static bool _inCinematic;

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

        _overlayOBJ = darknessBG;
        _overlayOBJ.SetActive(false);
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

        if (_boss == null)
        {
            Vector3 newPos;
            if (_inCinematic)
            {
                newPos = new Vector3(Mathf.Clamp(target.transform.position.x, min.x - 2, max.x + 2), Mathf.Clamp(target.transform.position.y, min.y - 2, max.y + 2), _initialPos.z);
            }
            else
            {
                newPos = new Vector3(Mathf.Clamp(target.transform.position.x, min.x, max.x), Mathf.Clamp(target.transform.position.y, min.y, max.y), _initialPos.z);
            }
            transform.position = Vector3.Slerp(transform.position, newPos, offset * Time.deltaTime);
        }
        else
        {
            // Calcular el punto medio
            Vector3 middlePoint = (target.transform.position + _boss.transform.position) / 2f;
            middlePoint.z = transform.position.z; // Mantener la posición z de la cámara

            Vector3 newPos = new Vector3(Mathf.Clamp(middlePoint.x, min.x, max.x), Mathf.Clamp(middlePoint.y, min.y, max.y), middlePoint.z);
            transform.position = Vector3.Slerp(transform.position, newPos, offset * Time.deltaTime);

            // Calcular la distancia entre el jugador y el jefe
            float distance = Vector3.Distance(target.transform.position, _boss.transform.position);

            // Ajustar el tamaño de la cámara según la distancia
            float targetSize = Mathf.Clamp(distance, 7, 10);
            _cam.orthographicSize = Mathf.Lerp(_cam.orthographicSize, targetSize, Time.deltaTime * zoomSpeed);
        }

        if (_inChangeSizing)
        {
            _cam.orthographicSize = Mathf.Lerp(_cam.orthographicSize, _newPerspective, 1 * Time.deltaTime);

            if (_cam.orthographicSize == _newPerspective) _inChangeSizing = false;
        }
    }
    public static void SetDie()
    {
        _diePlayer = true;
        _cam.cullingMask = (1 << 5) | (1 << 9);
    }
    public static void SetCinematic(bool isOn)
    {
        _inCinematic = isOn;

        if (isOn)
        {
            StartCinematic();
            // EVITA ESTAS CAPAS ESPECÍFICAS
            _cam.cullingMask = ~LayerMask.GetMask("No-Cinematic") & ~LayerMask.GetMask("UI");
        }
        else
        {
            EndCinematic();
            _cam.cullingMask = -1;
        }
    }
    // ---- SHAKE ---- //
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
    // ---- SIZING ---- //
    public static void SetSize(SizeCamera type)
    {
        _inChangeSizing = true;

        switch (type)
        {
            case SizeCamera.specific: _newPerspective = 4; break;
            case SizeCamera.normal: _newPerspective = 5; break;
            case SizeCamera.boss: _boss = FindAnyObjectByType<BossSystem>(); break;
        }
    }
    public static void CancelSize() { _newPerspective = 5; }
    public static void SetMinMax(Vector2 minValues, Vector2 maxValues)
    {
        min = minValues;
        max = maxValues;
    }
    // ---- CINEMATIC ---- //
    public static void StartCinematic() { _overlayOBJ.SetActive(true); }
    public static void EndCinematic() { _overlayOBJ.SetActive(false); }
    // ---- CALLER ---- //
    public static void CallCamera(Vector2 pos, float time)
    {
        prevPosCamera = _cam.transform.position;
        timeToCall = time;
        posCaller = pos;
        inCall = true;
    }
    public static void CancelCallCamera()
    {
        posCaller = Vector2.zero;
        inCall = false;
        timeToCall = 0;
    }
}