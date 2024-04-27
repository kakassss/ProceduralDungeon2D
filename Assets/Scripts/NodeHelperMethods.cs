using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class NodeHelperMethods 
{
    private static Vector2Int GetXNodePosition<T>(NodeData<T> nodeData) where T: Node
    {
        return nodeData.Position;
    }

    private static NodeData<Node> GetNodeAtXPosition(Vector2Int position,List<NodeData<Node>> nodeDataList)
    {
        foreach (var nodeData in nodeDataList.Where(nodeData => nodeData.Position == position))
        {
            return nodeData;
        }
        Debug.LogError("At Given Position Node can not found");
        return null;
    }
}
