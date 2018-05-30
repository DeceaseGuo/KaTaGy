using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HintManager : MonoBehaviour
{
    public static HintManager instance;
    [SerializeField] float delayTime;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    #region 從物件池創造提示文字
    public void CreatHint(string _content)
    {
        GameObject hintObj = ObjectPooler.instance.getPoolObject(GameManager.whichObject.HintText, Vector3.zero, Quaternion.identity);
        hintObj.GetComponent<Text>().text = _content;
        hintObj.transform.SetParent(this.transform);
        StartCoroutine(delayClose(hintObj));
    }
    #endregion

    #region 延遲回物件池
    IEnumerator delayClose(GameObject _obj)
    {
        yield return new WaitForSeconds(delayTime);
        ObjectPooler.instance.Repool(GameManager.whichObject.HintText, _obj);
    }
    #endregion
}
