using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class NodeHelperMethods 
{
    public static Vector2Int GetXNodePosition<T>(NodeData<T> nodeData) where T: Node
    {
        return nodeData.Position;
    }
    
    public static NodeData<Node> GetNodeAtXPosition(Vector2Int position,List<NodeData<Node>> nodeDataList)
    {
        return nodeDataList.FirstOrDefault(nodeData => nodeData.Position == position);
        //Debug.LogError("At Given Position Node can not found");
    }
    
    public static NodeData<Node> GetNodeDataAtXGrid(List<NodeData<Node>> nodeDataList,Vector2Int gridPos)
    {
        foreach (var node in nodeDataList.Where(node => node.Position == gridPos))
        {
            return node;
        }
        Debug.LogError("At Given Position NodeData can not found");
        return null;
    }
}
