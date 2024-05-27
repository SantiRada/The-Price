using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LanguageManager : MonoBehaviour {

    [Header("Data CSV")]
    [SerializeField] private TextAsset _menuFile;
    [SerializeField] private TextAsset _gameFile;
    [SerializeField] private TextAsset _skillFile;
    [SerializeField] private char _delimiter = ',';
    private static string[,] _menuData, _gameData, _skillData;

    [Header("Data Result")]
    [SerializeField] private TextMeshProUGUI[] _allText;
    [SerializeField] private Text[] _allLabel;
    [HideInInspector] public static int columnLanguage = 1;

    private void OnEnable()
    {
        InitialValues();
    }
    private void InitialValues()
    {
        LoadCSV();
        LoadAllText();

        columnLanguage = PlayerPrefs.GetInt("Language", 1);
        UpdateLanguage(columnLanguage);
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
    private void LoadCSV()
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
    }
    public void UpdateLanguage(int num)
    {
        columnLanguage = num;

        for(int i = 0; i < _allText.Length; i++)
        {
            string[] dataText = _allText[i].name.Split('[');
            string[] dataValues = dataText[1].Split(']');
            string[] dataFinal = dataValues[0].Split(',');
            // dataFinal[0] = "Menu" && dataFinal[1] = 1

            _allText[i].text = GetValue(dataFinal[0], int.Parse(dataFinal[1])).ToString();
        }

        for (int i = 0; i < _allLabel.Length; i++)
        {
            string[] dataText = _allLabel[i].name.Split('[');
            string[] dataValues = dataText[1].Split(']');
            string[] dataFinal = dataValues[0].Split(',');
            // dataFinal[0] = "Menu" && dataFinal[1] = 1

            _allLabel[i].text = GetValue(dataFinal[0], int.Parse(dataFinal[1])).ToString();
        }
    }
    public static string GetValue(string list, int rowIndex)
    {
        list = list.ToLower();
        switch (list)
        {
            case "menu": return _menuData[(rowIndex - 1), columnLanguage];
            case "skill": return _skillData[(rowIndex - 1), columnLanguage];

            default: return _gameData[(rowIndex - 1), columnLanguage];
        }
    }
}