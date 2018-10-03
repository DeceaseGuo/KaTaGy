using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HintManager : MonoBehaviour
{
    public static HintManager instance;
    private float delayTime = 1.5f;

    private ObjectPooler poolManager;
    private ObjectPooler PoolManager { get { if (poolManager == null) poolManager = ObjectPooler.instance; return poolManager; } }

    #region 緩存
    private Transform myCachedTransform;
    private GameObject hintObj;
    private GameManager.whichObject tmpHint;
    #endregion

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        myCachedTransform = this.transform;
    }

    private void Start()
    {
        tmpHint = GameManager.whichObject.HintText;
    }

    #region 從物件池創造提示文字
    public void CreatHint(string _content)
    {
        hintObj = PoolManager.getPoolObject(tmpHint, Vector3.zero, Quaternion.identity);
        hintObj.GetComponent<Text>().text = _content;
        hintObj.transform.SetParent(myCachedTransform);
        StartCoroutine(delayClose(hintObj));
    }
    #endregion

    #region 延遲回物件池
    IEnumerator delayClose(GameObject _obj)
    {
        yield return new WaitForSeconds(delayTime);
        PoolManager.Repool(GameManager.whichObject.HintText, _obj);
    }
    #endregion
}
