using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
