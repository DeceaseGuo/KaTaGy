using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Node : MonoBehaviour
{
    public int Num;

    //確認是哪個玩家
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.instance;
    }

    public virtual void OnTriggerEnter(Collider other)
    {        
        var agent = other.gameObject.GetComponent<EnemyControl>();
        if (agent != null && agent.NowPoint == Num)
        {
           // Debug.Log("撞到了");
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
