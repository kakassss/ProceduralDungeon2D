using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class DungeonNodeGenerator : MonoBehaviour
{
    [SerializeField] private NodeGameObjectDataProvider nodeGameObjectDataProvider;
    
    [HideInInspector] public Vector2Int Center = Vector2Int.zero;


    public int Width;
    public int Height;

    
    public Grid gridData;
    //private List<NodeData> _nodeDatas;

    private void Awake()
    {
        //10X10
        gridData = new Grid();
        //_nodeDatas = new List<NodeData>();
        gridData.Grids = new int[Width, Height];


        // for (int i = 0; i < Width; i++)
        // {
        //     for (int j = 0; j < Height; j++)
        //     {
        //         NodeData newNode = new NodeData(i, j);
        //         _nodeDatas.Add(newNode);
        //     }
        // }
        //
        // foreach (var nodeData in _nodeDatas)
        // {
        //     nodeData.WritePosition();
        // }
        
        CreateDungeon();
    }

    private Vector2Int NodePoint = Vector2Int.zero;

    private HashSet<Vector2Int> dungeonNodePosList = new HashSet<Vector2Int>();

    private int iterationCount = 100;
    
    private void CreateDungeon()
    {
        NodeData<CenterNodes> _initNode = new NodeData<CenterNodes>(new CenterNodes(),NodePoint.x,NodePoint.y);
        _initNode.node.NodeGameobject = nodeGameObjectDataProvider.GetCurrentNodeGO(_initNode.node);

        var nodeGo = _initNode.node.NodeGameobject;
        
        // Debug.Log("CenterNode Y" + _initNode.node.Direction.DirectionY);
        // Debug.Log("CenterNode X" + _initNode.node.Direction.DirectionX);
        
        dungeonNodePosList.Add(NodePoint);
        
        var nextNodePos = _initNode.Position;
        Instantiate(nodeGo, new Vector3(_initNode.PosX,_initNode.PosY,0),Quaternion.identity,transform);
        
        //Create positions
        for (int i = 0; i < iterationCount; i++)
        {
            nextNodePos += GetRandomDirection();
            dungeonNodePosList.Add(nextNodePos);
        }
        //Transfer to the list
        List<Vector2Int> NodePositions = new List<Vector2Int>(dungeonNodePosList);
        
        foreach (var currentPosition in NodePositions)
        {
            var randomNodeData = GetRandomNodeData<Node>(nextNodePos.x,nextNodePos.y);
            randomNodeData.node.NodeGameobject = nodeGameObjectDataProvider.GetCurrentNodeGO(randomNodeData.node);
            
            Instantiate(randomNodeData.node.NodeGameobject, new Vector3(currentPosition.x,currentPosition.y,0),Quaternion.identity,transform);
        }
        
        // var randomNodeData = GetRandomNodeData<Node>(nextNodePos.x,nextNodePos.y);
        // randomNodeData.node.NodeGameobject = nodeGameObjectDataProvider.GetCurrentNodeGO(randomNodeData.node);
        //
        // Instantiate(randomNodeData.node.NodeGameobject, new Vector3(nextNodePos.x,nextNodePos.y,0),Quaternion.identity,transform);
       
        Debug.Log("hashset " + dungeonNodePosList.Count);
        
        
        /*
         * En son node dataları için yeni bir direction datası oluşturdun.
         * bu data ile -1 0 1 gibi sayılarla uğrasmak yerine x için ve y için 2 tane ayrı vector oluşturdun.
         * şuan oluşturulan node'un sağ solunu ve yukarı aşağısının dolu olup olmadıgını test eden basit bir
         * if else döngüsü kurdun.
         * 
         * Yeni oluşturulan data ile bi sorun çıkmadı gibi gözüküyor.
         * Şuan kalan, for içinde nereye gideceğini seçen basit
         * bir producedural mantık kaldı gibi gözüküyor.
         */
        
        
        // NodeData: 0 is open, 1 is close
        void CheckOpenPositions(Node node)
        {
            var currentNode = node.Direction;
            
            //Check Left and Right possibilities
            if (currentNode.DirectionX == Vector2Int.zero)
            {
                //Left And Right node area is open
            }
            else
            {
                if (currentNode.DirectionX == new Vector2Int(0, 1))
                {
                    //Left node area is open
                }
                else if (currentNode.DirectionX == new Vector2Int(1, 0))
                {
                    //Right node area is open
                }
            }
           
            //Check Up and Down possibilities
            if (currentNode.DirectionY == Vector2Int.zero)
            {
                //Up and Down node area is open
            }
            else
            {
                if (currentNode.DirectionY == new Vector2Int(0, 1))
                {
                    //Up node area is open, down closed
                }
                else if (currentNode.DirectionY == new Vector2Int(1, 0))
                {
                    //Down node area is open, up closed
                }
            }
            
            
        }
        
    }

    private Node[] randomNodes = new Node[4];

    private Node GetRandomNode()
    {
        randomNodes[0] = new UpNodes();
        randomNodes[1] = new DownNodes();
        randomNodes[2] = new LeftNodes();
        randomNodes[3] = new RightNodes();


        return randomNodes[Random.Range(0, randomNodes.Length)];
    }
    
    private NodeData<T> GetRandomNodeData<T>(int x,int y) where T: Node
    {
        Type[] nodeTypes = { typeof(UpNodes), typeof(DownNodes), typeof(RightNodes), typeof(LeftNodes) };
        Type randomNodeType = nodeTypes[Random.Range(0, nodeTypes.Length)];
        NodeData<T> randomNodeData = new NodeData<T>((T)Activator.CreateInstance(randomNodeType), x, y);
        
        return randomNodeData;
    }


    [HideInInspector] public Vector2Int[] Directions = new[] { Vector2Int.up, Vector2Int.down, Vector2Int.right, Vector2Int.left, };

    private Vector2Int GetRandomDirection()
    {
        return Directions[Random.Range(0, Directions.Length)];
    }
    // private Vector2Int UpDirection = Vector2Int.up;
    // private Vector2Int DownDirection = Vector2Int.down;
    // private Vector2Int RightDirection = Vector2Int.right;
    // private Vector2Int LeftDirection = Vector2Int.left;
    //
    // private List<Vector2Int> directionList = new List<Vector2Int>();
    // private Vector2Int GetRandomDirection()
    // {
    //     directionList.Add(UpDirection);
    //     directionList.Add(DownDirection);
    //     directionList.Add(RightDirection);
    //     directionList.Add(LeftDirection);
    //     
    //     var randomDirection = directionList[Random.Range(0, directionList.Count)];
    //     directionList.Clear();
    //     return randomDirection;
    // }


}

public class Grid
{
    public int[,] Grids;
}

public class NodeData<T>
{
    public T node;
    public int PosX;
    public int PosY;

    public Vector2Int Position;
    
    
    public NodeData(T node, int posX, int posY)
    {
        this.node = node;
        PosX = posX;
        PosY = posY;
        Position = new Vector2Int(PosX, PosY);

    }
}

// public class NodeData
// {
//     public Node node;
//     public int PosX;
//     public int PosY;
//
//     public NodeData(int posX, int posY)
//     {
//         PosX = posX;
//         PosY = posY;
//     }
//
//     public void WritePosition()
//     {
//         Debug.Log("X " + PosX + " Y " + PosY);
//     }
// }
