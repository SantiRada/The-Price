using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TypeNode { walkable, unwalkable, undefined }
public class WalkableMapGenerator : MonoBehaviour {

    [Header("Map Data")]
    public bool manualMap;
    [SerializeField] private Vector3 initPos;
    [SerializeField] private Vector2Int mapSize;
    public TypeNode[,] walkableMap; // Mapa de walkable
    [Space]
    [SerializeField] private LayerMask walkableLayer;
    [SerializeField] private LayerMask unwalkableLayer;

    public event Action completeMap;

    private void Start()
    {
        if (manualMap) GenerateWalkableMap();
    }
    public void CompleteMap()
    {
        completeMap?.Invoke();
    }
    public bool GetPositionIsWalkable(int x, int y) { return walkableMap[x, y] == TypeNode.walkable ? true : false; }
    public void GenerateWalkableMap()
    {
        if (walkableMap != null) walkableMap = null;
        walkableMap = new TypeNode[mapSize.x, mapSize.y];

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                Vector3 worldPoint = new Vector3((initPos.x + x), (initPos.y + y), 0);

                // Inicializa las variables de detección
                bool tilemapWalkable = false;
                bool colliderWalkable = true;

                // Realiza la detección de colisiones con los Tilemaps
                Tilemap[] tilemaps = FindObjectsByType<Tilemap>(FindObjectsSortMode.None);

                foreach (Tilemap tilemap in tilemaps)
                {
                    Vector3Int cellPosition = tilemap.WorldToCell(worldPoint);
                    if (tilemap.HasTile(cellPosition))
                    {
                        if (((1 << tilemap.gameObject.layer) & walkableLayer) != 0) tilemapWalkable = true;
                        break;
                    }
                }

                // Realiza la detección de colisiones con el resto de objetos
                if (Physics2D.OverlapPoint(worldPoint, unwalkableLayer)) { colliderWalkable = false; }

                // Asigna el valor walkable basado en los resultados de la detección
                if (tilemapWalkable) walkableMap[x, y] = TypeNode.walkable;
                if(!tilemapWalkable || !colliderWalkable) walkableMap[x, y] = TypeNode.unwalkable;
            }
        }

        completeMap?.Invoke();
    }
    public Vector2Int SizeMap
    {
        get { return mapSize; }
        set { mapSize = value; }
    }
    public Vector3 InitialPosition
    {
        get { return initPos; }
        set { initPos = value; }
    }

    #region Gizmos
    
    private void OnDrawGizmos()
    {
        if (walkableMap == null) return;
        
        Gizmos.color = Color.red;
        for (int x = 0; x < SizeMap.x; x++)
        {
            for (int y = 0; y < SizeMap.y; y++)
            {
                Vector3 worldPoint = new Vector3((initPos.x + x), (initPos.y + y), 0);

                if (walkableMap[x, y] == TypeNode.unwalkable)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawCube(worldPoint, new Vector3(0.9f, 0.9f, 0.9f));
                }
                else if (walkableMap[x, y] == TypeNode.walkable)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawCube(worldPoint, new Vector3(0.9f, 0.9f, 0.9f));
                }
            }

        }
    }
    
    #endregion
}