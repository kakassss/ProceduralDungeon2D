using UnityEngine;

public class NodeGameObjectDataProvider : MonoBehaviour
{
    [SerializeField] private DungeonNodesSo nodeDataSO;
    
    public GameObject GetCurrentNodeGO(Node node)
    {
        var nodeDatas = nodeDataSO.Nodes;

        return node switch
        {
            CenterNodes => nodeDatas.GetCurrentNode(nodeDatas.CenterNodesList),
            UpNodes => nodeDatas.GetCurrentNode(nodeDatas.UpNodesList),
            DownNodes => nodeDatas.GetCurrentNode(nodeDatas.DownNodesList),
            LeftNodes => nodeDatas.GetCurrentNode(nodeDatas.LeftNodesList),
            RightNodes => nodeDatas.GetCurrentNode(nodeDatas.RightNodesList),
            _ => null
        };
    }
    
    
}
