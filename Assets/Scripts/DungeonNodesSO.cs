using System;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonNodesSo", menuName = "ScriptableObjects/DungeonNodesSo")]   
public class DungeonNodesSo : ScriptableObject
{
    public Nodes Nodes;
    
}

public class Node
{
    public GameObject NodeGameobject;
    protected Vector2Int Direction = new Vector2Int(0,0);
    public Vector2Int GetDirection()
    {
        return Direction;
    }
}

//Rooms
[Serializable] public class CenterNodes : Node { public CenterNodes() { Direction = new Vector2Int(0, 0); }}
[Serializable] public class UpNodes : Node { public UpNodes() { Direction = new Vector2Int(0, 1); } }
[Serializable] public class DownNodes : Node { public DownNodes() { Direction = new Vector2Int(0,-1); } }
[Serializable] public class RightNodes : Node { public RightNodes() { Direction = new Vector2Int(1,0); } }
[Serializable] public class LeftNodes : Node { public LeftNodes() { Direction = new Vector2Int(-1,0); } }

//Corridors
[Serializable] public class UpDownNodes : Node { public readonly Vector2Int Direction = new Vector2Int(-1,0); }
[Serializable] public class UpRightNodes : Node { public readonly Vector2Int Direction = new Vector2Int(-1,0); }
[Serializable] public class UpLeftNodes : Node { public readonly Vector2Int Direction = new Vector2Int(-1,0); }
[Serializable] public class DownLeft : Node { public readonly Vector2Int Direction = new Vector2Int(-1,0); }
[Serializable] public class DownRight : Node { public readonly Vector2Int Direction = new Vector2Int(-1,0); }
[Serializable] public class RightLeft : Node { public readonly Vector2Int Direction = new Vector2Int(-1,0); }