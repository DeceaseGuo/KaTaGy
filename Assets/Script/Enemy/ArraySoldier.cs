using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArraySoldier : MonoBehaviour
{
    private int maxPopulation = 6;
    public int MaxPopulation
    {
        get { return maxPopulation; }

        private set { maxPopulation = value; }
    }
    private int nowPopulation = 0;
    public int rewardPopulation = 0;
    [SerializeField] Text populationText;

    private Sort_nextBornBtn sort_born;
    public List<Sort_nextBornBtn> sort_list;

    private Sort_Soldier sort_soldier;
    public Dictionary<GameManager.whichObject, Sort_Soldier> storeData = new Dictionary<GameManager.whichObject, Sort_Soldier>();
    public List<Sort_Soldier> soldier_list;

    private int nowArray = 0;

    private void Start()
    { 
        SetData();
        MenuOpen();
    }

    #region 數據管理
    //初始化
    void SetData()
    {
        for (int i = 0; i < sort_list.Count; i++)
        {
            sort_list[i].order = i;
        }

        for (int i = 0; i < soldier_list.Count; i++)
        {
            if (soldier_list[i].DataName == GameManager.whichObject.None)
                continue;
            else
            {
                soldier_list[i].ResetSoldierData();
                storeData.Add(soldier_list[i].DataName, soldier_list[i]);
            }
        }
    }
    //取得數據
    public Sort_Soldier getSortPos(GameManager.whichObject _name)
    {
        if (!storeData.ContainsKey(_name))
        {
            //無數據
            return null;
        }

        return storeData[_name];
    }
    //更新數據
    public void ResetSoldierData()
    {        
        //按鈕區
        for (int i = 0; i < soldier_list.Count; i++)
        {
            soldier_list[i].ResetSoldierData();
        }
    }
    #endregion

    #region 開關倉庫變化
    public void MenuOpen()
    {
        AIFull();
        nowArray = 0;
        GoNext_SortBorn(sort_list[0]);
    }

    public void MenuClose()
    {
        RenewArray();
        nowArray = 0;
        sort_born.OpenSelectImage(false);
        sort_born = null;
        sort_soldier = null;

        AIFull();
    }
    #endregion

    #region 清除一切
    public void click_clearAll()
    {
        for (int i = 0; i < sort_list.Count; i++)
        {
            sort_list[i].removeSoldier();
        }
        sort_soldier = null;
        sort_born.OpenSelectImage(false);
        nowPopulation = 0;
        populationText.text = nowPopulation.ToString() + "/" + MaxPopulation.ToString();
        nowArray = 0;
        GoNext_SortBorn(sort_list[0]);
    }
    #endregion

    #region 排序功能
    //自動排順序
    void RenewArray()
    {
        for (int i = 0; i < MaxPopulation; i++)
        {
            if (sort_list[i].isChose)
                continue;
            else
            {
                for (int a = i + 1; a < MaxPopulation; a++)
                {
                    if (!sort_list[a].isChose)
                        continue;
                    else
                    {
                        if (!sort_list[i].isChose)
                        {
                            sort_list[i].changeSoldier(sort_list[a].Data);
                            sort_list[a].removeSoldier();
                        }
                    }
                }
            }
        }        
    }
    //開啟關閉自動填滿
     public void AIFull()
    {
        if (MaxPopulation == nowPopulation)
            return;
        else
        {
            int needFull = MaxPopulation - nowPopulation;
            for (int i = 0; i < MaxPopulation; i++)
            {
                if (needFull == 0)
                    break;

                if (sort_list[i].isChose)
                    continue;
                else
                {
                    sort_list[i].changeSoldier(soldier_list[0]);
                    needFull--;
                }
            }
            nowPopulation = MaxPopulation;
            populationText.text = nowPopulation.ToString() + "/" + MaxPopulation.ToString();
        }
    }
    #endregion

    ///////////還需更改////////////////////
    #region 上鎖與解鎖
    //解鎖 → 最大人口+獎勵人口
    void removeLock(int _max)
    {
        for (int i = 7; i < _max; i++)
        {
            sort_list[i].LockState(false);
        }

        MaxPopulation = _max;
        populationText.text = nowPopulation.ToString() + "/" + MaxPopulation.ToString();
    }
    //上鎖 → 上限值-最大人口
    void LockOn(int _min)
    {
        for (int i = sort_list.Count - 1; i >= _min; i--)
        {
            sort_list[i].LockState(true);
            if (sort_list[i].isChose)
            {
                nowPopulation -= sort_list[i].nowPopulation;
                sort_list[i].removeSoldier();
            }
        }
        populationText.text = nowPopulation.ToString() + "/" + MaxPopulation.ToString();
    }
    #endregion

    //按下排列槽時
    public void click_sortBorn(Sort_nextBornBtn _sort)
    {
        if (_sort.order > MaxPopulation - 1)
        {
            Debug.Log("未解鎖");
            return;
        }

        //刪除原本的
        sort_born.OpenSelectImage(false);
        nowPopulation -= _sort.nowPopulation;
        populationText.text = nowPopulation.ToString() + "/" + MaxPopulation.ToString();
        _sort.removeSoldier();
        //選擇此位置
        sort_born = _sort;
        sort_born.OpenSelectImage(true);
        nowArray = sort_born.order;
    }

    //自動前往下一個
    private void GoNext_SortBorn(Sort_nextBornBtn _sort)
    {
        sort_born = _sort;
        sort_born.OpenSelectImage(true);
        nowArray = sort_born.order;
    }

    //按士兵鈕 → 進行交換
    public void click_Soldier(Sort_Soldier _sort)
    {
        if (sort_born == null)
            return;

        if (MaxPopulation - nowPopulation >= _sort.SoldierData.population_need)
        {
            GoChange(_sort);
        }
        else
        {
            if (sort_born.nowPopulation >= _sort.SoldierData.population_need)           
                GoChange(_sort);
            else
            {
                if (nowArray != MaxPopulation)
                {
                    if (CheckAllSpace(_sort.SoldierData.population_need))
                    {
                        sort_soldier = _sort;
                        AIRemoveSort();
                    }
                    else
                        Debug.Log("人口過多");
                }
                else
                {
                    Debug.Log("已到達最後一個,人口已滿");
                }
            }
        }
    }

    //放士兵上去
    void GoChange(Sort_Soldier _sort)
    {
        nowPopulation += _sort.SoldierData.population_need;

        //那位置有人
        if (sort_born.isChose)
        {
            nowPopulation -= sort_born.nowPopulation;
            sort_born.Data.ChangeAllAmount(1);
        }

        populationText.text = nowPopulation.ToString() + "/" + MaxPopulation.ToString();

        sort_born.changeSoldier(_sort);
        if (nowArray == MaxPopulation - 1)
            nowArray = 0;
        else
            nowArray += 1;

        GoNext_SortBorn(sort_list[nowArray]);
    }

    //自動移除士兵
    void AIRemoveSort()
    {
        for (int i = MaxPopulation - 1; i >= nowArray; i--)
        {
            if (!sort_list[i].isChose)
                continue;
            else
            {
                nowPopulation -= sort_list[i].nowPopulation;
                populationText.text = nowPopulation.ToString() + "/" + MaxPopulation.ToString();
                sort_list[i].removeSoldier();
                CheckHaveSpace();
                return;
            }
        }
        Debug.Log("已達最大人口");
    }

    void CheckHaveSpace()
    {
        if (MaxPopulation - nowPopulation >= sort_soldier.SoldierData.population_need)
        {
            GoChange(sort_soldier);
            sort_soldier = null;
        }
        else
            AIRemoveSort();
    }

    bool CheckAllSpace(int _needSpace)
    {
        int space = 0;
        for (int i = MaxPopulation - 1; i >= nowArray; i--)
        {
            if (!sort_list[i].isChose)
                continue;
            else
                space += sort_list[i].nowPopulation;
        }

        return ((MaxPopulation - nowPopulation) + space >= _needSpace) ? true : false;
    }
}
