using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DungeonNodeGenerator : MonoBehaviour
{
    [SerializeField] private NodeGameObjectDataProvider nodeGameObjectDataProvider;
    
    public int Width;
    public int Height;

    private Grid gridData;
    
    private int halfOfWidth;
    private int halfOfHeight;
    
    
    private readonly Type[] nodeTypes =
    {
        typeof(UpNodes), typeof(DownNodes), typeof(RightNodes), typeof(LeftNodes),
        typeof(UpDownNodes),typeof(UpRightNodes),typeof(UpLeftNodes),typeof(DownLeftNodes),
        typeof(DownRightNodes),typeof(RightLeftNodes)
    };
    
    private readonly Vector2Int[] _directions = 
    {                                               
        Vector2Int.up,// 0,1 
        Vector2Int.down,// 0,-1 
        Vector2Int.right, // 1,0 
        Vector2Int.left,// -1,0       
    };

    private void Awake()
    {
        halfOfWidth = Width / 2;
        halfOfHeight = Height / 2;
        int counter = 0;
        
        
        //11X11
        gridData = new Grid();
        gridData.Grids = new NodeData<Node>[Width + 1, Height + 1];//10x10 -5x-5 to 5x5

        for (int i = -halfOfWidth; i <= halfOfWidth; i++)
        {
            for (int j = -halfOfHeight; j <= halfOfHeight; j++)
            {
                gridData.Grids[i + halfOfWidth, j + halfOfHeight] = new NodeData<Node>(i, j);
                counter++;
            }
        }

        GridDataConvertToList();
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
    private NodeData<T> GetRandomNodeData<T>(int x,int y) where T: Node
    {
        Type randomNodeType = nodeTypes[Random.Range(0, nodeTypes.Length)];
        NodeData<T> randomNodeData = new NodeData<T>((T)Activator.CreateInstance(randomNodeType), x, y);
        
        return randomNodeData;
    }
    
    private Vector2Int GetRandomDirection()
    {
        return _directions[Random.Range(0, _directions.Length)];
    }
}