using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public enum ContentUI { bigContent, skillContent, dialogueContent, shopContent, smallContent }
public enum ListContent { Menu, Game, Skill, Object }
public class InteractiveManager : MonoBehaviour {

    [Header("Window")]
    public List<Vector3> _offset = new List<Vector3>();
    public List<GameObject> _windowThatWillOpen = new List<GameObject>();
    [Space]
    public TextMeshProUGUI[] _nameContent;
    public TextMeshProUGUI[] _descContent;

    [Header("Shop")]
    public TextMeshProUGUI _typeContent;
    public TextMeshProUGUI _goldContent;

    [Header("Style")]
    public Color colorNames;

    [Header("Private Content")]
    private List<Vector3> _initPos = new List<Vector3>();

    private void Start()
    {
        for(int i = 0; i < _windowThatWillOpen.Count; i++) { _initPos.Add(_windowThatWillOpen[i].transform.position); }
    }
    public void LoadInfo(int index, string desc, Vector3 pos, string name, ListContent content, bool isFlair = false, string gold = "", int type = -1)
    {
        string cnt = content.ToString();

        _windowThatWillOpen[index].transform.position = (pos + _offset[index]);

        if (!isFlair)
        {
            string value = LanguageManager.GetValue(cnt, int.Parse(name));

            if(int.Parse(desc) != -1) _descContent[index].text = LanguageManager.GetValue(cnt, int.Parse(desc));
            else _descContent[index].text = value;

            if (index != 4) _nameContent[index].text = value;
            else if(int.Parse(desc) != -1) _descContent[index].text += " <color=#" + colorNames.ToHexString() + ">" + value + "</color>";
        }
        else
        {
            _nameContent[index].text = name;
            _descContent[index].text = desc;
        }

        if (type != -1) _typeContent.text = LanguageManager.GetValue("Game", type);
        if (gold != "") _goldContent.text = gold;
    }
    public void CloseWindow(ContentUI contentUI, bool obligatory = false)
    {
        if (!obligatory)
        {
            Interactive[] interac = FindObjectsByType<Interactive>(FindObjectsSortMode.None);
            bool inTrigger = false;

            for(int i = 0; i < interac.Length; i++)
            {
                if (interac[i].content == contentUI)
                {
                    if (interac[i].inTrigger)
                    {
                        inTrigger = true;
                        break;
                    }
                }
            }

            if (inTrigger) return;
        }

        _windowThatWillOpen[(int)contentUI].transform.position = _initPos[(int)contentUI];
    }
}
