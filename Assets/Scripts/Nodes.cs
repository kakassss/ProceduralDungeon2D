using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Nodes
{
    public List<CenterNodes> CenterNodesList;
    public List<UpNodes> UpNodesList;
    public List<DownNodes> DownNodesList;
    public List<RightNodes> RightNodesList;
    public List<LeftNodes> LeftNodesList;
    
    public GameObject GetCurrentNode<T>(List<T> currentNodes) where T : Node
    {
        return currentNodes[Random.Range(0,currentNodes.Count)].NodeGameobject;
    }
}