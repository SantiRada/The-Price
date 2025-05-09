using UnityEngine;

public class Node {

    public Vector2Int position;
    public bool isWalkable;
    public int gCost;
    public int hCost;
    public Node parent;

    public int fCost { get { return gCost + hCost; } }

    public Node(Vector2Int _position, TypeNode _isWalkable)
    {
        position = _position;
        if (_isWalkable != TypeNode.undefined) isWalkable = _isWalkable == TypeNode.walkable ? true : false;
    }
}
