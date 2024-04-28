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
    public Directions Direction;
}

public struct Directions
{
    public Vector2Int DirectionX;
    public Vector2Int DirectionY;
}

/*
 * DirectionX || Vector2Int(Left, Right)
 * DirectionX || Vector2Int (1,1) ---> Left And Right is closed
 * DirectionX || Vector2Int (0,0) ---> Left And Right is open
 *
 * DirectionY || Vector2Int (Up, Down)
 * DirectionY || Vector2Int (1,1) Up And Down is closed
 * DirectionY || Vector2Int (0,0) Up And Down is open
 */

//Rooms
[Serializable] public class CenterNodes : Node 
{
    public CenterNodes() 
    {
        Direction.DirectionX = new Vector2Int(0, 0);
        Direction.DirectionY = new Vector2Int(0, 0); 
    } 
}

[Serializable]
public class UpNodes : Node
{
    public UpNodes()
    {
        Direction.DirectionX = new Vector2Int(1, 1);
        Direction.DirectionY = new Vector2Int(0, 1);
    }
}

[Serializable]
public class DownNodes : Node
{
    public DownNodes()
    {
        Direction.DirectionX = new Vector2Int(1, 1);
        Direction.DirectionY = new Vector2Int(1, 0);
    }
}

[Serializable]
public class RightNodes : Node
{
    public RightNodes()
    {
        Direction.DirectionX = new Vector2Int(1, 0);
        Direction.DirectionY = new Vector2Int(1, 1);
    }
}

[Serializable]
public class LeftNodes : Node
{
    public LeftNodes()
    {
        Direction.DirectionX = new Vector2Int(0, 1);
        Direction.DirectionY = new Vector2Int(1, 1);
    }
}

//Old data type
// [Serializable] public class UpNodes : Node { public UpNodes() { Direction = new Vector2Int(0, 1); } }
// [Serializable] public class DownNodes : Node { public DownNodes() { Direction = new Vector2Int(0,-1); } }
// [Serializable] public class RightNodes : Node { public RightNodes() { Direction = new Vector2Int(1,0); } }
// [Serializable] public class LeftNodes : Node { public LeftNodes() { Direction = new Vector2Int(-1,0); } }

//--------------------------------------------Corridors-------------------------------------------


/*
 * UpDownNodes represents Up and Down is open
 * 
 * DirectionX || Vector2Int (1,1) both are closed
 * DirectionY || Vector2Int (0,0) both are open
 */
[Serializable]
public class UpDownNodes : Node
{
    public UpDownNodes()
    {
        Direction.DirectionX = new Vector2Int(1,1);
        Direction.DirectionY = new Vector2Int(0,0);
    }
}

/*
 * UpRightNode represents Up and Right is open
 * 
 * DirectionX || Vector2Int (1,0) Right Open
 * DirectionY || Vector2Int (0,1) Up open
 */
[Serializable]
public class UpRightNodes : Node
{
    public UpRightNodes()
    {
        Direction.DirectionX = new Vector2Int(1,0);
        Direction.DirectionY = new Vector2Int(0,1);
    }
}

/*
 * UpLeftNodes represents Up and Left is open
 * 
 * DirectionX || Vector2Int (0,1) Left Open
 * DirectionY || Vector2Int (0,1) Up open
 */
[Serializable]
public class UpLeftNodes : Node
{
    public UpLeftNodes()
    {
        Direction.DirectionX = new Vector2Int(0,1);
        Direction.DirectionY = new Vector2Int(0,1);
    }
}

/*
 * DownLeftNodes represents Down and Left is open
 * 
 * DirectionX || Vector2Int (0,1) Left Open
 * DirectionY || Vector2Int (1,0) Down open
 */
[Serializable]
public class DownLeftNodes : Node
{
    public DownLeftNodes()
    {
        Direction.DirectionX = new Vector2Int(0,1);
        Direction.DirectionY = new Vector2Int(1,0);
    }
}

/*
 * DownRightNodes represents Down and Right is open
 * 
 * DirectionX || Vector2Int (1,0) Right Open
 * DirectionY || Vector2Int (1,0) Down open
 */
[Serializable]
public class DownRightNodes : Node
{
    public DownRightNodes()
    {
        Direction.DirectionX = new Vector2Int(1,0);
        Direction.DirectionY = new Vector2Int(1,0);
    }
}

/*
 * RightLeftNodes represents Left and Right is open
 * 
 * DirectionX || Vector2Int (0,0) both are open
 * DirectionY || Vector2Int (1,1) both are closed
 */
[Serializable]
public class RightLeftNodes : Node
{
    public RightLeftNodes()
    {
        Direction.DirectionX = new Vector2Int(0,0);
        Direction.DirectionY = new Vector2Int(1,1);
    }
}