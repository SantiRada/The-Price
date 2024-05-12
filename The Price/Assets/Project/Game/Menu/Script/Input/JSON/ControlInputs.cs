using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ControlInputs : MonoBehaviour {

    [Header("Data JSON")]
    [SerializeField] private string _dataPlayer;
    [SerializeField] private InputDataPlayer _inputs = new InputDataPlayer();

    private InputManager _inputManager;

    private void Awake()
    {
        _dataPlayer = Application.dataPath + "/Inputs.json";
    }
    private void Start()
    {
        Invoke("LoadData", 1f);
    }
    // ---- JSON ---- //
    public void SaveData()
    {
        InputDataPlayer newData = new InputDataPlayer()
        {
            keyboardData = new List<string>(_inputManager.GetInputsForControl(0)),
            gamepadData = new List<string>(_inputManager.GetInputsForControl(1)),
        };
        string stringJSON = JsonUtility.ToJson(newData);
        File.WriteAllText(_dataPlayer, stringJSON);
    }
    private void LoadData()
    {
        if (File.Exists(_dataPlayer))
        {
            string contain = File.ReadAllText(_dataPlayer);
            _inputs = JsonUtility.FromJson<InputDataPlayer>(contain);

            ChangedData(_inputs);
        }
    }
    private void ChangedData(InputDataPlayer values)
    {
        List<string> keyboardData = new List<string>(values.keyboardData);
        _inputManager.SetInputsForControl(0, keyboardData);

        List<string> gamepadData = new List<string>(values.gamepadData);
        _inputManager.SetInputsForControl(1, gamepadData);
    }
}
