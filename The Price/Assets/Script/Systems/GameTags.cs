using UnityEngine;

/// <summary>
/// Clase estática que centraliza todos los tags utilizados en el juego.
/// Evita el uso de strings mágicos y previene errores de tipeo.
/// </summary>
public static class GameTags
{
    // Tags de entidades
    public const string Player = "Player";
    public const string Enemy = "Enemy";
    public const string Boss = "Boss";

    // Tags de objetos
    public const string Projectile = "Proyectile";
    public const string Weapon = "Weapon";
    public const string Soul = "Soul";
    public const string Gold = "Gold";

    // Tags de interactables
    public const string Interactive = "Interactive";
    public const string Chest = "Chest";
    public const string Door = "Door";

    // Tags de ambiente
    public const string Ground = "Ground";
    public const string Wall = "Wall";
    public const string Obstacle = "Obstacle";

    /// <summary>
    /// Verifica si un GameObject tiene un tag específico de manera segura
    /// </summary>
    public static bool HasTag(this GameObject obj, string tag)
    {
        if (obj == null) return false;
        return obj.CompareTag(tag);
    }

    /// <summary>
    /// Verifica si un Component tiene un tag específico de manera segura
    /// </summary>
    public static bool HasTag(this Component component, string tag)
    {
        if (component == null || component.gameObject == null) return false;
        return component.CompareTag(tag);
    }
}
