//using TowerDefense.Agents;
using TowerDefense.MeshCreator;

    /// <summary>
    /// A point along the path which agents will navigate towards before recieving the next instruction from the NodeSelector
    /// Requires a collider to be added manually.
    /// </summary>
using UnityEngine;
[RequireComponent(typeof(Collider))]
public class Node : MonoBehaviour
{
    /// <summary>
    /// Reference to the MeshObject created by an AreaMeshCreator
    /// </summary>
    //[HideInInspector]
    public AreaMeshCreator areaMesh;
    public int Num;

    //確認是哪個玩家
    private GameManager gameManager;

    /// <summary>
    /// Selection weight of the node
    /// </summary>
    public int weight = 1;

    private void Start()
    {
        gameManager = GameManager.instance;
    }

    /// <summary>
    /// Gets a random point inside the area defined by a node's meshcreator
    /// </summary>
    /// <returns>A random point within the MeshObject's area</returns>
    public Vector3 GetRandomPointInNodeArea()
    {
        // Fallback to our position if we have no mesh
        return areaMesh == null ? transform.position : areaMesh.GetRandomPointInside();
    }

    /// <summary>
    /// When agent enters the node area, get the next node
    /// </summary>
    public virtual void OnTriggerEnter(Collider other)
    {
        Debug.Log("撞到了");
        var agent = other.gameObject.GetComponent<EnemyControl>();
        if (agent != null)
        {
            if (gameManager.getMyPlayer() == GameManager.MyNowPlayer.player_1)
            {
                agent.touchPoint(Num + 1, true);
            }
            else if (gameManager.getMyPlayer() == GameManager.MyNowPlayer.player_2)
            {
                agent.touchPoint(Num - 1, true);
            }
        }
    }


#if UNITY_EDITOR
    /// <summary>
    /// Ensure the collider is a trigger
    /// </summary>
    protected void OnValidate()
    {
        var trigger = GetComponent<Collider>();
        if (trigger != null)
        {
            trigger.isTrigger = true;
        }

        // Try and find AreaMeshCreator
        if (areaMesh == null)
        {
            areaMesh = GetComponentInChildren<AreaMeshCreator>();
        }
    }
#endif
}
