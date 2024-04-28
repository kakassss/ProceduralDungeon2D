using UnityEngine;

public class NodeData<T>
{
    public T node;
    public int PosX;
    public int PosY;
    
    public Vector2Int Position;
    public bool IsEmpty;
    
    public NodeData(T node, int posX, int posY)
    {
        this.node = node;
        PosX = posX;
        PosY = posY;
        Position = new Vector2Int(PosX, PosY);

    }

    public NodeData(int posX, int posY)
    {
        PosX = posX;
        PosY = posY;
        Position = new Vector2Int(PosX, PosY);
    }
}