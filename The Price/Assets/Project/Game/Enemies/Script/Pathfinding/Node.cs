using UnityEngine;

public class Node {

    public Vector2Int position;
    public bool isWalkable;
    public int gCost;
    public int hCost;
    public Node parent;

    public int fCost { get { return gCost + hCost; } }

    public Node(Vector2Int _position, bool _isWalkable)
    {
        position = _position;
        isWalkable = _isWalkable;
    }
}
