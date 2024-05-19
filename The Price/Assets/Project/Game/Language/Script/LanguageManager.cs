using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LanguageManager : MonoBehaviour {

    [Header("Data CSV")]
    [SerializeField] private TextAsset csvFile;
    [SerializeField] private char delimiter = '-';
    private string[,] csvData;

    [Header("Data Result")]
    [SerializeField] private TextMeshProUGUI[] _allText;
    [SerializeField] private Text[] _allLabel;
    [HideInInspector] public int columnLanguage = 1;

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

        LoadingScreen.CountElement++;
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
        string[] lines = csvFile.text.Split('\n');
        csvData = new string[lines.Length, 3];

        for (int i = 0; i < lines.Length; i++)
        {
            string[] columns = lines[i].Split(delimiter);

            for (int j = 0; j < columns.Length; j++)
            {
                csvData[i, j] = columns[j];
            }
        }
    }
    public void UpdateLanguage(int num)
    {
        columnLanguage = num;

        for(int i = 0; i < _allText.Length; i++)
        {
            string[] dataText = _allText[i].name.Split('[');
            string[] dataFinal = dataText[1].Split(']');

            int rowValue = int.Parse(dataFinal[0]);
            _allText[i].text = GetValue(rowValue).ToString();
        }

        for (int i = 0; i < _allLabel.Length; i++)
        {
            string[] dataText = _allLabel[i].name.Split('[');
            string[] dataFinal = dataText[1].Split(']');

            int rowValue = int.Parse(dataFinal[0]);
            _allLabel[i].text = GetValue(rowValue).ToString();
        }
    }
    public string GetValue(int rowIndex)
    {
        return csvData[(rowIndex-1), columnLanguage];
    }
}