using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class DungeonNodeGenerator : MonoBehaviour
{
    [SerializeField] private NodeGameObjectDataProvider nodeGameObjectDataProvider;
    
    public int Width;
    public int Height;

    [SerializeField] private float corridorNodeRate = 0.5f;
    [SerializeField] private int _iterationCount = 100;
    
    private Vector2Int _pointZero = Vector2Int.zero;

    private readonly HashSet<Vector2Int> _nodePositionsHashSet = new HashSet<Vector2Int>();
    private List<Vector2Int> _nodePositionsList;
    private readonly List<NodeData<Node>> _nodeDataList = new List<NodeData<Node>>();
    private readonly List<NodeData<Node>> _gridDataList = new List<NodeData<Node>>();
    
    private Grid _gridData;
    
    private int _halfOfWidth;
    private int _halfOfHeight;


    public Node UpNode { get; private set; } = new UpNodes();
    public Node DownNode { get; private set; } = new DownNodes();
    public Node LeftNode { get; private set; } = new LeftNodes();
    public Node RightNode { get; private set; } = new RightNodes();
    public Node UpDownNode { get; private set; } = new UpDownNodes();
    public Node UpRightNode { get; private set; } = new UpRightNodes();
    public Node UpLeftNode { get; private set; } = new UpLeftNodes();
    public Node DownLeftNode { get; private set; } = new DownLeftNodes();
    public Node DownRightNode { get; private set; } = new DownRightNodes();
    public Node RightLeftNode { get; private set; } = new RightLeftNodes();
    


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
        SelectPointNodePosition();

        // for (int i = 0; i < _gridDataList.Count; i++)
        // {
        //     Debug.Log("GridData " + i + " " + _gridDataList[i].Position);
        // }
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

    private void SetNodePositionData(Vector2Int nodePosition)
    {
        foreach (var grids in _gridDataList.Where(grids => nodePosition == grids.Position))
        {
            grids.IsEmpty = false;
        }
    }
    /*
     *  GOING TO REVISION ON MAIN MECHANIC  
     *  Normalde pozisyonları 0,0'dan başlatarak random vectorler ekleyerek ilerletirdin.
     *  Günün sonunda hep random node yerleştirme düşünerek yaptıgımızdan dolayı, şimdi fark ediyoruz ki
     *  bunları birleştirmek zor, yani günün sonunda hepsi centerNode olur.
     *  Hepsi kendi çevresine baktıgında en mantıklısı centerNode oluyor, eğer koridor mantıgı gelirse
     *  diğer tarafdakiler patlıyor.
     *
     *
     * şuan yapmak istediğin şu
     * grid data listinden random bir node çekeceksin atıyorum, (5,4) node'u geldi
     * 0,0'dan oraya gitmen için bi path oluşacak, işte örnek olarak
     * 1,0 1,0 1,0 1,0 1,0 0,1 0,1 0,1 0,1 0,1 = 5,4 gibi
     * 5 tane rightToLeft corridor 1 tane right up 3 tane updown corridor'dan oluşacak
     * bunu 5-6 kere tekrar ediceksin.
     * günün sonunda 5,4 çıkmalı bu vectorlerın toplamı buraya dikkat et
     * örnek videodaki adamın algoritması böyle gibi gözüküyor
     * centernode hep ortada, 3-4 tane path var bunlar birbirini tamamlıyor ve birbirine ellemiyor.
     * 
     */
    private readonly Node[] _coordinateUpRight = 
    {                                               
        new DownRightNodes(),    // 0,1 
        new UpLeftNodes(),  // 0,-1 
    };
    private readonly Node[] _coordinateUpLeft = 
    {                                               
        new UpRightNodes(),    // 0,1 
        new DownLeftNodes(),  // 0,-1 
    };
    private readonly Vector2Int[] _coordinateDownRight = 
    {                                               
        Vector2Int.up,    // 0,1 
        Vector2Int.down,  // 0,-1 
    };
    private readonly Vector2Int[] _coordinateDownLeft = 
    {                                               
        Vector2Int.up,    // 0,1 
        Vector2Int.down,  // 0,-1 
    };
    
    private Vector2Int _currentPointPosition;
    private NodeData<Node> _currentPointNodeData;
    private Node _selectedNode;
    private List<Node> _xNodes = new();
    private List<Node> _yNodes = new();

    private Node SetSelectedNodes(Vector2Int nodeDirection)
    {
        if (nodeDirection.x is 1 or -1 && nodeDirection.y == 0)
        {
            var rightLeftNode = new RightLeftNodes();
            _xNodes.Add(rightLeftNode);
            return rightLeftNode;
        }
        
        if (nodeDirection.y is 1 or -1 && nodeDirection.x == 0)
        {
            var upDownNode = new UpDownNodes();
            _yNodes.Add(upDownNode);
            return upDownNode;
        }
        
        Debug.LogError("CenterNode has returned!");
        return new CenterNodes();
    }
    
    private void SelectPointNodePosition()
    {
        //var randomGridData = Random.Range(0, _gridDataList.Count);
        var minRandomGridData = Random.Range(0 ,34);   // These array groups has a bigger numbers except 
        var maxRandomGridData = Random.Range(86, 120); // other them.
        _currentPointNodeData = _gridDataList[100]; // ---> (4,-4)
        _currentPointPosition = _currentPointNodeData.Position;
        
        Vector2Int startNodePosition = Vector2Int.zero;
        Vector2Int targetNodePosition = _currentPointPosition;
        

        List<Vector2Int> mainPathPosition = new List<Vector2Int>();
        List<NodeData<Node>> pathNodes = new List<NodeData<Node>>();
        Vector2Int corridorPosition;
        
        Node corridorNode;
        var nextNodePos = startNodePosition;
        int corner = Random.Range(0,2);
        bool cornerSelect = corner == 0;
        
        NodeData<CenterNodes> centerNode = GetNodeDataFromNode<CenterNodes>(_selectedNode, nextNodePos.x, nextNodePos.y);
        centerNode.node.NodeGameobject = nodeGameObjectDataProvider.GetCurrentNodeGO(centerNode.node);
                    
        Instantiate(centerNode.node.NodeGameobject, new Vector3(centerNode.Position.x,centerNode.Position.y,0),
            Quaternion.identity, transform);
        
        
        // Assuming target vector be like (1,2) (-2,3) (-3,-5) (2,-4)
        if (targetNodePosition.x == 0 || targetNodePosition.y == 0) return;
        
        var posX = targetNodePosition.x;
        var posY = targetNodePosition.y;
        
        
        //An example for (4,-4)
        if (posX > 0 && posY < 0)
        {
            // corridorNode = _coordinateUpLeft[0];
            //     
            // if (corridorNode.GetType() == typeof(UpRightNodes))
            // {
                corridorPosition = new Vector2Int(1, 0);
                targetNodePosition -= corridorPosition;
                
                SetTargetPositionNodes();
                
                //Create NodesDatas and declare their positions
                for (int i = 0; i < mainPathPosition.Count; i++)
                {
                    nextNodePos += mainPathPosition[i];
                    _selectedNode = SetSelectedNodes(mainPathPosition[i]);
                    NodeData<Node> placableNode = GetNodeDataFromNode<Node>(_selectedNode, nextNodePos.x, nextNodePos.y);
                    pathNodes.Add(placableNode);
                }
                
                //Get and set edgeNodes datas
                var xEdgeNode = pathNodes[_xNodes.Count-1];
                xEdgeNode.node = cornerSelect ? DownLeftNode : UpRightNode;
                var yEdgeNode = pathNodes[^1];
                yEdgeNode.node = cornerSelect ? UpNode: LeftNode;
                
                //Instantiate NodeData Gameobjects
                foreach (var nodeData in pathNodes)
                {
                    nodeData.node.NodeGameobject = nodeGameObjectDataProvider.GetCurrentNodeGO(nodeData.node);
                    Instantiate(nodeData.node.NodeGameobject, new Vector3(nodeData.Position.x,nodeData.Position.y,0),
                        Quaternion.identity, transform);
                }
                
                foreach (var node in pathNodes)
                {
                    Debug.Log("CurrentNode Position " + node.Position);
                }
            //}

            // if (corridorNode.GetType() == typeof(DownLeftNodes))
            // {
            //         
            // }
            
        }
        //An example for (1,2)
        if (posX > 0 && posY > 0)
        {
                
        }
        //An example for (-2,3)
        if (posX < 0 && posY > 0)
        {
                
        }
        //An example for (-3,-5)
        if (posX < 0 && posY < 0)
        {
                
        }

        
        void SetTargetPositionNodes()
        {
            if (cornerSelect)
            {
                for (int i = 0; i < targetNodePosition.x; i++)
                {
                    mainPathPosition.Add(new Vector2Int(1,0));
                }
                    
                mainPathPosition.Add(corridorPosition);
                
                for (int i = 0; i < Mathf.Abs(targetNodePosition.y); i++)
                {
                    mainPathPosition.Add(new Vector2Int(0,-1));
                }
            }
            else
            {
                for (int i = 0; i < Mathf.Abs(targetNodePosition.y); i++)
                {
                    mainPathPosition.Add(new Vector2Int(0,-1));
                }
                    
                mainPathPosition.Add(corridorPosition);
                    
                for (int i = 0; i < targetNodePosition.x; i++)
                {
                    mainPathPosition.Add(new Vector2Int(1,0));
                }
            }
        }
        
    }
    
    
    
    private IEnumerator CreateDungeon()
    {
        // NodeData<CenterNodes> initNode = new NodeData<CenterNodes>(new CenterNodes(),_pointZero.x,_pointZero.y);
        // initNode.node.NodeGameobject = nodeGameObjectDataProvider.GetCurrentNodeGO(initNode.node);
        // _selectedNode = initNode.node;
        var nextNodePos = _pointZero;
        
        //
        // var nodeGo = initNode.node.NodeGameobject;
        
        //Instantiate(nodeGo, new Vector3(initNode.PosX,initNode.PosY,0),Quaternion.identity,transform);
        
        //_nodePositionsHashSet.Add(nextNodePos);
        //nextNodePos += GetRandomDirection();
        
        for (int i = 0; i < _iterationCount; i++)
        {
            yield return new WaitForSeconds(0.001f);
            
            if (CheckIsNodeExceedGridBorder(nextNodePos) == false)
            {
                Debug.Log("||||||||||||||||||||||||||||||||||||||||||");
                nextNodePos = _pointZero;
                continue;
            }
            
            NodeData<Node> placableNode = GetNodeDataFromNode<Node>(_selectedNode, nextNodePos.x, nextNodePos.y);
            SetNodePositionData(placableNode.Position);
            Debug.Log("nextNode1 " + nextNodePos);
            Debug.Log("nextNode2 " + placableNode.Position);
            _selectedNode = CheckOpenTransformPositions(placableNode);
            
            
            
            //var randomNodeTemp = GetRandomNodeData<Node>(nextNodePos.x,nextNodePos.y);
            _nodePositionsHashSet.Add(nextNodePos);
            _nodeDataList.Add(placableNode);
            
             
            //var mynode = CheckOpenTransformPositions(placableNode);
            nextNodePos += GetRandomDirection();
           
        }
        
        //Transfer to the list
        _nodePositionsList = new List<Vector2Int>(_nodePositionsHashSet);
        //SetNodePositionData(nodePositionsList);
        
        
        //Get Random Node and Instantiate
        for (int i = 0; i < _nodePositionsList.Count; i++)
        {
            var currentNodeData = _nodeDataList[i];
            currentNodeData.node.NodeGameobject = nodeGameObjectDataProvider.GetCurrentNodeGO(currentNodeData.node);
            // var randomNodeData = GetRandomNodeData<Node>(nextNodePos.x,nextNodePos.y);
            // randomNodeData.node.NodeGameobject = nodeGameObjectDataProvider.GetCurrentNodeGO(randomNodeData.node);
            
            //CheckOpenPositions(randomNodeData.node);
            
            //nodeDataList.Add(randomNodeData);
            
            Instantiate(currentNodeData.node.NodeGameobject, 
                new Vector3(_nodePositionsList[i].x,_nodePositionsList[i].y,0),
                Quaternion.identity, transform);
        }
        
        
        
    }

    private List<NodeData<Node>> neighboorNodeDatas;

    [SerializeField] private Node _upNodes;
    // private Node CheckOpenTransformPositions<T>(NodeData<T> currentNode) where T : Node
    // {
    //     neighboorNodeDatas = new List<NodeData<Node>>();
    //     
    //     var UpNodePos = currentNode.Position + Vector2Int.up;
    //     var DownNodePos = currentNode.Position + Vector2Int.down;
    //     var LeftNodePos = currentNode.Position + Vector2Int.left;
    //     var RightNodePos = currentNode.Position + Vector2Int.right;
    //     
    //     Debug.Log("currentNode " + currentNode.Position);
    //     Debug.Log("UpNodePos " + UpNodePos);
    //     Debug.Log("DownNodePos " + DownNodePos);
    //     Debug.Log("LeftNodePos " + LeftNodePos);
    //     Debug.Log("RightNodePos " + RightNodePos);
    //     
    //     
    //     Debug.Log("------------------------------------------");
    //     
    //     var upNodeData = NodeHelperMethods.GetNodeAtXPosition(UpNodePos, _nodeDataList);
    //     var downNodeData = NodeHelperMethods.GetNodeAtXPosition(DownNodePos, _nodeDataList);
    //     var leftNodeData = NodeHelperMethods.GetNodeAtXPosition(LeftNodePos, _nodeDataList);
    //     var rightNodeData = NodeHelperMethods.GetNodeAtXPosition(RightNodePos, _nodeDataList);
    //
    //     Debug.Log("upNodeData " + upNodeData.IsUnityNull());
    //     Debug.Log("downNodeData " + downNodeData.IsUnityNull());
    //     Debug.Log("leftNodeData " + leftNodeData.IsUnityNull());
    //     Debug.Log("rightNodeData " + rightNodeData.IsUnityNull());
    //     
    //     Debug.Log("------------------------------------------");
    //     
    //     if(upNodeData != null) neighboorNodeDatas.Add(upNodeData);
    //     if(downNodeData != null) neighboorNodeDatas.Add(downNodeData);
    //     if(leftNodeData != null) neighboorNodeDatas.Add(leftNodeData);
    //     if(rightNodeData != null) neighboorNodeDatas.Add(rightNodeData);
    //
    //     List<Node> mynodes = new List<Node>();
    //     
    //     if (neighboorNodeDatas.Count == 0) return new CenterNodes();
    //     
    //     foreach (var datas in neighboorNodeDatas)
    //     {
    //         var aa = CheckOpenPositions(datas.node);
    //         mynodes?.Add(aa);
    //     }
    //     
    //     if (mynodes.Count > 1)
    //     {
    //         var currentNodeCount = mynodes.Count;
    //         return mynodes[Random.Range(0, currentNodeCount)];
    //     }
    //
    //     return mynodes[0];
    // }
    
    private Node CheckOpenTransformPositions(NodeData<Node> currentNode)
    {
        neighboorNodeDatas = new List<NodeData<Node>>();
        
        var UpNodePos = currentNode.Position + Vector2Int.up;
        var DownNodePos = currentNode.Position + Vector2Int.down;
        var LeftNodePos = currentNode.Position + Vector2Int.left;
        var RightNodePos = currentNode.Position + Vector2Int.right;
        
        Debug.Log("currentNode name " + currentNode.node);
        Debug.Log("currentNode " + currentNode.Position);
        Debug.Log("UpNodePos " + UpNodePos);
        Debug.Log("DownNodePos " + DownNodePos);
        Debug.Log("LeftNodePos " + LeftNodePos);
        Debug.Log("RightNodePos " + RightNodePos);
        
        Debug.Log("------------------------------------------");
        
        var upNodeData = NodeHelperMethods.GetNodeAtXPosition(UpNodePos, _nodeDataList);
        var downNodeData = NodeHelperMethods.GetNodeAtXPosition(DownNodePos, _nodeDataList);
        var leftNodeData = NodeHelperMethods.GetNodeAtXPosition(LeftNodePos, _nodeDataList);
        var rightNodeData = NodeHelperMethods.GetNodeAtXPosition(RightNodePos, _nodeDataList);
        
        Debug.Log("upNodeData " + upNodeData.IsUnityNull());
        Debug.Log("downNodeData " + downNodeData.IsUnityNull());
        Debug.Log("leftNodeData " + leftNodeData.IsUnityNull());
        Debug.Log("rightNodeData " + rightNodeData.IsUnityNull());
        
        Debug.Log("------------------------------------------");
        
        if(upNodeData != null) neighboorNodeDatas.Add(upNodeData);
        if(downNodeData != null) neighboorNodeDatas.Add(downNodeData);
        if(leftNodeData != null) neighboorNodeDatas.Add(leftNodeData);
        if(rightNodeData != null) neighboorNodeDatas.Add(rightNodeData);
        
        if (neighboorNodeDatas.Count == 0) return new CenterNodes();
        List<Node> _sendableNodes = new List<Node>();
        
        foreach (var datas in neighboorNodeDatas)
        {
            var _validNodes = CheckOpenPositions(datas.node);
            _sendableNodes.Add(_validNodes);
        }

        if (_sendableNodes.Count <= 1) return _sendableNodes[0];
        
        var currentNodeCount = _sendableNodes.Count;
        return _sendableNodes[Random.Range(0, currentNodeCount)];

    }
    
     // NodeData: 0 is open, 1 is close
    private Node CheckOpenPositions(Node node)
    {
        var currentNode = node.Direction;
        List<Node> _canPlacableNodes = new List<Node>();
        
        //Check Left and Right possibilities

        
        
        
        if (currentNode.DirectionX.x == 0)
        {
            //Left node area is open
            _canPlacableNodes.Add(new RightNodes());
            _canPlacableNodes.Add(new DownLeftNodes());
        }
        else if (currentNode.DirectionX.y == 0)
        {
            //Right node area is open
            _canPlacableNodes.Add(new LeftNodes());
            _canPlacableNodes.Add(new DownLeftNodes());
            _canPlacableNodes.Add(new DownRightNodes());
        }
        
        if (_canPlacableNodes.Count > 1)
        {
            var currentNodeCount = _canPlacableNodes.Count;
            return _selectedNode = _canPlacableNodes[Random.Range(0, currentNodeCount)];
        }
        
        
        if (currentNode.DirectionY.x == 0)
        {
            //Up node area is open, down closed
            //_canPlacableNodes.Add(new DownNodes());
            _canPlacableNodes.Add(new DownLeftNodes());
            _canPlacableNodes.Add(new DownRightNodes());
            _canPlacableNodes.Add(new UpDownNodes());
        }
        else if (currentNode.DirectionY.y == 0)
        {
            //Down node area is open, up closed 
            //_canPlacableNodes.Add(new UpNodes());
            _canPlacableNodes.Add(new UpRightNodes());
            _canPlacableNodes.Add(new UpLeftNodes());
            _canPlacableNodes.Add(new UpDownNodes());
        }

        if (_canPlacableNodes.Count > 1)
        {
            var currentNodeCount = _canPlacableNodes.Count;
            return _selectedNode = _canPlacableNodes[Random.Range(0, currentNodeCount)];
        }
        
        return _selectedNode = _canPlacableNodes[0];
        
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
}