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
            
            UpDownNodes => nodeDatas.GetCurrentNode(nodeDatas.UpDownNodesList),
            UpRightNodes => nodeDatas.GetCurrentNode(nodeDatas.UpRightNodesList),
            UpLeftNodes => nodeDatas.GetCurrentNode(nodeDatas.UpLeftNodesList),
            DownLeftNodes => nodeDatas.GetCurrentNode(nodeDatas.DownLeftNodesList),
            DownRightNodes => nodeDatas.GetCurrentNode(nodeDatas.DownRightNodesList),
            RightLeftNodes => nodeDatas.GetCurrentNode(nodeDatas.RightLeftNodesList),
            
            UpCloseNodes => nodeDatas.GetCurrentNode(nodeDatas.UpCloseNodesList),
            DownCloseNodes => nodeDatas.GetCurrentNode(nodeDatas.DownCloseNodesList),
            RightCloseNodes => nodeDatas.GetCurrentNode(nodeDatas.RightCloseNodesList),
            LeftCloseNodes => nodeDatas.GetCurrentNode(nodeDatas.LeftCloseNodesList),
            
            _ => null
        };
    }
    
    
}
