using UnityEngine;

/// <summary>
/// Clase de utilidad que proporciona métodos seguros para buscar y validar componentes.
/// Previene NullReferenceException y proporciona logging útil para debugging.
/// </summary>
public static class ComponentHelper
{
    /// <summary>
    /// Busca un componente en el GameObject de forma segura
    /// </summary>
    public static bool TryGetComponentSafe<T>(this GameObject obj, out T component) where T : Component
    {
        component = null;
        if (obj == null)
        {
            Debug.LogWarning($"[ComponentHelper] Intentando obtener componente {typeof(T).Name} de un GameObject null");
            return false;
        }

        component = obj.GetComponent<T>();
        if (component == null)
        {
            Debug.LogWarning($"[ComponentHelper] No se encontró el componente {typeof(T).Name} en {obj.name}");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Busca un componente en el MonoBehaviour de forma segura
    /// </summary>
    public static bool TryGetComponentSafe<T>(this MonoBehaviour mono, out T component) where T : Component
    {
        component = null;
        if (mono == null || mono.gameObject == null)
        {
            Debug.LogWarning($"[ComponentHelper] Intentando obtener componente {typeof(T).Name} de un MonoBehaviour null");
            return false;
        }

        component = mono.GetComponent<T>();
        if (component == null)
        {
            Debug.LogWarning($"[ComponentHelper] No se encontró el componente {typeof(T).Name} en {mono.name}");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Busca un componente en el Collider2D de forma segura
    /// </summary>
    public static bool TryGetComponentSafe<T>(this Collider2D collider, out T component) where T : Component
    {
        component = null;
        if (collider == null || collider.gameObject == null)
        {
            Debug.LogWarning($"[ComponentHelper] Intentando obtener componente {typeof(T).Name} de un Collider2D null");
            return false;
        }

        component = collider.GetComponent<T>();
        if (component == null)
        {
            Debug.LogWarning($"[ComponentHelper] No se encontró el componente {typeof(T).Name} en {collider.name}");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Busca un componente en la escena de forma segura
    /// </summary>
    public static bool TryFindObjectSafe<T>(out T obj, string contextName = "") where T : UnityEngine.Object
    {
        obj = Object.FindAnyObjectByType<T>();

        if (obj == null)
        {
            string context = string.IsNullOrEmpty(contextName) ? "" : $" (desde {contextName})";
            Debug.LogWarning($"[ComponentHelper] No se encontró ningún objeto de tipo {typeof(T).Name} en la escena{context}");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Busca un componente en la escena sin warnings (para componentes opcionales)
    /// </summary>
    public static bool TryFindObjectQuiet<T>(out T obj) where T : UnityEngine.Object
    {
        obj = Object.FindAnyObjectByType<T>();
        return obj != null;
    }

    /// <summary>
    /// Valida que un array no sea null y tenga el índice especificado
    /// </summary>
    public static bool IsValidIndex<T>(this T[] array, int index, string arrayName = "array")
    {
        if (array == null)
        {
            Debug.LogWarning($"[ComponentHelper] El array '{arrayName}' es null");
            return false;
        }

        if (index < 0 || index >= array.Length)
        {
            Debug.LogWarning($"[ComponentHelper] Índice {index} fuera de rango en '{arrayName}' (longitud: {array.Length})");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Valida que una lista no sea null y tenga el índice especificado
    /// </summary>
    public static bool IsValidIndex<T>(this System.Collections.Generic.List<T> list, int index, string listName = "list")
    {
        if (list == null)
        {
            Debug.LogWarning($"[ComponentHelper] La lista '{listName}' es null");
            return false;
        }

        if (index < 0 || index >= list.Count)
        {
            Debug.LogWarning($"[ComponentHelper] Índice {index} fuera de rango en '{listName}' (cantidad: {list.Count})");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Obtiene un elemento de un array de forma segura (para tipos de referencia)
    /// </summary>
    public static T GetSafe<T>(this T[] array, int index, T defaultValue = default) where T : class
    {
        if (!array.IsValidIndex(index))
            return defaultValue;

        return array[index] ?? defaultValue;
    }

    /// <summary>
    /// Obtiene un elemento de un array de forma segura (para tipos de valor)
    /// </summary>
    public static T GetSafeValue<T>(this T[] array, int index, T defaultValue = default) where T : struct
    {
        if (!array.IsValidIndex(index))
            return defaultValue;

        return array[index];
    }

    /// <summary>
    /// Obtiene un elemento de una lista de forma segura (para tipos de referencia)
    /// </summary>
    public static T GetSafe<T>(this System.Collections.Generic.List<T> list, int index, T defaultValue = default) where T : class
    {
        if (!list.IsValidIndex(index))
            return defaultValue;

        return list[index] ?? defaultValue;
    }

    /// <summary>
    /// Obtiene un elemento de una lista de forma segura (para tipos de valor)
    /// </summary>
    public static T GetSafeValue<T>(this System.Collections.Generic.List<T> list, int index, T defaultValue = default) where T : struct
    {
        if (!list.IsValidIndex(index))
            return defaultValue;

        return list[index];
    }

    /// <summary>
    /// Ejecuta una acción solo si el objeto no es null
    /// </summary>
    public static void SafeInvoke<T>(this T obj, System.Action<T> action) where T : class
    {
        if (obj != null && action != null)
        {
            action(obj);
        }
    }

    /// <summary>
    /// Ejecuta una función solo si el objeto no es null, devolviendo un valor por defecto si es null
    /// </summary>
    public static TResult SafeInvoke<T, TResult>(this T obj, System.Func<T, TResult> func, TResult defaultValue = default) where T : class
    {
        if (obj != null && func != null)
        {
            return func(obj);
        }
        return defaultValue;
    }

    /// <summary>
    /// Verifica si un objeto de Unity es válido (no null y no destruido)
    /// </summary>
    public static bool IsValid(this UnityEngine.Object obj)
    {
        return obj != null && obj;
    }

    /// <summary>
    /// Destruye un GameObject de forma segura
    /// </summary>
    public static void DestroySafe(this GameObject obj, float delay = 0f)
    {
        if (obj != null && obj)
        {
            if (delay > 0)
                Object.Destroy(obj, delay);
            else
                Object.Destroy(obj);
        }
    }

    /// <summary>
    /// Destruye un Component de forma segura
    /// </summary>
    public static void DestroySafe(this Component component, float delay = 0f)
    {
        if (component != null && component)
        {
            if (delay > 0)
                Object.Destroy(component, delay);
            else
                Object.Destroy(component);
        }
    }
}
