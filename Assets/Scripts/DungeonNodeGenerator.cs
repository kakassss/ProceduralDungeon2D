using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class DungeonNodeGenerator : MonoBehaviour
{
    [SerializeField] private NodeGameObjectDataProvider nodeGameObjectDataProvider;
    
    public int Width;
    public int Height;

    [SerializeField] private float corridorNodeRate = 0.5f;
    
    private Vector2Int _pointZero = Vector2Int.zero;

    private readonly HashSet<Vector2Int> _nodePositionsHashSet = new HashSet<Vector2Int>();
    private List<Vector2Int> _nodePositionsList;
    private readonly List<NodeData<Node>> _nodeDataList = new List<NodeData<Node>>();
    private readonly List<NodeData<Node>> _gridDataList = new List<NodeData<Node>>();
    
    private Grid _gridData;
    
    private readonly int _iterationCount = 100;
    private int _halfOfWidth;
    private int _halfOfHeight;

    private int _totalGridCount = 0;
    
    private readonly Type[] _allNodeTypes =
    {
        typeof(UpNodes), typeof(DownNodes), typeof(RightNodes), typeof(LeftNodes),
        typeof(UpDownNodes),typeof(UpRightNodes),typeof(UpLeftNodes),typeof(DownLeftNodes),
        typeof(DownRightNodes),typeof(RightLeftNodes)
    };
    
    private readonly Type[] _nodeTypes =
    {
        typeof(UpNodes), typeof(DownNodes), typeof(RightNodes), typeof(LeftNodes),
    };
    
    private readonly Type[] _corridorNodeTypes =
    {
        typeof(UpDownNodes),typeof(UpRightNodes),typeof(UpLeftNodes),typeof(DownLeftNodes),
        typeof(DownRightNodes),typeof(RightLeftNodes)
    };
    
    private readonly Vector2Int[] _directions = 
    {                                               
        Vector2Int.up,    // 0,1 
        Vector2Int.down,  // 0,-1 
        Vector2Int.right, // 1,0 
        Vector2Int.left,  // -1,0       
    };

    private void Awake()
    {
        _halfOfWidth = Width / 2;
        _halfOfHeight = Height / 2;
        
        //11X11
        _gridData = new Grid();
        _gridData.Grids = new NodeData<Node>[Width + 1, Height + 1];//10x10 -5x-5 to 5x5

        for (int i = -_halfOfWidth; i <= _halfOfWidth; i++)
        {
            for (int j = -_halfOfHeight; j <= _halfOfHeight; j++)
            {
                _gridData.Grids[i + _halfOfWidth, j + _halfOfHeight] = new NodeData<Node>(i, j);
                _totalGridCount++;
            }
        }

        GridDataConvertToList();
        CreateDungeon();
    }

    private void GridDataConvertToList()
    {
        for (int i = -_halfOfWidth; i <= _halfOfWidth; i++)
        {
            for (int j = -_halfOfHeight; j <= _halfOfHeight; j++)
            {
                _gridDataList.Add(_gridData.Grids[i + _halfOfWidth, j + _halfOfHeight]);
            }
        }
    }
    
    private bool CheckIsNodeExceedGridBorder(Vector2Int position)
    {
        foreach (var grids in _gridDataList)
        {
            if (position == grids.Position)
                return true;
        }
        return false;
    }

    private bool SetNodePositionData(Vector2Int nodePosition)
    {
        foreach (var grids in _gridDataList)
        {
            if (nodePosition == grids.Position)
            {
                grids.IsEmpty = false;
                return true;
            }
        }

        return false;
    }
    
    private void CreateDungeon()
    {
        NodeData<CenterNodes> initNode = new NodeData<CenterNodes>(new CenterNodes(),_pointZero.x,_pointZero.y);
        initNode.node.NodeGameobject = nodeGameObjectDataProvider.GetCurrentNodeGO(initNode.node);

        var nodeGo = initNode.node.NodeGameobject;
        _nodePositionsHashSet.Add(_pointZero);
        
        var nextNodePos = initNode.Position;
        Instantiate(nodeGo, new Vector3(initNode.PosX,initNode.PosY,0),Quaternion.identity,transform);
        //initNode.IsEmpty = false;
        
        //Create positions
        for (int i = 0; i < _iterationCount; i++)
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
            _nodeDataList.Add(randomNodeTemp);
            
                
                    
            if (CheckIsNodeExceedGridBorder(nextNodePos) == false)
            {
                nextNodePos = _pointZero;
                continue;
            }
            
            _nodePositionsHashSet.Add(nextNodePos);
        }
        
        //Transfer to the list
        _nodePositionsList = new List<Vector2Int>(_nodePositionsHashSet);
        //SetNodePositionData(nodePositionsList);
        
        //Get Random Node and Instantiate
        for (int i = 0; i < _nodePositionsList.Count; i++)
        {
            var randomNodeData = GetRandomNodeData<Node>(nextNodePos.x,nextNodePos.y);
            randomNodeData.node.NodeGameobject = nodeGameObjectDataProvider.GetCurrentNodeGO(randomNodeData.node);
            
            //CheckOpenPositions(randomNodeData.node);
            
            //nodeDataList.Add(randomNodeData);
            
            
            Instantiate(randomNodeData.node.NodeGameobject, 
                new Vector3(_nodePositionsList[i].x,_nodePositionsList[i].y,0),
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

    
    //[SerializeField] private float normalNodeRate = 0.8f; 
    private NodeData<T> GetRandomNodeData<T>(int x,int y) where T: Node
    {
        Type randomNodeType = SelectRandomNode();
        NodeData<T> randomNodeData = new NodeData<T>((T)Activator.CreateInstance(randomNodeType), x, y);
        
        return randomNodeData;
    }

    
    private Type SelectRandomNode()
    {
        Type randomNodeType = null;
        
        float randomValue = Random.value;
        
        if (randomValue <= corridorNodeRate) // for 0.2f value, %20 chance
        {
            randomNodeType = GetNodeType(_corridorNodeTypes);
        }
        else if (randomValue >= corridorNodeRate)// for 0.2f value, %80 chance
        {
            randomNodeType = GetNodeType(_nodeTypes);
        }
        
        return randomNodeType;
    }

    private Type GetNodeType(Type[] currentType)
    {
        return currentType[Random.Range(0, currentType.Length)];
    }
    
    private Vector2Int GetRandomDirection()
    {
        return _directions[Random.Range(0, _directions.Length)];
    }
}