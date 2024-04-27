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

    private int halfOfWidth;
    private int halfOfHeight;
    
    public Grid gridData;
    //private List<NodeData> _nodeDatas;

    private void Awake()
    {
        halfOfWidth = Width / 2;
        halfOfHeight = Height / 2;
        //10X10
        gridData = new Grid();
        gridData.Grids = new Vector2Int[Width+1, Height+1]; //10x10 -5x-5 to 5x5
        

        
        // gridData.Grids[0, 1] = new Vector2Int(0,0);
        // gridData.Grids[3, 2] = new Vector2Int(3,2);
        // gridData.Grids[0, 9] = new Vector2Int(1,0);
        // gridData.Grids[0, 8] = new Vector2Int(-2,0);

        int counter = 0;
        for (int i = -5; i <= 5; i++)
        {
            for (int j = -5; j <= 5; j++)
            {
                
                gridData.Grids[i + halfOfWidth, j + halfOfHeight] = new Vector2Int(i,j);
                Debug.Log("onur vec 0 0 " + gridData.Grids[0,0]);
                Debug.Log("onur vec 0 10" + gridData.Grids[0,10]);
                Debug.Log("onur vec 0 1" + gridData.Grids[0,1]);
                Debug.Log("onur vec 11 11 " + gridData.Grids[10,10]);
                Debug.Log("onur vec " + gridData.Grids[11,11]);
                Debug.Log("onur vec " + gridData.Grids[11,11]);
                Debug.Log("onur vec " + gridData.Grids[11,11]);
                Debug.Log("onur vec " + gridData.Grids[11,11]);
                Debug.Log("onur vec " + gridData.Grids[11,11]);
            }
        }
        Debug.Log("counterr " + counter);
        //
        // foreach (var nodeData in _nodeDatas)
        // {
        //     nodeData.WritePosition();
        // }
        
        CreateDungeon();
    }

    private Vector2Int NodePoint = Vector2Int.zero;

    private HashSet<Vector2Int> dungeonNodePosList = new HashSet<Vector2Int>();
    private List<Vector2Int> nodePositions;
    private List<NodeData<Node>> nodeDataList = new List<NodeData<Node>>();
    
    
    private int iterationCount = 100;

    // private void GetBorderDungeonGrid()
    // {
    //     for (int i = 0; i < nodePositions.Count; i++)
    //     {
    //         //if(gridData.Grids[])
    //     }
    // }

    private Vector2Int GetXNodePosition<T>(NodeData<T> nodeData) where T: Node
    {
        return nodeData.Position;
    }

    private NodeData<Node> GetNodeAtXPosition(Vector2Int position)
    {
        foreach (var nodeData in nodeDataList.Where(nodeData => nodeData.Position == position))
        {
            return nodeData;
        }
        Debug.LogError("At Given Position Node can not found");
        return null;
    }
    
    private void CreateDungeon()
    {
        NodeData<CenterNodes> _initNode = new NodeData<CenterNodes>(new CenterNodes(),NodePoint.x,NodePoint.y);
        _initNode.node.NodeGameobject = nodeGameObjectDataProvider.GetCurrentNodeGO(_initNode.node);

        var nodeGo = _initNode.node.NodeGameobject;
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
        nodePositions = new List<Vector2Int>(dungeonNodePosList);

        for (int i = 0; i < nodePositions.Count; i++)
        {
            var randomNodeData = GetRandomNodeData<Node>(nextNodePos.x,nextNodePos.y);
            randomNodeData.node.NodeGameobject = nodeGameObjectDataProvider.GetCurrentNodeGO(randomNodeData.node);
            
            CheckOpenPositions(randomNodeData.node);
            
            nodeDataList.Add(randomNodeData);
            
            
            Instantiate(
                randomNodeData.node.NodeGameobject, 
                new Vector3(nodePositions[i].x,nodePositions[i].y,0),
                Quaternion.identity,
                transform);
        }
        
        foreach (var currentPosition in nodePositions)
        {
            var randomNodeData = GetRandomNodeData<Node>(nextNodePos.x,nextNodePos.y);
            randomNodeData.node.NodeGameobject = nodeGameObjectDataProvider.GetCurrentNodeGO(randomNodeData.node);
            
            CheckOpenPositions(randomNodeData.node);
            
            nodeDataList.Add(randomNodeData);
            
            Instantiate(randomNodeData.node.NodeGameobject, new Vector3(currentPosition.x,currentPosition.y,0),Quaternion.identity,transform);
        }

        
        
        
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


    private readonly Vector2Int[] _directions = 
    {                                               
        Vector2Int.up,// 0,1 
        Vector2Int.down,// 0,-1 
        Vector2Int.right, // 1,0 
        Vector2Int.left,// -1,0       
    };

    private Vector2Int GetRandomDirection()
    {
        return _directions[Random.Range(0, _directions.Length)];
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
    public Vector2Int[,] Grids;
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
