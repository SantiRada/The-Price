using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LunarCycle : MonoBehaviour {

    [Header("Content UI")]
    public CanvasGroup canvas;
    public Image[] planets;
    public Image cross;

    [Header("Data")]
    public int countIteration;
    [Range(0, 0.25f)] public float delayBetweenIter;
    private static int countRoom = 0;
    public static bool isActive;

    private void Start() { isActive = false; }
    public static int CalculateNextWorld()
    {
        if(countRoom % 4 == 0 && countRoom != 0 && isActive) return (countRoom / 4);
        else return -1;
    }
    private void Update()
    {
        if (isActive) canvas.alpha = 1;
        else canvas.alpha = 0;
    }
    public IEnumerator AddRoom()
    {        
        countRoom++;

        yield return new WaitForSeconds(0.15f);

        // La cantidad total de rotación que queremos aplicar a cada planeta
        float[] totalRotations = { -90f, -180f, -360f };
        float crossRotation = (90f / 4f) / countIteration;

        // La cantidad de rotación por iteración para cada planeta
        float[] rotationsPerIteration = new float[planets.Length];

        for (int i = 0; i < planets.Length; i++)
        {
            rotationsPerIteration[i] = totalRotations[i] / countIteration;
        }

        for (int i = 0; i < countIteration; i++)
        {
            for (int j = 0; j < planets.Length; j++)
            {
                planets[j].transform.Rotate(0, 0, rotationsPerIteration[j]);
            }

            cross.transform.Rotate(0, 0, crossRotation);

            yield return new WaitForSeconds(delayBetweenIter);
        }
    }
}
