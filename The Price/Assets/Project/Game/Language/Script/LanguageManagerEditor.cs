using UnityEngine;
using UnityEditor;

public enum TypeFile { Menu = 0, Game = 1, Skill = 2}

[CustomEditor(typeof(LanguageManager))]
public class LanguageManagerEditor : Editor {

    private int row = 2;
    [SerializeField, TextArea(5,5)] private string data = "";
    private TypeFile _type;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LanguageManager languageManager = (LanguageManager)target;
        FindAnyObjectByType<LanguageManager>().LoadCSV();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Previsualización del Texto", EditorStyles.boldLabel);

        row = EditorGUILayout.IntField("Posición", row);
        _type = (TypeFile)EditorGUILayout.EnumPopup("Archivo", _type);
        
        EditorGUILayout.Space();
        EditorStyles.textField.wordWrap = true;
        data = EditorGUILayout.TextArea(data, GUILayout.Height(100));


        if (GUILayout.Button("Previsualizar"))
        {
            string files = (int)_type == 0 ? "Menu" : (int)_type == 1 ? "Game" : "Skill";
            data = LanguageManager.GetValue(files, row) + "\n\n";
            if(row > 1) data += (row - 1).ToString() + ": " + LanguageManager.GetValue(files, (row - 1)) + "\n";
            
            if(LanguageManager.GetValue(files, (row + 1)) != null) data += (row + 1).ToString() + ": " + LanguageManager.GetValue(files, (row + 1)) + "\n";
            if (LanguageManager.GetValue(files, (row + 2)) != null) data += (row + 2).ToString() + ": " + LanguageManager.GetValue(files, (row + 2));

        }
    }
}
