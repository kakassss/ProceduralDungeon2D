using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonNodesSo", menuName = "ScriptableObjects/DungeonNodesSo")]   
public class DungeonNodesSo : ScriptableObject
{
    public Nodes Nodes;
    
}

public class Node2
{
    [HideInInspector] public Vector2Int Direction = new Vector2Int(0,0);
    public GameObject NodeGameobject;
    
    [HideInInspector] public int PosX;
    [HideInInspector] public int PosY;

    [HideInInspector] public Vector2Int Position;


    protected Node2(int posX, int posY)
    {
        PosX = posX;
        PosY = posY;
        Position = new Vector2Int(PosX, PosY);
    }
}

[Serializable]
public class CenterNodes2 : Node2
{
    public CenterNodes2(int posX, int posY) : base(posX, posY)
    {
        Direction = new Vector2Int(0, 0); 
    }
}

public class Node
{
    [HideInInspector] public Vector2Int Direction = new Vector2Int(0,0);
    public GameObject NodeGameobject;
}

//Rooms
[Serializable] public class CenterNodes : Node { public CenterNodes() { Direction = new Vector2Int(0, 0); }}
[Serializable] public class UpNodes : Node { public UpNodes() { Direction = new Vector2Int(0, 1); } }
[Serializable] public class DownNodes : Node { public DownNodes() { Direction = new Vector2Int(0,-1); } }
[Serializable] public class RightNodes : Node { public RightNodes() { Direction = new Vector2Int(1,0); } }
[Serializable] public class LeftNodes : Node { public LeftNodes() { Direction = new Vector2Int(-1,0); } }

//Corriodors
[Serializable] public class UpDownNodes : Node { public readonly Vector2Int Direction = new Vector2Int(-1,0); }
[Serializable] public class UpRightNodes : Node { public readonly Vector2Int Direction = new Vector2Int(-1,0); }
[Serializable] public class UpLeftNodes : Node { public readonly Vector2Int Direction = new Vector2Int(-1,0); }
[Serializable] public class DownLeft : Node { public readonly Vector2Int Direction = new Vector2Int(-1,0); }
[Serializable] public class DownRight : Node { public readonly Vector2Int Direction = new Vector2Int(-1,0); }
[Serializable] public class RightLeft : Node { public readonly Vector2Int Direction = new Vector2Int(-1,0); }


[Serializable]
public class Nodes
{
    public List<CenterNodes2> CenterNodes2;
    public List<CenterNodes> CenterNodesList;
    public List<UpNodes> UpNodesList;
    public List<DownNodes> DownNodesList;
    public List<RightNodes> RightNodesList;
    public List<LeftNodes> LeftNodesList;
}
