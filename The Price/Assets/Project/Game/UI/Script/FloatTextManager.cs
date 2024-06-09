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
    public static void CreateText(Vector2 position, TypeColor type, string content, bool isCrazy = false)
    {
        TextMeshProUGUI text;

        if (isCrazy) text = Instantiate(_textCrazyPrefab.gameObject, CalculatePos(position), Quaternion.identity, _transform).GetComponent<TextMeshProUGUI>();
        else text = Instantiate(_textPrefab.gameObject, CalculatePos(position), Quaternion.identity, _transform).GetComponent<TextMeshProUGUI>();

        text.color = colors[(int)type];
        text.text = content;
    }
    // ---- FUNCIÓN INTEGRA ---- //
    public static Vector2 CalculatePos(Vector2 pos)
    {
        float rndX;
        do { rndX = Random.Range(-1f, 1f); } while (rndX > -.5f && rndX < .5f);

        float rndY = Random.Range(-1.5f, 0f);

        pos.x += rndX;
        pos.y += rndY;

        return pos;
    }
}
