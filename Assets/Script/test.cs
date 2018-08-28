using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using MyCode.Timer;
using UnityEngine.AI;
namespace Mytest
{
    public class test : MonoBehaviour
    {

        public enum kkk :int
        {
            A=0,
            B=1,
            C=2,
            D=3
        }

        public kkk nowTest;

        NavMeshAgent nav;
        [SerializeField] Transform pos;
        private void Start()
        {
            nav = GetComponent<NavMeshAgent>();
        }

        void SetPos()
        {
            if (pos != null)
                nav.SetDestination(pos.position);
        }

        public EnemyControl aaa;
        public GameObject target;

        private void Update()
        {
            /* if (Input.GetKeyDown(KeyCode.Alpha1))
             {
                 kkk.Invoke();
                 Debug.Log("click");
             }*/

            
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
               /* aaa.currentTarget = target;
                aaa.nowState = EnemyControl.states.Atk;
                StartCoroutine(aaa.enemyAttack());*/
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            { 
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {

                

            }

        }

    }
}
