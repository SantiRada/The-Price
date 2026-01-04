using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LanguageManager : MonoBehaviour {

    [Header("Data CSV")]
    [SerializeField] private TextAsset _menuFile;
    [SerializeField] private TextAsset _gameFile;
    [SerializeField] private TextAsset _skillFile;
    [SerializeField] private TextAsset _objectFile;
    [SerializeField] private char _delimiter = ',';
    private static string[,] _menuData, _gameData, _skillData, _objectData;

    [Header("Data Result")]
    public static bool _styleTDAH = false;
    private TextMeshProUGUI[] _allText;
    private Text[] _allLabel;
    [HideInInspector] public static int language = 1;

    private void Awake()
    {
        InitialValues();
    }
    private void InitialValues()
    {
        _styleTDAH = false;

        LoadCSV();
        LoadAllText();

        language = 1;
        UpdateLanguage(language);
    }
    private void LoadAllText()
    {
        // TEXT.MESH.PRO
        TextMeshProUGUI[] allTextElements = FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);
        List<TextMeshProUGUI> textElementsWithName = new List<TextMeshProUGUI>();

        foreach (TextMeshProUGUI textElement in allTextElements)
        {
            if (textElement.name.Contains("Text")) textElementsWithName.Add(textElement);
        }

        _allText = textElementsWithName.ToArray();

        // LABELS - TEXT
        Text[] allLabelElements = FindObjectsByType<Text>(FindObjectsSortMode.None);
        List<Text> labelElementsWithName = new List<Text>();

        foreach (Text labelElement in allLabelElements)
        {
            if (labelElement.name.Contains("Text")) labelElementsWithName.Add(labelElement);
        }

        _allLabel = labelElementsWithName.ToArray();
    }
    public void LoadCSV()
    {
        // SEPARA EL CSV DEL MENU
        string[] linesMenu = _menuFile.text.Split('\n');
        _menuData = new string[linesMenu.Length, 3];

        for (int i = 0; i < linesMenu.Length; i++)
        {
            string[] columns = linesMenu[i].Split(_delimiter);

            for (int j = 0; j < columns.Length; j++)
            {
                _menuData[i, j] = columns[j];
            }
        }

        // SEPARA EL CSV DE JUEGO
        string[] linesGame = _gameFile.text.Split('\n');
        _gameData = new string[linesGame.Length, 3];

        for (int i = 0; i < linesGame.Length; i++)
        {
            string[] columns = linesGame[i].Split(_delimiter);

            for (int j = 0; j < columns.Length; j++)
            {
                _gameData[i, j] = columns[j];
            }
        }

        // SEPARA EL CSV DE SKILLS
        string[] linesSkill = _skillFile.text.Split('\n');
        _skillData = new string[linesSkill.Length, 3];

        for (int i = 0; i < linesSkill.Length; i++)
        {
            string[] columns = linesSkill[i].Split(_delimiter);

            for (int j = 0; j < columns.Length; j++)
            {
                _skillData[i, j] = columns[j];
            }
        }

        // SEPARA EL CSV DE OBJECTS
        string[] linesObject = _objectFile.text.Split('\n');
        _objectData = new string[linesObject.Length, 3];

        for (int i = 0; i < linesObject.Length; i++)
        {
            string[] columns = linesObject[i].Split(_delimiter);

            for (int j = 0; j < columns.Length; j++)
            {
                _objectData[i, j] = columns[j];
            }
        }
    }
    public void UpdateLanguage(int pos)
    {
        language = pos;

        for(int i = 0; i < _allText.Length; i++)
        {
            string[] dataText = _allText[i].name.Split('[');
            string[] dataValues = dataText[1].Split(']');
            string[] dataFinal = dataValues[0].Split(',');
            // dataFinal[0] = "Menu" && dataFinal[1] = 1

            _allText[i].text = GetValue(dataFinal[0], int.Parse(dataFinal[1])).ToString();
            // Debug.Log(dataFinal[0] + "-" + dataFinal[1] + ": " + GetValue(dataFinal[0], int.Parse(dataFinal[1])));
        }

        for (int i = 0; i < _allLabel.Length; i++)
        {
            string[] dataText = _allLabel[i].name.Split('[');
            string[] dataValues = dataText[1].Split(']');
            string[] dataFinal = dataValues[0].Split(',');
            // dataFinal[0] = "Menu" && dataFinal[1] = 1

            _allLabel[i].text = GetValue(dataFinal[0], int.Parse(dataFinal[1])).ToString();
            // Debug.Log(dataFinal[0] + "-" + dataFinal[1] + ": " + GetValue(dataFinal[0], int.Parse(dataFinal[1])));
        }
    }
    public static string GetValue(string list, int rowIndex)
    {
        string data = "";
        list = list.ToLower();

        // Validación de índice
        int arrayIndex = rowIndex - 1;

        switch (list)
        {
            case "menu":
                if (arrayIndex < 0 || arrayIndex >= _menuData.GetLength(0))
                {
                    Debug.LogWarning($"LanguageManager: Index {rowIndex} fuera de rango para 'menu' (max: {_menuData.GetLength(0)})");
                    return $"[MENU:{rowIndex}]";
                }
                if (_styleTDAH) data = _menuData[arrayIndex, language];
                else return _menuData[arrayIndex, language];
                break;
            case "skill":
                if (arrayIndex < 0 || arrayIndex >= _skillData.GetLength(0))
                {
                    Debug.LogWarning($"LanguageManager: Index {rowIndex} fuera de rango para 'skill' (max: {_skillData.GetLength(0)})");
                    return $"[SKILL:{rowIndex}]";
                }
                if(_styleTDAH) data = _skillData[arrayIndex, language];
                else return _skillData[arrayIndex, language];
                break;
            case "object":
                if (arrayIndex < 0 || arrayIndex >= _objectData.GetLength(0))
                {
                    Debug.LogWarning($"LanguageManager: Index {rowIndex} fuera de rango para 'object' (max: {_objectData.GetLength(0)})");
                    return $"[OBJECT:{rowIndex}]";
                }
                if (_styleTDAH) data = _objectData[arrayIndex, language];
                else return _objectData[arrayIndex, language];
                break;
            default:
                if (arrayIndex < 0 || arrayIndex >= _gameData.GetLength(0))
                {
                    Debug.LogWarning($"LanguageManager: Index {rowIndex} fuera de rango para 'game' (max: {_gameData.GetLength(0)})");
                    return $"[GAME:{rowIndex}]";
                }
                if(_styleTDAH) data = _gameData[arrayIndex, language];
                else return _gameData[arrayIndex, language];
                break;
        }

        string pattern = @"(?<!^)(?=[A-Z])|(?<=\s)";
        string[] words = Regex.Split(data, pattern);

        string[] separate = data.Split(" ");
        data = "";
        for (int i = 0; i < separate.Length; i++)
        {
            if (!string.IsNullOrEmpty(words[i]))
            {
                if (separate[i].Length > 3) separate[i] = $"<b><color=white>{separate[i].Substring(0, 3)}</color></b>{separate[i].Substring(3)}";
                else separate[i] = $"<b><color=white>{separate[i].Substring(0, 1)}</color></b>{separate[i].Substring(1)}";

                data += separate[i];
            }
        }

        return data;
    }
}