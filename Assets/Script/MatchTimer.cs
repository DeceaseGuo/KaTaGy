using Photon;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MatchTimer : PunBehaviour
{
    #region 單例模式
    private static MatchTimer instance;
    public static MatchTimer Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(MatchTimer)) as MatchTimer;
                if (instance == null)
                {
                    GameObject go = new GameObject("MatchTimer");
                    instance = go.AddComponent<MatchTimer>();
                }
            }
            return instance;
        }
    }
    #endregion
    private EnemyManager enemyManager;
    private EnemyManager EnemyManagerScript { get { if (enemyManager == null) enemyManager = EnemyManager.instance; return enemyManager; } }

    public delegate void methods();

    private const string TimeToStartProp = "st";
    private double temp;    //進遊戲時間同步
    private double timeToStart = 0.0f;  //遊戲總時間

    private const string TimeToBornSoldier = "bornSoldier";
    private double timeToBornSoldier = 0.0f;    //生士兵倒數時間
    [Header("每一波怪物時間")]
    [SerializeField] double SecondsBeforeStart = 30.4f; //生士兵間隔時間
    ////////////////////////// 
    public bool isAutoBorn;

    private bool totalTimeShow = false;
    public bool TotalTimeShow { get { return totalTimeShow; } set { totalTimeShow = value; } }

    [Header("時間存放")]
    [SerializeField] Text allTimeText_min;
    [SerializeField] Text allTimeText_sec;
    [SerializeField] Text nextWaveText;
    double delta;
    int timeMin;
    int timeSec;

    ///     
    [System.Serializable]
    public class TmpFunction
    {
        public byte taskIndex;
        public bool reverseBar;
        public float needTime;
        public float arriveTime;
        public float nowTime;
        public methods doFunction;        
        public Text showText;
        public Image showBar;
        public bool needToShow;

        public TmpFunction(float _time,float _arriveTime, methods _function,byte _num)
        {
            taskIndex = _num;
            needTime = _time;
            arriveTime = _arriveTime;
            doFunction = _function;
            needToShow = false;
        }

        public TmpFunction(float _time, float _arriveTime ,methods _function, Text _text, Image _img, byte _num)
        {
            taskIndex = _num;
            needTime = _time;
            arriveTime = _arriveTime;
            doFunction = _function;
            showText = _text;
            showBar = _img;
            needToShow = true;
        }
    }
    private byte numberPlate;
    private int allTaskAmount;
    private int modifyIndex;
    private TmpFunction tmpFunction;
    public List<TmpFunction> myTasks = new List<TmpFunction>();
    ///

    public bool IsTimeToStop
    {
        get { return timeToBornSoldier > 0.001f; }
    }

    void Start()
    {
        if (Instance != this)
            Destroy(this);

        SetDataAllTime();

        if (isAutoBorn)
            NextWaveTime();
    }

    public void NeedToLateUpdate()
    {
        timeToStart = PhotonNetwork.time - temp;

        if (allTaskAmount != 0)
        {
            for (int i = 0; i < allTaskAmount; i++)
            {
                if (myTasks[i].arriveTime <= (float)timeToStart)
                {
                    if (myTasks[i].needToShow)
                    {
                        if (myTasks[i].showText != null)
                            myTasks[i].showText.text = "";
                        if (myTasks[i].showBar != null)
                            myTasks[i].showBar.fillAmount = 0;
                    }
                    myTasks[i].doFunction();
                    myTasks.Remove(myTasks[i]);
                    allTaskAmount = myTasks.Count;
                }
                else if (myTasks[i].needToShow)
                {
                    myTasks[i].nowTime = myTasks[i].arriveTime - (float)timeToStart;
                    if (myTasks[i].showText != null)
                        myTasks[i].showText.text = myTasks[i].nowTime.ToString("0");
                    if (myTasks[i].showBar != null)
                    {
                        if (!myTasks[i].reverseBar)
                            myTasks[i].showBar.fillAmount = myTasks[i].nowTime / myTasks[i].needTime;
                        else
                            myTasks[i].showBar.fillAmount = ((myTasks[i].nowTime / myTasks[i].needTime) - 1) * -1;
                    }
                }
            }
        }

        if (totalTimeShow)
            CorrectTimeText();

        if (IsTimeToStop)
            nextWaveText.text = SecondsToNextWave.ToString("0");
    }

    #region 計算總遊戲時間
    public void FirstOpen()
    {
        timeMin = (int)timeToStart / 60;
        TotalTimeShow = true;
    }

    void CorrectTimeText()
    {
        timeSec = (int)timeToStart - (timeMin * 60);
        if (timeSec == 60)
            timeMin++;

        allTimeText_min.text = timeMin.ToString("0");
        allTimeText_sec.text = timeSec.ToString("0");
    }
    #endregion

    #region 傳遞資訊
    void SetDataAllTime()
    {
        if (PhotonNetwork.isMasterClient)
        {
            if (timeToStart == 0.0f && PhotonNetwork.time > 0.0001f)
            {
                //紀錄當前時間
                temp = PhotonNetwork.time;
                ExitGames.Client.Photon.Hashtable timeProps = new ExitGames.Client.Photon.Hashtable() { { TimeToStartProp, this.temp } };
                PhotonNetwork.room.SetCustomProperties(timeProps);
            }
        }
    }

    public void NextWaveTime()
    {
        if (PhotonNetwork.isMasterClient)
        {
            if (!IsTimeToStop && PhotonNetwork.time > 0.0001f)
            {
                timeToBornSoldier = PhotonNetwork.time + SecondsBeforeStart;
                ExitGames.Client.Photon.Hashtable timeProps = new ExitGames.Client.Photon.Hashtable() { { TimeToBornSoldier, this.timeToBornSoldier } };
                PhotonNetwork.room.SetCustomProperties(timeProps);
            }
        }
    }
    #endregion

    #region 生兵倒數時間
    public double SecondsToNextWave
    {
        get
        {
            if (IsTimeToStop)
            {
                delta = timeToBornSoldier - PhotonNetwork.time;
                if (delta > 0.0f)
                    return delta;
                else
                {
                    timeToBornSoldier = 0.0f;
                    EnemyManagerScript.SpawnWave();
                    NextWaveTime();
                    return 0.0f;
                }
            }
            else
            {
                return 0.0f;
            }
        }
    }
    #endregion

    #region 真實時間倒數
    //[需要每秒時間]
    public byte SetCountDown(methods _function, float a, Text _text, Image _img)
    {
        //號碼牌從1開始
        if (numberPlate < 230)
            numberPlate++;
        else
            numberPlate = 0;
        tmpFunction = new TmpFunction(a, a + (float)timeToStart, _function, _text, _img, numberPlate);
        myTasks.Add(tmpFunction);
        allTaskAmount = myTasks.Count;
        return numberPlate;
    }

    //[不需要每秒時間] 可取消修改
    public byte SetCountDown(methods _function, float a)
    {
        if (numberPlate < 230)
            numberPlate++;
        else
            numberPlate = 0;
        tmpFunction = new TmpFunction(a, a + (float)timeToStart, _function, numberPlate);
        myTasks.Add(tmpFunction);
        allTaskAmount = myTasks.Count;
        return numberPlate;
    }
    //[需要每秒時間] 可取消修改 但bar條從0到1(Bar條反轉)
    public byte SetCountDownReveres(methods _function, float a, Image _img)
    {
        //號碼牌從1開始
        if (numberPlate < 230)
            numberPlate++;
        else
            numberPlate = 0;
        tmpFunction = new TmpFunction(a, a + (float)timeToStart, _function, null, _img, numberPlate);
        tmpFunction.reverseBar = true;
        myTasks.Add(tmpFunction);
        allTaskAmount = myTasks.Count;
        return numberPlate;
    }
    //[不需要每秒時間]不可取消修改
    public void SetCountDownNoCancel(methods _function, float a)
    {
        tmpFunction = new TmpFunction(a, a + (float)timeToStart, _function, 255);
        myTasks.Add(tmpFunction);
        allTaskAmount = myTasks.Count;
    }

    //修改時間
    public void ModifyTime(byte _index,float _time)
    {
        modifyIndex = myTasks.FindIndex(x => x.taskIndex == _index);
        if (modifyIndex != -1)
            myTasks[modifyIndex].arriveTime = _time + (float)timeToStart;
    }
    //取消這個任務
    public void ClearThisTask(byte _index)
    {
        myTasks.Remove(myTasks.Find(x => x.taskIndex == _index));
        allTaskAmount = myTasks.Count;
    }
    #endregion

    public override void OnPhotonCustomRoomPropertiesChanged(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(TimeToStartProp))
        {
            this.temp = (double)propertiesThatChanged[TimeToStartProp];
            Debug.Log("Got StartTime: " + this.temp);
        }

        if (propertiesThatChanged.ContainsKey(TimeToBornSoldier))
        {
            this.timeToBornSoldier = (double)propertiesThatChanged[TimeToBornSoldier];
            Debug.Log("enemyBornTime: " + this.timeToBornSoldier);
        }
    }
}