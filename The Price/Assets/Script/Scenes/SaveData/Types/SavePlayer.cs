using System.Collections.Generic;

/// <summary>
/// Datos del jugador para guardar/cargar.
/// Simplificado: 7 stats, 1 arma, máximo 2 habilidades.
/// </summary>
[System.Serializable]
public class SavePlayer
{
    // Stats máximas (7 total)
    public float maxPV;
    public float maxConcentracion;
    public float speedMovement;
    public float speedAttack;
    public float skillDamage;
    public float damage;
    public float criticalChance;

    // Stats actuales
    public float pv;
    public float concentracion;

    // Recursos
    public bool canHaveSouls;
    public int gold;
    public int souls;

    // Arma (solo 1)
    public int weaponID = -1; // -1 = sin arma

    // Habilidades y objetos
    public List<int> skills = new List<int>(); // Máximo 2
    public List<int> objects = new List<int>();
}
