using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Nodes
{
    [Header("4-Way Nodes")]
    public List<CenterNodes> CenterNodesList;
    
    [Header("1-Way Nodes")]
    public List<UpNodes> UpNodesList;
    public List<DownNodes> DownNodesList;
    public List<RightNodes> RightNodesList;
    public List<LeftNodes> LeftNodesList;
    
    [Header("2-Way Nodes")]
    public List<UpDownNodes> UpDownNodesList;
    public List<UpRightNodes> UpRightNodesList;
    public List<UpLeftNodes> UpLeftNodesList;
    public List<DownLeftNodes> DownLeftNodesList;
    public List<DownRightNodes> DownRightNodesList;
    public List<RightLeftNodes> RightLeftNodesList;
    
    [Header("3-Way Nodes")]
    public List<UpCloseNodes> UpCloseNodesList;
    public List<DownCloseNodes> DownCloseNodesList;
    public List<LeftCloseNodes> LeftCloseNodesList;
    public List<RightCloseNodes> RightCloseNodesList;
    
    public GameObject GetCurrentNode<T>(List<T> currentNodes) where T : Node
    {
        return currentNodes[Random.Range(0,currentNodes.Count)].NodeGameobject;
    }
}