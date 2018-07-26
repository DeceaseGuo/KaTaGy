using Photon;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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
    [SerializeField] Text allTimeText;
    [SerializeField] Text nextWaveText;
    int timeMin;
    int timeSec;


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

    private void LateUpdate()
    {
        if (totalTimeShow)
            allTimeText.text = CorrectTimeText();

        if (IsTimeToStop)
            nextWaveText.text = SecondsToNextWave.ToString("0");
    }

    #region 計算總遊戲時間
    public void FirstOpen()
    {
        timeMin = (int)SecondsUntilItsTime / 60;
        TotalTimeShow = true;
    }

    string CorrectTimeText()
    {
        timeSec = (int)SecondsUntilItsTime - (timeMin * 60);
        if (timeSec == 60)
            timeMin++;
        
        return timeMin.ToString("0") + "分" + timeSec.ToString("0") + "秒";
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

    #region 取得時間
    //取得總時間
    public double SecondsUntilItsTime
    {
        get
        {
            timeToStart = PhotonNetwork.time - temp;
            return timeToStart;
        }
    }
    //取得生兵倒數時間
    public double SecondsToNextWave
    {
        get
        {
            if (this.IsTimeToStop)
            {
                double delta = timeToBornSoldier - PhotonNetwork.time;
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
    //需要每秒時間
    public IEnumerator SetCountDown(methods _function, float a, Text _text, Image _img)
    {
        float firstTime = (float)SecondsUntilItsTime + a;
        while (true)
        {
            float nowTime = firstTime - (float)SecondsUntilItsTime;
            if (nowTime <= 0)
            {
                if (_text != null)
                    _text.text = 0.ToString("0.0");
                if (_function != null)
                    _function();
                if (_img != null)
                    _img.fillAmount = 0;
                yield break;
            }
            else
            {
                if (_text != null)
                    _text.text = nowTime.ToString("0.0");
                if (_img != null)
                    _img.fillAmount = nowTime / a;
            }

            yield return null;
        }
    }
    //不需要每秒時間
    public IEnumerator SetCountDown(methods _function, float a)
    {
        float firstTime = (float)SecondsUntilItsTime + a;
        while (true)
        {
            float nowTime = firstTime - (float)SecondsUntilItsTime;
            if (nowTime <= 0)
            {
                if (_function != null)
                    _function();
                yield break;
            }
            yield return null;
        }
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