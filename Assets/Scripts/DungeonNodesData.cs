using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonNodesData : MonoBehaviour
{
    public static DungeonNodesData Instance;

    private void Awake()
    {
        Instance = this;
    }

    public DungeonNodesSo _dungeonNodeData;
}
