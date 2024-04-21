using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class DungeonNodeGenerator : MonoBehaviour
{
    public Vector2Int Center = Vector2Int.zero;


    public int Width;
    public int Height;

    
    public Grid gridData;
    private List<NodeData> _nodeDatas;

    private void Awake()
    {
        //10X10
        gridData = new Grid();
        _nodeDatas = new List<NodeData>();
        gridData.Grids = new int[Width, Height];


        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                NodeData newNode = new NodeData(i, j);
                _nodeDatas.Add(newNode);
            }
        }

        foreach (var nodeData in _nodeDatas)
        {
            nodeData.WritePosition();
        }
        
        CreateDungeon();
    }

    [SerializeField] private GameObject visualNode;
    private Vector2Int NodePoint = Vector2Int.zero;

    private HashSet<Vector2Int> dungeonNodePosList = new HashSet<Vector2Int>();

    private int iterationCount = 100;
    
    private void CreateDungeon()
    {
        dungeonNodePosList.Add(NodePoint);
        var nextNodePos = NodePoint;
        Instantiate(visualNode, new Vector3(nextNodePos.x,nextNodePos.y,0),Quaternion.identity);
        
        
        for (int i = 0; i < iterationCount; i++)
        {
            nextNodePos += GetRandomDirection();
            dungeonNodePosList.Add(nextNodePos);
            Instantiate(visualNode, new Vector3(nextNodePos.x,nextNodePos.y,0),Quaternion.identity);
        }
        
        
    }
    
    private Vector2Int UpDirection = Vector2Int.up;
    private Vector2Int DownDirection = Vector2Int.down;
    private Vector2Int RightDirection = Vector2Int.right;
    private Vector2Int LeftDirection = Vector2Int.left;

    private List<Vector2Int> directionList = new List<Vector2Int>();
    private Vector2Int GetRandomDirection()
    {
        directionList.Add(UpDirection);
        directionList.Add(DownDirection);
        directionList.Add(RightDirection);
        directionList.Add(LeftDirection);
        
        var randomDirection = directionList[Random.Range(0, directionList.Count)];
        directionList.Clear();
        return randomDirection;
    }


}

public class Grid
{
    public int[,] Grids;
}

public class NodeData
{
    public Node node;
    public int PosX;
    public int PosY;

    public NodeData(int posX, int posY)
    {
        PosX = posX;
        PosY = posY;
    }

    public void WritePosition()
    {
        Debug.Log("X " + PosX + " Y " + PosY);
    }
}
