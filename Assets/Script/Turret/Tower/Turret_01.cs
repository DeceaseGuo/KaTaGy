using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret_01 : MonoBehaviour
{
    private EnemyManager enemyManager;
    [SerializeField] EnemyBornPoint enemyBornPoint;
    [SerializeField] float hp;

    private void OnEnable()
    {
        enemyManager = EnemyManager.instance;
        enemyBornPoint = enemyManager.getBornQueue();
        enemyManager.addBornPoint();
        enemyManager.nextSoldier();
    }

    private void Update()
    {
        if (Input.GetKeyDown("i"))
        {
            takeDamage(10);
        }
    }

    void takeDamage(float _damage)
    {
        if (hp > 0)
        {
            hp -= _damage;
        }

        if (hp <= 0)
        {
            enemyManager.removeBornPoint(enemyBornPoint);
            Destroy(this.gameObject);
        }
    }
}
