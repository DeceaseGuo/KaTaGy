using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MinMapSyn : MonoBehaviour, IPointerClickHandler
{
    [Header("大地圖")]
    [SerializeField] MeshFilter myTerrain;

    [Header("設定")]
    [SerializeField] float updateIconTime = 0.6f;
    [Tooltip("Icon偵測範圍")]
    [SerializeField] float IconRange = 30;

    [Header("IconPrefabs")]
    public RectTransform TowerIcon;
    public RectTransform EIcon;
    public RectTransform SoliderIcon;
    public RectTransform AllenIcon;
    public RectTransform QueenIcon;

    public RectTransform myplayerIcon;
    private RectTransform enemyplayerIcon;

    [Header("IconList")]
    public List<RectTransform> myTowerIcons = new List<RectTransform>();
    public List<RectTransform> mySoliderIcons = new List<RectTransform>();
    public List<RectTransform> enemyTowerIcons = new List<RectTransform>();
    public List<RectTransform> enemySoliderIcons = new List<RectTransform>();
    public List<RectTransform> ShowEnemyIcons = new List<RectTransform>();

    Player playerScript;
    
    //大地圖寬高
    float widthMax;
    float heightMax;

    RectTransform litMap;

    //寬高比例
    float widthRate;
    float heightRate;

    Vector3 tmpAngle;
    Vector2 tmpPos = Vector2.zero;

    private SceneObjManager sceneObjManager;
    private SceneObjManager SceneManager { get { if (sceneObjManager == null) sceneObjManager = SceneObjManager.Instance; return sceneObjManager; } }

    private void Start()
    {
        myplayerIcon = Instantiate(AllenIcon, transform);
        enemyplayerIcon = Instantiate(AllenIcon, transform);
        enemyplayerIcon.gameObject.SetActive(false);
        litMap = GetComponent<RectTransform>();
        playerScript = Creatplayer.instance.Player_Script;
        SceneManager.minmap = this;
        getWidthHeight();

        StartCoroutine("UpdateIconPos");
    }

    #region 獲得地圖長寬
    void getWidthHeight()
    {
        widthMax = myTerrain.mesh.bounds.size.x;
        heightMax = myTerrain.mesh.bounds.size.z;
    }
    #endregion

    #region 更新icon位置
    void UpdatePos(GameObject _obj, RectTransform _icon)
    {
        if (_obj == null || _icon == null)
        {
            return;
        }
        Transform trans = _obj.transform;
        //print("更新");
        //寬比例=玩家目前位置.x / 大地圖.x
        widthRate = (trans.position.x + (widthMax*0.5f)) / widthMax;
        //高比例=玩家目前位置.z / 大地圖.z
        heightRate = (trans.position.z + (heightMax*0.5f)) / heightMax;
        //玩家icon位置.x=小地圖.x *寬比例
        tmpPos.x = litMap.sizeDelta.x * (widthRate - 0.5f);
        //玩家icon位置.y=小地圖.y *寬比例
        tmpPos.y = litMap.sizeDelta.y * (heightRate - 0.5f);
        //print(tmpPos);
        tmpAngle = _icon.localEulerAngles;
        tmpAngle.z = 180 - trans.localEulerAngles.y;
        _icon.localEulerAngles = tmpAngle;
        _icon.localPosition = tmpPos;
    }

    IEnumerator UpdateIconPos()
    {
        while (true)
        {
            yield return new WaitForSeconds(updateIconTime);
            #region 我方
            UpdatePos(playerScript.gameObject, myplayerIcon);//更新玩家icon
            ShowEnemysIcon(myplayerIcon);//顯示視野內敵人
            //Tower
            for (int i = 0; i < myTowerIcons.Count; i++)
            {
                UpdatePos(SceneManager.myTowerObjs[i], myTowerIcons[i]);                                                                  
                ShowEnemysIcon(myTowerIcons[i]);//顯示視野內敵人
            }
            //Solider
            for (int i = 0; i < mySoliderIcons.Count; i++)
            {
                UpdatePos(SceneManager.mySoldierObjs[i], mySoliderIcons[i]);                                                                
                ShowEnemysIcon(mySoliderIcons[i]);//顯示視野內敵人
            }
            #endregion

            #region 敵方
            UpdatePos(SceneManager.enemy_Player, enemyplayerIcon);//更新玩家icon
            //Tower
            for (int i = 0; i < enemyTowerIcons.Count; i++)
            {
                UpdatePos(SceneManager.enemyTowerObjs[i], enemyTowerIcons[i]);
            }
            //Solider
            for (int i = 0; i < enemySoliderIcons.Count; i++)
            {
                UpdatePos(SceneManager.enemySoldierObjs[i], enemySoliderIcons[i]);
            }
            #endregion
        }
    }
    #endregion

    void dis(RectTransform my, RectTransform _r, List<RectTransform> curr)
    {
        if (_r == null)
        {
            return;
        }

        if (Vector3.Distance(_r.transform.position , my.transform.position) <= IconRange)
        {
            curr.Add(_r);
            if (!ShowEnemyIcons.Contains(_r))
            {
                ShowEnemyIcons.Add(_r);
                _r.gameObject.SetActive(true);
            }
            Debug.LogFormat("我方{0}視野，顯示敵人 : {1}", my.name, _r.name);
        }
    }
    
    #region 顯示視野內敵人icon
    void ShowEnemysIcon(RectTransform myeyes)
    {
        List<RectTransform> currentIcon = new List<RectTransform>();
        dis(myeyes, enemyplayerIcon, currentIcon);

        for (int i = 0; i < enemyTowerIcons.Count; i++)
        {
            dis(myeyes, enemyTowerIcons[i], currentIcon);
        }

        for (int i = 0; i < enemySoliderIcons.Count; i++)
        {
            dis(myeyes, enemySoliderIcons[i], currentIcon);
        }
        for (int i = 0; i < currentIcon.Count; i++)
        {
            print(currentIcon[i]);
        }

        ClearList(currentIcon);
    }
    #endregion

    #region 清除不再範圍內的Icon
    [SerializeField]List<RectTransform> lastList = new List<RectTransform>();
    void ClearList(List<RectTransform> curr)
    {
        for (int i = 0; i < lastList.Count; i++)
        {
            if (!curr.Contains(lastList[i]))
            {
                lastList[i].gameObject.SetActive(false);
                ShowEnemyIcons.Remove(lastList[i]);
                Debug.Log("移除敵人");
            }
        }
        lastList.Clear();
        for (int i = 0; i < curr.Count; i++)
        {
            lastList.Add(curr[i]);
        }
    }
    #endregion

    #region 點擊小地圖移動
    public void ClickMap()
    {
        Vector3 _pos = Vector3.zero;

        float mapX = Input.mousePosition.x - (litMap.position.x - (litMap.rect.width * 0.5f));
        float mapY = Input.mousePosition.y - (litMap.position.y - (litMap.rect.height * 0.5f));

        _pos.x = ((mapX / litMap.rect.width) - 0.5f) * widthMax;
        _pos.z = ((mapY / litMap.rect.height) - 0.5f) * heightMax;

        playerScript.getTatgetPoint(_pos);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if ((playerScript.MyState == Player.statesData.canMove_Atk || playerScript.MyState == Player.statesData.canMvoe_Build) && eventData.pointerId == -2)
        {
            Debug.Log("點擊");
            ClickMap();
        }
    }
    #endregion
}