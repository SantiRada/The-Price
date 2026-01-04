using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sistema de Object Pooling para proyectiles de armas de distancia.
/// Evita la creación constante de proyectiles mejorando el rendimiento.
/// </summary>
public class ProjectilePool : MonoBehaviour
{
    private static ProjectilePool _instance;
    public static ProjectilePool Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject poolObject = new GameObject("ProjectilePool");
                _instance = poolObject.AddComponent<ProjectilePool>();
                DontDestroyOnLoad(poolObject);
            }
            return _instance;
        }
    }

    // Diccionario de pools por tipo de proyectil (usando el prefab como clave)
    private Dictionary<GameObject, Queue<Projectile>> _pools = new Dictionary<GameObject, Queue<Projectile>>();

    // Tamaño inicial de cada pool
    private const int INITIAL_POOL_SIZE = 10;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Obtiene un proyectil del pool o crea uno nuevo si no hay disponibles
    /// </summary>
    public Projectile GetProjectile(GameObject projectilePrefab, Vector3 position, Quaternion rotation)
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("[ProjectilePool] Prefab de proyectil es null");
            return null;
        }

        // Si no existe un pool para este tipo de proyectil, créalo
        if (!_pools.ContainsKey(projectilePrefab))
        {
            _pools[projectilePrefab] = new Queue<Projectile>();
            PrewarmPool(projectilePrefab, INITIAL_POOL_SIZE);
        }

        Projectile projectile;

        // Intenta obtener un proyectil inactivo del pool
        if (_pools[projectilePrefab].Count > 0)
        {
            projectile = _pools[projectilePrefab].Dequeue();
            projectile.transform.position = position;
            projectile.transform.rotation = rotation;
            projectile.gameObject.SetActive(true);
        }
        else
        {
            // Si no hay disponibles, crea uno nuevo
            GameObject newObj = Instantiate(projectilePrefab, position, rotation, transform);
            projectile = newObj.GetComponent<Projectile>();
            if (projectile == null)
            {
                Debug.LogError($"[ProjectilePool] El prefab {projectilePrefab.name} no tiene componente Projectile");
                Destroy(newObj);
                return null;
            }
            projectile.SetPool(this, projectilePrefab);
        }

        return projectile;
    }

    /// <summary>
    /// Devuelve un proyectil al pool para reutilización
    /// </summary>
    public void ReturnProjectile(GameObject prefab, Projectile projectile)
    {
        if (projectile == null || prefab == null) return;

        if (!_pools.ContainsKey(prefab))
        {
            _pools[prefab] = new Queue<Projectile>();
        }

        projectile.gameObject.SetActive(false);
        _pools[prefab].Enqueue(projectile);
    }

    /// <summary>
    /// Pre-calienta el pool creando proyectiles de antemano
    /// </summary>
    private void PrewarmPool(GameObject prefab, int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(prefab, transform);
            Projectile projectile = obj.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectile.SetPool(this, prefab);
                obj.SetActive(false);
                _pools[prefab].Enqueue(projectile);
            }
            else
            {
                Debug.LogError($"[ProjectilePool] El prefab {prefab.name} no tiene componente Projectile");
                Destroy(obj);
            }
        }
    }

    /// <summary>
    /// Limpia todos los pools
    /// </summary>
    public void ClearAllPools()
    {
        foreach (var pool in _pools.Values)
        {
            while (pool.Count > 0)
            {
                Projectile projectile = pool.Dequeue();
                if (projectile != null)
                {
                    Destroy(projectile.gameObject);
                }
            }
        }
        _pools.Clear();
    }
}
