using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Node : MonoBehaviour
{
    public int Num;
    private EnemyControl agent;

    //確認是哪個玩家
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.instance;
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        agent = other.GetComponent<EnemyControl>();
        if (agent != null && agent.NowPoint == Num)
        {            
            if (gameManager.getMyPlayer() == GameManager.MyNowPlayer.player_1)
            {
                agent.touchPoint(Num + 1);
            }
            else if (gameManager.getMyPlayer() == GameManager.MyNowPlayer.player_2)
            {
                agent.touchPoint(Num - 1);
            }
        }
    }
}
