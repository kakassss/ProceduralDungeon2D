using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NodeRefactor
{
    public List<GameObject> DestroyGOList = new List<GameObject>();
    
    
    public void SetInstantiatedNodeDatas(Transform transform,DungeonNodeGenerator _nodeGenerator,NodeGameObjectDataProvider nodeGameObjectDataProvider)
    {
        var nodeList = _nodeGenerator._allInstantiatedNodes;
        
        for (int i = 0; i < _nodeGenerator._allInstantiatedNodes.Count; i++)
        {
            nodeList[i].node = SetNodeAfterInstatiated(_nodeGenerator._allInstantiatedNodes[i],i,_nodeGenerator);
            nodeList[i].node.NodeGameobject = nodeGameObjectDataProvider.GetCurrentNodeGO(nodeList[i].node);
            var go = GameObject.Instantiate(nodeList[i].node.NodeGameobject, new Vector3(nodeList[i].Position.x,nodeList[i].Position.y,0),
                Quaternion.identity, transform) as GameObject;
        }
    }
    
    private Node SetNodeAfterInstatiated(NodeData<Node> currentNode, int currentIndex,DungeonNodeGenerator _nodeGenerator)
    {
        
        
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
        
        var upNodeData = NodeHelperMethods.GetNodeAtXPosition(UpNodePos, _nodeGenerator._allInstantiatedNodes);
        var downNodeData = NodeHelperMethods.GetNodeAtXPosition(DownNodePos, _nodeGenerator._allInstantiatedNodes);
        var leftNodeData = NodeHelperMethods.GetNodeAtXPosition(LeftNodePos, _nodeGenerator._allInstantiatedNodes);
        var rightNodeData = NodeHelperMethods.GetNodeAtXPosition(RightNodePos, _nodeGenerator._allInstantiatedNodes);
        
        Debug.Log("upNodeData " + upNodeData.IsUnityNull());
        Debug.Log("downNodeData " + downNodeData.IsUnityNull());
        Debug.Log("leftNodeData " + leftNodeData.IsUnityNull());
        Debug.Log("rightNodeData " + rightNodeData.IsUnityNull());
        
        Debug.Log("------------------------------------------");

        if (upNodeData != null && downNodeData != null && leftNodeData != null && rightNodeData != null && DestroyGOList != null)
        {
            Destroy(_nodeGenerator._allInstantiatedNodesGO[currentIndex]);
            return NodeDataProvider.CenterNode;
        }
        if (downNodeData != null && leftNodeData != null && rightNodeData != null && DestroyGOList != null)
        {
            Destroy(_nodeGenerator._allInstantiatedNodesGO[currentIndex]);
            return NodeDataProvider.UpCloseNode;
        }
        if (upNodeData != null && leftNodeData != null && rightNodeData != null && DestroyGOList != null)
        {
            Destroy(_nodeGenerator._allInstantiatedNodesGO[currentIndex]);
            return NodeDataProvider.DownCloseNode;
        }
        if (upNodeData != null && downNodeData != null  && rightNodeData != null && DestroyGOList != null)
        {
            Destroy(_nodeGenerator._allInstantiatedNodesGO[currentIndex]);
            return NodeDataProvider.LeftCloseNode;
        }
        if (upNodeData != null && downNodeData != null && leftNodeData != null && DestroyGOList != null)
        {
            Destroy(_nodeGenerator._allInstantiatedNodesGO[currentIndex]);
            return NodeDataProvider.RightCloseNode;
        }

        return currentNode.node; // Todo: refactor 
    }

    private void Destroy(GameObject go)
    {
        DestroyGOList.Add(go);
    }
    
}
