using UnityEngine;

public enum Worlds { Terrenal, Cielo, Infierno, Astral, Inframundo }
public class DeadSystem : MonoBehaviour {

    [Header("Prev Data")]
    public int terrenal, celestial, infernal, astral, subterrenal;

    [Header("Current Data")]
    public Worlds currentWorld;
    public Worlds nextWorld;

    private void Start()
    {

    }
}
