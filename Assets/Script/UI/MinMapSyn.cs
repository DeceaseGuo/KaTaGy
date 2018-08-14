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
    [SerializeField] float updateIconTime = 0.2f;
    [Tooltip("Icon偵測範圍")]
    [SerializeField] float IconRange;

    [Header("IconPrefabs")]
    public RectTransform TowerIcon;
    public RectTransform EIcon;
    public RectTransform SoliderIcon;
    public RectTransform PlayerIcon;
    public RectTransform myplayerIcon;
    public RectTransform enemyplayerIcon;

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
        litMap = GetComponent<RectTransform>();
        playerScript = Creatplayer.instance.Player_Script;
        myplayerIcon = Instantiate(PlayerIcon, transform);
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
    void UpdatePos(Transform _objTrans, RectTransform _icon)
    {
        //print("更新");
        //寬比例=玩家目前位置.x / 大地圖.x
        widthRate = (_objTrans.position.x + (widthMax*0.5f)) / widthMax;
        //高比例=玩家目前位置.z / 大地圖.z
        heightRate = (_objTrans.position.z + (heightMax*0.5f)) / heightMax;
        //玩家icon位置.x=小地圖.x *寬比例
        tmpPos.x = litMap.sizeDelta.x * (widthRate - 0.5f);
        //玩家icon位置.y=小地圖.y *寬比例
        tmpPos.y = litMap.sizeDelta.y * (heightRate - 0.5f);
        //print(tmpPos);
        tmpAngle = _icon.localEulerAngles;
        tmpAngle.z = 180 - _objTrans.localEulerAngles.y;
        _icon.localEulerAngles = tmpAngle;
        _icon.localPosition = tmpPos;
    }

    List<GameObject> targets = new List<GameObject>();
    IEnumerator UpdateIconPos()
    {
        while (true)
        {
            yield return new WaitForSeconds(updateIconTime);

            UpdatePos(playerScript.transform, myplayerIcon);//更新玩家icon
            ShowEnemysIcon(playerScript.gameObject);//顯示視野內敵人
            //Tower
            for (int i = 0; i < myTowerIcons.Count; i++)
            {
                UpdatePos(SceneManager.myTowerObjs[i].transform, myTowerIcons[i]);                                                                  
                //ShowEnemysIcon(myObj[i]);//顯示視野內敵人
            }
            //Solider
            for (int i = 0; i < mySoliderIcons.Count; i++)
            {
                UpdatePos(SceneManager.mySoldierObjs[i].transform, mySoliderIcons[i]);                                                                
                //ShowEnemysIcon(myObj[i]);//顯示視野內敵人
            }
        }
    }
    #endregion

    #region 顯示視角內敵人icon
    void ShowEnemysIcon(GameObject myeyes)
    {
        targets = SceneManager.CalculationDis(myeyes, IconRange, true, true);
        if (targets.Count <= 0)
        {
            return;
        }
        for (int i = 0; i < targets.Count; i++)
        {
            isDead targetDead = targets[i].GetComponent<isDead>();
            RectTransform _target = null;
            switch (targetDead.myAttributes)
            {
                case GameManager.NowTarget.Player:
                    {
                        _target = enemyplayerIcon;
                    }
                    break;
                case GameManager.NowTarget.Soldier:
                    {
                        _target = enemySoliderIcons[SceneManager.enemySoldierObjs.IndexOf(targets[i])];
                    }
                    break;
                case GameManager.NowTarget.Tower:
                    {
                        _target = enemyTowerIcons[SceneManager.enemySoldierObjs.IndexOf(targets[i])];
                    }
                    break;
                default:
                    break;
            }
            if (_target != null)
            {
                CheckEnemyIconExist(_target);
                Debug.LogFormat("我方視野{0}，顯示敵人{1} : {2}", myeyes.name, i, targets[i].name);
            }
        }
    }
    #endregion

    #region
    void CheckEnemyIconExist(RectTransform _enemy)
    {
        if (!ShowEnemyIcons.Contains(_enemy))
        {
            ShowEnemyIcons.Add(_enemy);
            _enemy.gameObject.SetActive(true);
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