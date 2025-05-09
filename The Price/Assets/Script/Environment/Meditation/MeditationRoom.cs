using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MeditationRoom : MonoBehaviour {

    [Header("Content Base")]
    public GameObject posPlayer;
    public TextMeshProUGUI textTimer;
    public GameObject[] doors;
    public GameObject[] posDoors;

    [Header("Private Data")]
    private bool isMeditation;
    private float time;
    private List<GameObject> doorsInScene = new List<GameObject>();

    [Header("Private Callers")]
    private MeditateSector _meditateSector;
    private DeadSystem _deadSystem;

    private void OnEnable() { _deadSystem = FindAnyObjectByType<DeadSystem>(); }
    private void Start()
    {
        CreateDoors();
        InitialValues();
    }
    private void InitialValues()
    {
        textTimer.text = "";
        isMeditation = false;
        textTimer.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (Pause.Comprobation(State.Game)) return;

        if (isMeditation)
        {
            time -= Time.deltaTime;
            textTimer.text = (time - 5).ToString("f0");

            if ((time - 5) <= 0) { EndMeditation(); }
        }
    }
    public void StartMeditation(float timer, MeditateSector obj)
    {
        time = timer;
        isMeditation = true;
        _meditateSector = obj;
        textTimer.gameObject.SetActive(true);

        StartCoroutine("StartChanges");
    }
    private IEnumerator StartChanges()
    {
        yield return new WaitForSeconds(1f);

        PlayerStats _player = _deadSystem.GetComponent<PlayerStats>();

        for(int i = 0; i< (_player.GetterStats(0, true) - _player.GetterStats(0, false)); i++)
        {
            _player.SetValue(0, 1, false);
            yield return new WaitForSeconds(0.5f);
        }

        // APLICAR CAMBIOS EN CORDURA
        FloatTextManager.CreateText(_player.transform.position, TypeColor.sanity, "12", false, true);
        _player.SetValue(10, 10, false);
    }
    private void EndMeditation()
    {
        InitialValues();

        _meditateSector.StartCoroutine("EndMeditate");
    }
    private void CreateDoors()
    {
        for(int i = 0; i < 4; i++)
        {
            DoorSystem doorData = Instantiate(doors[i], posDoors[i].transform.position, Quaternion.identity, transform).GetComponent<DoorSystem>();

            switch (doorData.whereItTakesMe)
            {
                case Worlds.Terrenal: if (_deadSystem.wasInTerrenal > 0) doorData.isActive = true; else doorData.isActive = false; break;
                case Worlds.Cielo: if (_deadSystem.wasInCielo > 0) doorData.isActive = true; else doorData.isActive = false; break;
                case Worlds.Infierno: if (_deadSystem.wasInInfierno > 0) doorData.isActive = true; else doorData.isActive = false; break;
                case Worlds.Inframundo: if (_deadSystem.wasInInframundo > 0) doorData.isActive = true; else doorData.isActive = false; break;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) { EndMeditation(); }
    }
}