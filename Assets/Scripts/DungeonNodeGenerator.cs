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
    
    private Grid gridData;

    private void Awake()
    {
        halfOfWidth = Width / 2;
        halfOfHeight = Height / 2;
        
        
        //11X11
        gridData = new Grid();
        gridData.Grids = new NodeData<Node>[Width + 1, Height + 1];
        // gridData = new Grid
        // {
        //     Grids = new NodeData<Node>[,] //10x10 -5x-5 to 5x5
        // };
        //new Vector2Int[Width+1, Height+1]
        
        // gridData.Grids[0, 1] = new Vector2Int(0,0);
        // gridData.Grids[3, 2] = new Vector2Int(3,2);
        // gridData.Grids[0, 9] = new Vector2Int(1,0);
        // gridData.Grids[0, 8] = new Vector2Int(-2,0);
        int counter = 0;
        for (int i = -halfOfWidth; i <= halfOfWidth; i++)
        {
            for (int j = -halfOfHeight; j <= halfOfHeight; j++)
            {
                gridData.Grids[i + halfOfWidth, j + halfOfHeight] = new NodeData<Node>(i, j);
                counter++;
            }
        }
        Debug.Log("gridData.Grids[0, 0] " + gridData.Grids[0, 0].Position);

        GridDataConvertToList();
        
        
        // Debug.Log("onur vec 0 0 " + gridData.Grids[0,0]);
        // Debug.Log("onur vec 0 10" + gridData.Grids[0,10]);
        // Debug.Log("onur vec 0 1" + gridData.Grids[0,1]);
        // Debug.Log("onur vec 0 2" + gridData.Grids[0,2]);
        // Debug.Log("onur vec 11 11 " + gridData.Grids[10,10]);
        // Debug.Log("counterr " + counter);
        //
        // foreach (var nodeData in _nodeDatas)
        // {
        //     nodeData.WritePosition();
        // }
        
        CreateDungeon();
    }

    private void GridDataConvertToList()
    {
        for (int i = -halfOfWidth; i <= halfOfWidth; i++)
        {
            for (int j = -halfOfHeight; j <= halfOfHeight; j++)
            {
                GridDataList.Add(gridData.Grids[i + halfOfWidth, j + halfOfHeight]);
            }
        }
    }
    
    private Vector2Int pointZero = Vector2Int.zero;

    private HashSet<Vector2Int> nodePositionsHashSet = new HashSet<Vector2Int>();
    private List<Vector2Int> nodePositionsList;
    private List<NodeData<Node>> nodeDataList = new List<NodeData<Node>>();
    
    
    private int iterationCount = 100;
    
    private List<NodeData<Node>> GridDataList = new List<NodeData<Node>>();
    private bool CheckIsNodeExceedGridBorder(Vector2Int position)
    {
        foreach (var grids in GridDataList)
        {
            if (position == grids.Position)
                return true;
        }
        return false;
    }

    private bool SetNodePositionData(Vector2Int nodePosition)
    {
        foreach (var grids in GridDataList)
        {
            if (nodePosition == grids.Position)
            {
                grids.IsEmpty = false;
                return true;
            }
        }

        return false;
    }
    /*
     * YAPTIGIN PREFABLER HEP TEK TARAFLI, BİR YÖNE KAPISI VAR DİĞER YÖNE YOK
     * BU DA DUNGEONI İMKANSIZ KILIYOR ÖNCE BUNU DÜZELTMEN LAZIM, HALI HAZIRDA KORİDOR OLARAK
     * BAZI PREFABLERİN VAR AMA ONLARIN DATA KISMI YOK SANIIRM?
     * 
     * EN SON DİRECTİON CHECKLEME MANTIGINDA SORUN YAŞADIN, POSİZYONLARI ÖNCEDEN OLUŞTURUP ÜSTÜNE
     * NODELARI OLUŞTURUYORDUN FAKAT DİRECTİON DATALARINI NODELAR TAŞIYOR
     * BELKİ BURDA GENEL YAPIYI DEĞİŞTİREBİLİRSİN YA DA BİR ŞEKİLDE NODELARI POZİSYONLA YARATABİLİRSİN
     *
     * EN SON YAPMAYA ÇALISTIGIN NODELARI POZİSYON İLE YARATMAK FAKAT O DA KENDİNCE GARİP SORUNLARI VAR
     *
     *
     * 
     */
    
    private void CreateDungeon()
    {
        NodeData<CenterNodes> initNode = new NodeData<CenterNodes>(new CenterNodes(),pointZero.x,pointZero.y);
        initNode.node.NodeGameobject = nodeGameObjectDataProvider.GetCurrentNodeGO(initNode.node);

        var nodeGo = initNode.node.NodeGameobject;
        nodePositionsHashSet.Add(pointZero);
        
        var nextNodePos = initNode.Position;
        Instantiate(nodeGo, new Vector3(initNode.PosX,initNode.PosY,0),Quaternion.identity,transform);
        //initNode.IsEmpty = false;
        
        //Create positions
        for (int i = 0; i < iterationCount; i++)
        {
            nextNodePos += GetRandomDirection();
            CheckOpenPositions(initNode.node);
            /*
             *  KORİDORLARI İŞİN İÇİNE SOKTUKTAN SONRA 
             *  CHECKOPENPOSİTİON FONKSİYONUNDAN RANDOM BİR NODE DÖNDÜRÜP AŞAĞIDAKİ
             *  TEMP VARİABLEINA VERMEK BİR FİKİR OLARAK VAR İDİ KAFANDA
             * 
             */
            var randomNodeTemp = GetRandomNodeData<Node>(nextNodePos.x,nextNodePos.y);
            nodeDataList.Add(randomNodeTemp);
            
                
                    
            if (CheckIsNodeExceedGridBorder(nextNodePos) == false)
            {
                nextNodePos = pointZero;
                continue;
            }
            
            nodePositionsHashSet.Add(nextNodePos);
        }
        
        //Transfer to the list
        nodePositionsList = new List<Vector2Int>(nodePositionsHashSet);
        //SetNodePositionData(nodePositionsList);
        
        //Get Random Node and Instantiate
        for (int i = 0; i < nodePositionsList.Count; i++)
        {
            var randomNodeData = GetRandomNodeData<Node>(nextNodePos.x,nextNodePos.y);
            randomNodeData.node.NodeGameobject = nodeGameObjectDataProvider.GetCurrentNodeGO(randomNodeData.node);
            
            //CheckOpenPositions(randomNodeData.node);
            
            //nodeDataList.Add(randomNodeData);
            
            
            Instantiate(randomNodeData.node.NodeGameobject, 
                new Vector3(nodePositionsList[i].x,nodePositionsList[i].y,0),
                Quaternion.identity, transform);
        }
        
        
        
    }
     // NodeData: 0 is open, 1 is close
    private void CheckOpenPositions(Node node)
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
                return;
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
    private readonly Node[] randomNodes = new Node[4];

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
    public NodeData<Node>[,] Grids;
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
