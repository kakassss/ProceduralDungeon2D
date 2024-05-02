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
    private void SelectPointNodePosition()
    {
        var randomGridData = Random.Range(0, _gridDataList.Count);
        _currentPointNodeData = _gridDataList[100];
        _currentPointPosition = _currentPointNodeData.Position;
        
        Vector2Int _startNodePosition = Vector2Int.zero;
        Vector2Int _targetNodePosition = _currentPointPosition;
        
        Debug.Log("current point " + _currentPointPosition);

        List<Vector2Int> _pathPositionX = new List<Vector2Int>();
        List<Vector2Int> _pathPositionY = new List<Vector2Int>();
        List<Vector2Int> _mainPathPosition = new List<Vector2Int>();
        Vector2Int _corridorPosition;
        Node _randomCorridorNode;
        Node _corridorNode;
        int counter = 0;
        var nextNodePos = _pointZero;
        // new UpRightNodes(),    // 0,1 
        // new DownLeftNodes(),
        List<NodeData<Node>> nodesss = new List<NodeData<Node>>();
        // Assuming target vector be like (1,2) (-2,3) (-3,-5) (2,-4)
        if (_targetNodePosition.x != 0 && _targetNodePosition.y != 0)
        {
            var posX = _targetNodePosition.x;
            var posY = _targetNodePosition.y;
            //An example for (4,-4)
            if (posX > 0 && posY < 0)
            {
                //Random.Range(0,1)
                _corridorNode = _coordinateUpLeft[0];
                
                
                if (_corridorNode.GetType() == typeof(UpRightNodes))
                {
                    _corridorPosition = new Vector2Int(1, 0);
                    _targetNodePosition -= _corridorPosition;
                    
                    for (int i = 0; i < _targetNodePosition.x; i++)
                    {
                        _mainPathPosition.Add(new Vector2Int(1,0));
                    }

                    _mainPathPosition.Add(_corridorPosition);
                    
                    for (int i = 0; i < Mathf.Abs(_targetNodePosition.y); i++)
                    {
                        _mainPathPosition.Add(new Vector2Int(0,-1));
                    }

                    // foreach (var node in _mainPathPosition)
                    // {
                    //     Debug.Log("NodePosition " + node);
                    // }

                    for (int i = 0; i < _mainPathPosition.Count; i++)
                    {
                        nextNodePos += _mainPathPosition[counter];
                        NodeData<Node> placableNode = GetNodeDataFromNode<Node>(_selectedNode, nextNodePos.x, nextNodePos.y);
                        counter++;
                        nodesss.Add(placableNode);
                    }


                    foreach (var node in nodesss)
                    {
                        Debug.Log("CurrentNode Position " + node.Position);
                    }
                }

                if (_corridorNode.GetType() == typeof(DownLeftNodes))
                {
                    
                }

                
                

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

        }

    }
    
    
    private Node _selectedNode;
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
            
            Debug.Log("nextNode2 " + _nodePositionsList[i].x + " " + " " + _nodePositionsList[i].y);
            Instantiate(currentNodeData.node.NodeGameobject, 
                new Vector3(_nodePositionsList[i].x,_nodePositionsList[i].y,0),
                Quaternion.identity, transform);
        }
        
        
        
    }

    private List<NodeData<Node>> neighboorNodeDatas;
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