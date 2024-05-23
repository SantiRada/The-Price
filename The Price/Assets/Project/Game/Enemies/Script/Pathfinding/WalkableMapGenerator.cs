using System.Collections;
using UnityEngine;

public class WalkableMapGenerator : MonoBehaviour {

    [Header("Map Data")]
    [SerializeField] private Vector2Int mapSize;
    public bool[,] walkableMap; // Mapa de walkable
    [Space]
    [SerializeField] private LayerMask walkableLayer;
    [SerializeField] private LayerMask unwalkableLayer;

    public void GenerateWalkableMap()
    {
        StartCoroutine("CreateMap");
    }
    private IEnumerator CreateMap()
    {
        walkableMap = new bool[mapSize.x, mapSize.y];

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                Vector3 worldPoint = new Vector3((transform.position.x + x), (transform.position.y + y), 0);

                if (Physics2D.OverlapPoint(worldPoint, unwalkableLayer))
                {
                    walkableMap[x, y] = false;
                    Debug.Log("No Walkable");
                }
                else if (Physics2D.OverlapPoint(worldPoint, walkableLayer))
                {
                    walkableMap[x, y] = true;
                    Debug.Log("Walkable");
                }
                else
                {
                    // Si no se encuentra ni en walkable ni en unwalkable, se considera no caminable
                    walkableMap[x, y] = false;
                    Debug.Log("Undefined");
                }
                Debug.Log("Corroborando: (" + worldPoint.x + ", " + worldPoint.y + ")");
                yield return new WaitForSeconds(.2f);
            }
        }
    }
    private void OnDrawGizmos()
    {
        if (walkableMap == null)
            return;

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                Vector3 worldPoint = new Vector3((transform.position.x + x), (transform.position.y + y), 0);
                Gizmos.color = walkableMap[x, y] ? Color.green : Color.red;
                Gizmos.DrawCube(worldPoint, Vector3.one * 0.9f);
            }
        }
    }
}
