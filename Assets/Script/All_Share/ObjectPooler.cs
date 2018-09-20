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
    GameManager.WhichObjectEnumComparer myEnumComparer = new GameManager.WhichObjectEnumComparer();
    private void Start()
    {
        producePool();
    }

    #region 產生物件池
    void producePool()
    {
        poolDictionary = new Dictionary<GameManager.whichObject, Queue<GameObject>>(myEnumComparer);

        for (int a = 0; a < pools.Count; a++)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pools[a].pool_amount; i++)
            {
                GameObject obj = null;
                if (pools[a].pool_Prefab.GetComponent<PhotonView>() == null)
                {
                    obj = Instantiate(pools[a].pool_Prefab);
                    obj.SetActive(false);
                    obj.transform.SetParent(transform);
                }
                else
                {
                    obj = PhotonNetwork.Instantiate(pools[a].filePath, Vector3.zero, Quaternion.identity, 0);
                    obj.GetComponent<PhotonView>().RPC("SetActiveF", PhotonTargets.All);
                }

                obj.transform.position = Vector3.zero;
                objectPool.Enqueue(obj);
            }
            poolDictionary.Add(pools[a].pool_Name, objectPool);
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
        if (poolDictionary[_name].Count == 0)//拿出來之後等於0就會增加
        {
            Repool(_name, PoolAddNewObj(_name));
        }

        objectToSpawn.transform.rotation = _rot;
        PhotonView Net = objectToSpawn.GetComponent<PhotonView>();

        if (Net == null)
        {
            objectToSpawn.transform.position = _pos;
            objectToSpawn.SetActive(true);
        }
        else
        {
            Net.RPC("SetActiveT", PhotonTargets.All, _pos);
        }
        return objectToSpawn;
    }
    #endregion

    #region 返回物件池
    public void Repool(GameManager.whichObject _name, GameObject _obj)
    {
        PhotonView Net = _obj.GetComponent<PhotonView>();
        if (Net == null)
        {
            _obj.SetActive(false);
            _obj.transform.SetParent(transform);
        }
        else
        {
            Net.RPC("SetActiveF", PhotonTargets.All);
        }
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
        }

        obj.transform.SetParent(transform);
        return obj;
    }
    #endregion
}
