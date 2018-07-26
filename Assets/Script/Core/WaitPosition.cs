using UnityEngine;
using UnityEngine.UI;

public class WaitPosition : MonoBehaviour
{
    private MyCore coreManager;
    private MyCore CoreManager { get { if (coreManager == null) coreManager = MyCore.instance; return coreManager; } }
    private UpdateManager updateManager;
    private UpdateManager UpdateScript { get { if (updateManager == null) updateManager = UpdateManager.instance; return updateManager; } }

    private CoreSort selectSort;
    
    [SerializeField] Image myImage;
    [SerializeField] Image myCDBar;
    private Coroutine cnacle;
    private int cancelMoney = 0;

    //取消升級
    public void CancelUpdate()
    {
        if (selectSort != null)
        {
            StopCoroutine(cnacle);
            cnacle = null;
            selectSort.nowUpdate = false;
            //返回一部分錢
            ResetData();
        }
    }

    public void SetData(CoreSort _sort)
    {
        selectSort = _sort;
        cancelMoney = selectSort.needMoney;
        myImage.sprite = selectSort.abilityImg;
        cnacle = StartCoroutine(CoreManager.MatchTimeManager.SetCountDown(UpdateSuccess, selectSort.time_CD, null, myCDBar));
    }
    //升級成功
    void UpdateSuccess()
    {
        cnacle = null;
        selectSort.Overto_UnLock();
        UpdateScript.Update_ThisAbility(selectSort.abilityData, selectSort.myLevel, selectSort.unLockObj);
        ResetData();
    }
    //返回金錢
    private void ResetData()
    {
        selectSort = null;
        cancelMoney = 0;
        CoreManager.ReturnCanUse(this);
    }
}
