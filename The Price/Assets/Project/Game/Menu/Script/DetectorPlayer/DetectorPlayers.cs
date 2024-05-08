using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DetectorPlayers : MonoBehaviour {

    [Header("Data Start Game")]
    [SerializeField] private float _timerToStartGame;
    private float _timerToStartBase;
    [HideInInspector] public bool canDetect = false;
    private bool delayToClic = false;


    [Header("Data Visual")]
    [SerializeField] private Image[] _playersOn = new Image[4];

    [Header("Data Players")]
    [SerializeField] private List<string> _detectorPlayers = new List<string>();

    private void Start()
    {
        _timerToStartBase = _timerToStartGame;

        _playersOn[0].color = Color.black;
        _playersOn[1].color = Color.black;
        _playersOn[2].color = Color.black;
        _playersOn[3].color = Color.black;
    }
    private void LateUpdate()
    {
        if (!canDetect) return;

        if (Input.anyKeyDown)
        {
            foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode))
                {
                    string test = keyCode.ToString();
                    bool inTest = false;

                    string[] data = test.Split("B");

                    if (test.Contains("Joystick"))
                    {
                        for (int i = 0; i < _detectorPlayers.Count; i++)
                        {
                            if (data[0] == _detectorPlayers[i])
                            {
                                inTest = true;
                                break;
                            }
                        }

                        if (!inTest)
                        {
                            char lastChar = data[0][data[0].Length - 1];

                            if (char.IsDigit(lastChar)) AddPlayer(data[0]);

                        }
                    }
                    else
                    {
                        for (int i = 0; i < _detectorPlayers.Count; i++)
                        {
                            if (_detectorPlayers[i].Contains("Keyboard") || _detectorPlayers[i].Contains("Mouse"))
                            {
                                inTest = true;
                                break;
                            }
                        }

                        if (!inTest) AddPlayer("Keyboard & Mouse");
                    }
                }
            }
        }

        if (delayToClic) _timerToStartGame -= Time.deltaTime;

        if(_timerToStartGame <= 0)
        {
            delayToClic = false;

            if (Input.GetButtonDown("Submit"))
            {
                PlayerPrefs.SetInt("countPlayers", _detectorPlayers.Count);

                string data = "";
                for(int i = 0; i < _detectorPlayers.Count; i++)
                {
                    data += _detectorPlayers[i] + ",";
                }

                PlayerPrefs.SetString("dataPlayers", data);
                SceneManager.LoadScene(1);
            }
        }
    }
    private void AddPlayer(string data)
    {
        delayToClic = true;
        _timerToStartGame = _timerToStartBase;
        _detectorPlayers.Add(data);
        _playersOn[_detectorPlayers.Count - 1].color = Color.gray;
    }
}
