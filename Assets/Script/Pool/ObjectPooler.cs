using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    #region 單例模式
    public static ObjectPooler instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    #endregion

    [System.Serializable]
    public class pool
    {
        public GameManager.whichObject pool_Name;
        public GameObject pool_Prefab;
        public int pool_amount;
        public string filePath;
    }

    [SerializeField] List<pool> pools;
    [SerializeField] Dictionary<GameManager.whichObject, Queue<GameObject>> poolDictionary;

    private void Start()
    {
        producePool();
    }

    #region 產生物件池
    void producePool()
    {
        poolDictionary = new Dictionary<GameManager.whichObject, Queue<GameObject>>();

        foreach (pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.pool_amount; i++)
            {
                GameObject obj = null;
                if (pool.pool_Prefab.GetComponent<PhotonView>() != null)
                {
                    obj = PhotonNetwork.Instantiate(pool.filePath, Vector3.zero, Quaternion.identity, 0);
                    obj.GetComponent<PhotonView>().RPC("SetActiveF", PhotonTargets.All);
                    obj.GetComponent<PhotonView>().RPC("SetPoolparent", PhotonTargets.All);
                }
                else
                {
                    obj = Instantiate(pool.pool_Prefab);
                    obj.SetActive(false);
                    obj.transform.SetParent(transform);
                }

                obj.transform.position = Vector3.zero;
                objectPool.Enqueue(obj);
            }
            poolDictionary.Add(pool.pool_Name, objectPool);
        }
    }
    #endregion

    #region 取得物件池
    public GameObject getPoolObject(GameManager.whichObject _name, Vector3 _pos, Quaternion _rot)
    {
        if (!poolDictionary.ContainsKey(_name))
        {
            Debug.Log("物件池沒有啦 耖");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[_name].Dequeue();
        if (poolDictionary[_name].Count == 0)
        {
            GameObject newObjToSpawn = PoolAddNewObj(_name);
            Repool(_name, newObjToSpawn);
        }

        objectToSpawn.transform.rotation = _rot;

        if (objectToSpawn.GetComponent<PhotonView>() == null)
        {
            objectToSpawn.transform.position = _pos;
            objectToSpawn.SetActive(true);
        }
        else
        {
            objectToSpawn.GetComponent<PhotonView>().RPC("SetActiveT", PhotonTargets.All, _pos);
        }
        return objectToSpawn;
    }
    #endregion

    #region 返回物件池
    public void Repool(GameManager.whichObject _name, GameObject _obj)
    {
        PhotonView photon_Script = _obj.GetComponent<PhotonView>();
        if (photon_Script == null)
            _obj.SetActive(false);
        else
            photon_Script.RPC("SetActiveF", PhotonTargets.All);

        poolDictionary[_name].Enqueue(_obj);
    }
    #endregion

    #region 物件池內物件不夠時→增加一個
    GameObject PoolAddNewObj(GameManager.whichObject _name)
    {
        pool _pool = pools.Find(x => x.pool_Name == _name);
        GameObject obj = null;

        if (_pool.pool_Prefab.GetComponent<PhotonView>() == null)
        {
            obj = Instantiate(_pool.pool_Prefab);
            obj.SetActive(false);
            obj.transform.SetParent(this.transform);
        }
        else
        {
            obj = PhotonNetwork.Instantiate(_pool.filePath, Vector3.zero, Quaternion.identity, 0);
            obj.GetComponent<PhotonView>().RPC("SetActiveF", PhotonTargets.All);
            obj.GetComponent<PhotonView>().RPC("SetPoolparent", PhotonTargets.All);
        }

        obj.transform.SetParent(transform);
        return obj;
    }
    #endregion
}
