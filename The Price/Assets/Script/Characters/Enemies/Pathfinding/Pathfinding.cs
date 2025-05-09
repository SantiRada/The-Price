using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour {

    [Header("References")]
    private WalkableMapGenerator mapGenerator;

    [Header("Private Data")]
    private int width;
    private int height;
    private Node[,] grid;

    private void Start()
    {
        mapGenerator = GetComponent<WalkableMapGenerator>();
    }
    public List<Node> FindPath(Vector2Int start, Vector2Int target, TypeNode[,] walkableMap)
    {
        if (mapGenerator == null)
        {
            Debug.LogError("Pathfinding: WalkableMapGenerator no asignado.");
            return null;
        }

        // Tamaño del mapa
        width = walkableMap.GetLength(0);
        height = walkableMap.GetLength(1);
        grid = new Node[width, height];

        // Usar InitialPosition como offset real
        Vector2Int offset = new Vector2Int(
            Mathf.RoundToInt(mapGenerator.InitialPosition.x),
            Mathf.RoundToInt(mapGenerator.InitialPosition.y)
        );

        // Crear nodos con posiciones globales
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int worldPos = new Vector2Int(offset.x + x, offset.y + y);
                grid[x, y] = new Node(worldPos, walkableMap[x, y]);
            }
        }

        // Convertir start/target a coordenadas locales del grid
        Vector2Int startGridPos = start - offset;
        Vector2Int targetGridPos = target - offset;

        // Validación de límites
        if (!IsInBounds(startGridPos) || !IsInBounds(targetGridPos))
        {
            Debug.LogWarning($"Pathfinding: Coordenadas fuera de rango. StartGrid: {startGridPos}, TargetGrid: {targetGridPos}, Offset: {offset}");
            return null;
        }

        Node startNode = grid[startGridPos.x, startGridPos.y];
        Node targetNode = grid[targetGridPos.x, targetGridPos.y];

        // Algoritmo A*
        List<Node> openSet = new List<Node> { startNode };
        HashSet<Node> closedSet = new HashSet<Node>();

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost ||
                    (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (Node neighbor in GetNeighbors(currentNode, offset))
            {
                if (!neighbor.isWalkable || closedSet.Contains(neighbor))
                    continue;

                int newCost = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newCost < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newCost;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return null; // No path found
    }
    private List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node current = endNode;

        while (current != startNode)
        {
            path.Add(current);
            current = current.parent;
        }

        path.Reverse();
        return path;
    }
    private List<Node> GetNeighbors(Node node, Vector2Int offset)
    {
        List<Node> neighbors = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.position.x + x - offset.x;
                int checkY = node.position.y + y - offset.y;

                if (IsInBounds(checkX, checkY))
                {
                    neighbors.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbors;
    }
    private bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }
    private bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }
    private int GetDistance(Node a, Node b)
    {
        int dx = Mathf.Abs(a.position.x - b.position.x);
        int dy = Mathf.Abs(a.position.y - b.position.y);
        return dx > dy ? 14 * dy + 10 * (dx - dy) : 14 * dx + 10 * (dy - dx);
    }
}