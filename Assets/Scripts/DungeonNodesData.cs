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
