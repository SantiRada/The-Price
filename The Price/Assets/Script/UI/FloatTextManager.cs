using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum TypeColor
{
    PV = 0, Concentracion = 1, VelocidadDeMovimiento = 2, VelocidadDeAtaque = 3, SkillDamage = 4, Damage = 5, SubsequenceDamage = 6, CriticalChance = 7, MissChance = 8, StealingHP = 9, sanity = 10,
    cura = 11, receivedDamage = 12, Crazy = 13
}
public class FloatTextManager : MonoBehaviour {

    public GameObject objPrefab;
    public GameObject objCrazyPrefab;
    public List<Color> colorsPool = new List<Color>();

    [Header("Valores Estáticos")]
    public static List<Color> colors = new List<Color>();
    public static TextMeshProUGUI _textPrefab, _textCrazyPrefab;
    public static Transform _transform;

    private void Start()
    {
        _textPrefab = objPrefab.GetComponent<TextMeshProUGUI>();
        _textCrazyPrefab = objCrazyPrefab.GetComponent<TextMeshProUGUI>();
        _transform = transform;
        colors = colorsPool;
    }
    public static Color GetColor(TypeFlair type) { return colors[(int)type]; }
    public static void CreateText(Vector2 position, TypeColor type, string content, bool isCrazy = false, bool isWord = false)
    {
        FloatText text;

        // VERIFY TYPE TEXT AND CREATE
        if (isCrazy) { text = Instantiate(_textCrazyPrefab.gameObject, CalculatePos(position, isWord), Quaternion.identity, _transform).GetComponent<FloatText>(); }
        else { text = Instantiate(_textPrefab.gameObject, CalculatePos(position, isWord), Quaternion.identity, _transform).GetComponent<FloatText>(); }

        // CHANGE MOVE
        text.isStatic = isWord;

        // CHANGE VALUES
        if (isWord) content = LanguageManager.GetValue("Game", int.Parse(content));
        TextMeshProUGUI textInScene = text.GetComponent<TextMeshProUGUI>();

        // APPLY VALUES
        textInScene.color = colors[(int)type];
        textInScene.text = content;
    }
    // ---- FUNCIÓN INTEGRA ---- //
    public static Vector2 CalculatePos(Vector2 pos, bool isWord = false)
    {
        float rndX;
        float rndY;

        if (!isWord)
        {
            rndX = Random.Range(-0.5f, 0.5f);

            rndY = 0.5f;
        }
        else
        {
            do { rndX = Random.Range(-1f, 1f); } while (rndX > -.5f && rndX < .5f);

            rndY = Random.Range(-1f, 1f);
        }

        pos.x += rndX;
        pos.y += rndY;

        return pos;
    }
}
