using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
        //EN SON YAPMAYA CALISTIGIN SEY
        /*
         *  Aşağıdaki GetRandomNode fonksiyonundan
         * rastgele bir node çekmek istiyorsun fakat, çekilen rastgele
         * node'un içindeki gameobje newlendiği için null oluyor.
         * 5 farklı node var, belli bi for içinde her seferinde şu şu diyip GO atanmayacagını düşünüyorsun
         * bu yüzden atamadan otomatik olarak kendi içlerinde bir şekilde referanslanmaları gerekiyor.
         *
         * bunla ilgili bi şey denerken DungeonNodesSO scriptindeki
         *
         * Node 2 ve CenterNodes2 classlarını oluşturdun.
         * Onlardan da pek bir şey çıkmayacak gibi düşünmeye başlamıstın
         */
        NodeData<CenterNodes> _initNode = new NodeData<CenterNodes>(new CenterNodes(),NodePoint.x,NodePoint.y);
        _initNode.node.NodeGameobject = nodeGameObjectDataProvider.GetCurrentNodeGO(_initNode.node);
        
        
        dungeonNodePosList.Add(NodePoint);
        var nextNodePos = _initNode;
        Instantiate(_initNode.node.NodeGameobject, new Vector3(_initNode.PosX,_initNode.PosY,0),Quaternion.identity);
        
        
        // for (int i = 0; i < iterationCount; i++)
        // {
        //     nextNodePos.Position += GetRandomDirection();
        //     dungeonNodePosList.Add(nextNodePos.Position);
        //     Instantiate(visualNode, new Vector3(nextNodePos.PosX,nextNodePos.PosY,0),Quaternion.identity);
        // }
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

public class NodeData<T> where T : Node
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
