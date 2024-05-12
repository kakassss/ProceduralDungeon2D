using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class DungeonNodeGenerator : MonoBehaviour
{
    [HideInInspector] public List<NodeData<Node>> _allInstantiatedNodes = new List<NodeData<Node>>();
    [HideInInspector] public List<GameObject> _allInstantiatedNodesGO = new List<GameObject>();
    
    public int Width;
    public int Height;
    
    [SerializeField] private NodeGameObjectDataProvider nodeGameObjectDataProvider;
    [SerializeField] private float corridorNodeRate = 0.5f;
    [SerializeField] private int _iterationCount = 100;
    
    private readonly List<NodeData<Node>> _gridDataList = new List<NodeData<Node>>();
    private List<Vector2Int> _nodePositionsList;
    private Vector2Int _currentPointPosition;
    
    private NodeData<Node> _currentPointNodeData;
    private Node _selectedNode;
    private Grid _gridData;
    private List<Node> _xNodes;
    private List<Node> _yNodes;
    
    private int _totalGridCount;
    private int _halfOfWidth;
    private int _halfOfHeight;
    
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
        typeof(DownRightNodes),typeof(RightLeftNodes), typeof(CenterNodes)
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
        NodeRefactor nodeRefactor = new NodeRefactor();
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
        InstantiateCenterNode();
        
        for (int i = 0; i < _iterationCount; i++)
        {
            SetPositionPointNodeAndInstantiate();
        }

        
        
        nodeRefactor.SetInstantiatedNodeDatas(transform,this,nodeGameObjectDataProvider);
        foreach (var destroyableNode in nodeRefactor.DestroyGOList)
        {
            Destroy(destroyableNode);
        }
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

    private void InstantiateCenterNode()
    {
        NodeData<Node> centerNode = GetNodeDataFromNode<Node>(NodeDataProvider.CenterNode, Vector2Int.zero.x, Vector2Int.zero.y);
        centerNode.node.NodeGameobject = nodeGameObjectDataProvider.GetCurrentNodeGO(centerNode.node);
                
        var centerNodeGO = Instantiate(centerNode.node.NodeGameobject, new Vector3(centerNode.Position.x,centerNode.Position.y,0),
            Quaternion.identity, transform);
        _allInstantiatedNodes.Add(centerNode);
        //_allInstantiatedNodesGO.Add(centerNodeGO);
    }
    
    private void SetPositionPointNodeAndInstantiate()
    {
        //var randomGridData = Random.Range(0, _gridDataList.Count);
        var minRandomGridData = Random.Range(0 ,34);   // These array groups has a bigger numbers except 
        var maxRandomGridData = Random.Range(86, 120); // other them.
        _currentPointNodeData = _gridDataList[Random.Range(0,120)]; // Random.Range(minRandomGridData,maxRandomGridData)
        _currentPointPosition = _currentPointNodeData.Position;
        
        Vector2Int startNodePosition = Vector2Int.zero;
        Vector2Int targetNodePosition = _currentPointPosition;

        List<Vector2Int> mainPathPosition = new List<Vector2Int>();
        List<NodeData<Node>> pathNodes = new List<NodeData<Node>>();
        _xNodes = new List<Node>();
        _yNodes = new List<Node>();
        Vector2Int corridorPosition;

        _selectedNode = NodeDataProvider.CenterNode;
        var nextNodePos = startNodePosition;
        int corner = Random.Range(0,2);
        bool cornerSelect = corner == 0;
        
        
        // Assuming target vector be like (1,2) (-2,3) (-3,-5) (2,-4)
        if (targetNodePosition.x == 0 || targetNodePosition.y == 0) return;
        
        var posX = targetNodePosition.x;
        var posY = targetNodePosition.y;
        
        
        //An example for (4,-4)
        if (posX > 0 && posY < 0)
        {
            SetTargetPositionNodes(1,1,-1,-1,-1,1);
            
            //Create NodesDatas and declare their positions
            for (int i = 0; i < mainPathPosition.Count; i++)
            {
                SetNodeData(i);
            }
            
            //Get and set edgeNodes datas
            SetEdgeNodesDatas(NodeDataProvider.DownRightNode,NodeDataProvider.LeftNode,
                NodeDataProvider.UpNode,NodeDataProvider.UpRightNode);
            
            //Instantiate NodeData Gameobjects
            foreach (var nodeData in pathNodes)
            {
                InstantiateNodeGameobject(nodeData);
            }
        }
        
        //An example for (1,2)
        if (posX > 0 && posY > 0)
        {
            SetTargetPositionNodes(1,1,1,1,1,1);
            
            //Create NodesDatas and declare their positions
            for (int i = 0; i < mainPathPosition.Count; i++)
            {
                SetNodeData(i);
            }
            
            //Get and set edgeNodes datas
            SetEdgeNodesDatas(NodeDataProvider.UpLeftNode,NodeDataProvider.LeftNode,
                NodeDataProvider.DownNode,NodeDataProvider.DownRightNode);
            
            //Instantiate NodeData Gameobjects
            foreach (var nodeData in pathNodes)
            {
                InstantiateNodeGameobject(nodeData);
            }
        }
        
        //An example for (-2,3)
        if (posX < 0 && posY > 0)
        {
            SetTargetPositionNodes(-1,-1,1,1,1,-1);
            
            //Create NodesDatas and declare their positions
            for (int i = 0; i < mainPathPosition.Count; i++)
            {
                SetNodeData(i);
            }
            
            //Get and set edgeNodes datas
            SetEdgeNodesDatas(NodeDataProvider.UpRightNode,NodeDataProvider.RightNode,
                NodeDataProvider.DownNode,NodeDataProvider.DownLeftNode);
            
            //Instantiate NodeData Gameobjects
            foreach (var nodeData in pathNodes)
            {
                InstantiateNodeGameobject(nodeData);
            }
        }
        
        //An example for (-3,-5)
        if (posX < 0 && posY < 0)
        {
            SetTargetPositionNodes(-1,-1,-1,-1,-1,-1);
            
            //Create NodesDatas and declare their positions
            for (int i = 0; i < mainPathPosition.Count; i++)
            {
                SetNodeData(i);
            }
            
            //Get and set edgeNodes datas
            SetEdgeNodesDatas(NodeDataProvider.DownRightNode,NodeDataProvider.RightNode,NodeDataProvider.UpNode,NodeDataProvider.UpLeftNode);
            
            //Instantiate NodeData Gameobjects
            foreach (var nodeData in pathNodes)
            {
                InstantiateNodeGameobject(nodeData);
            }
        }
        
        void InstantiateNodeGameobject(NodeData<Node> nodeData)
        {
            nodeData.node.NodeGameobject = nodeGameObjectDataProvider.GetCurrentNodeGO(nodeData.node);
            var currentNodeGO =  Instantiate(nodeData.node.NodeGameobject, new Vector3(nodeData.Position.x,nodeData.Position.y,0),
                Quaternion.identity, transform);
            _allInstantiatedNodesGO.Add(currentNodeGO);
        }
        
        void SetNodeData(int i)
        {
            nextNodePos += mainPathPosition[i];
            _selectedNode = SetSelectedNodes(mainPathPosition[i],NodeDataProvider.RightLeftNode,NodeDataProvider.UpDownNode);
            NodeData<Node> placableNode = GetNodeDataFromNode<Node>(_selectedNode, nextNodePos.x, nextNodePos.y);
            pathNodes.Add(placableNode);
            _allInstantiatedNodes.Add(placableNode);
        }

        void SetEdgeNodesDatas(Node xNodeTrue,Node xNodeFalse,Node yNodeTrue, Node yNodeFalse)
        {
            if (cornerSelect)
            {
                var xEdgeNode = pathNodes[_xNodes.Count-1];
                xEdgeNode.node = cornerSelect ? xNodeTrue : xNodeFalse;
                var yEdgeNode = pathNodes[^1];
                yEdgeNode.node = cornerSelect ? yNodeTrue: yNodeFalse;
            }
            else
            {
                var yEdgeNode = pathNodes[_yNodes.Count -1];
                yEdgeNode.node = cornerSelect ? yNodeTrue: yNodeFalse;
                var xEdgeNode = pathNodes[^1];
                xEdgeNode.node = cornerSelect ? xNodeTrue : xNodeFalse;
            }
        }
        
        void SetTargetPositionNodes(int corridorTrue, int xPosTrue, int yPosTrue,int corridorFalse, int xPosFalse,int yPosFalse)
        {
            if (cornerSelect)
            {
                corridorPosition = new Vector2Int(corridorTrue, 0);
                targetNodePosition -= corridorPosition;
                
                for (int i = 0; i < Mathf.Abs(targetNodePosition.x); i++)
                {
                    mainPathPosition.Add(new Vector2Int(xPosTrue,0));
                }
                    
                mainPathPosition.Add(corridorPosition);
                
                for (int i = 0; i < Mathf.Abs(targetNodePosition.y); i++)
                {
                    mainPathPosition.Add(new Vector2Int(0,yPosTrue));
                }
            }
            else
            {
                corridorPosition = new Vector2Int(0, corridorFalse);
                targetNodePosition -= corridorPosition;
                
                for (int i = 0; i < Mathf.Abs(targetNodePosition.y); i++)
                {
                    mainPathPosition.Add(new Vector2Int(0,xPosFalse));
                }
                    
                mainPathPosition.Add(corridorPosition);
                    
                for (int i = 0; i < Mathf.Abs(targetNodePosition.x); i++)
                {
                    mainPathPosition.Add(new Vector2Int(yPosFalse,0));
                }
            }
        }
        
        
        
    }
    
    private Node SetSelectedNodes(Vector2Int nodeDirection,Node xNode,Node yNode)
    {
        if (nodeDirection.x is 1 or -1 && nodeDirection.y == 0)
        {
            _xNodes.Add(xNode);
            return xNode;
        }
        
        if (nodeDirection.y is 1 or -1 && nodeDirection.x == 0)
        {
            _yNodes.Add(yNode);
            return yNode;
        }
        
        Debug.LogError("CenterNode has returned!");
        return NodeDataProvider.CenterNode;
    }
    
    private NodeData<T> GetRandomNodeData<T>(int x,int y) where T: Node
    {
        Type randomNodeType = SelectRandomNode();
        NodeData<T> randomNodeData = new NodeData<T>((T)Activator.CreateInstance(randomNodeType), x, y);
        
        return randomNodeData;
    }

    private NodeData<T> GetNodeDataFromNode<T>(Node node,int posX,int posY) where T: Node
    {
        if (node == null)
        {
            NodeData<T> center = new NodeData<T>((T)Activator.CreateInstance(typeof(CenterNodes)), posX, posY);
            return center;
        }
        
        Type _tpyeNode = node.GetType();
        NodeData<T> _nodeData = new NodeData<T>((T)Activator.CreateInstance(_tpyeNode), posX, posY);
        return _nodeData;
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
    
    private bool CheckIsNodeExceedGridBorder(Vector2Int position)
    {
        foreach (var grids in _gridDataList)
        {
            if (position == grids.Position)
                return true;
        }
        return false;
    }
    
}