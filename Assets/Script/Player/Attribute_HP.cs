using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Attribute_HP : Photon.MonoBehaviour
{
    private Player player;
    private GameObject displayHpBarPos;
    [Header("左上螢幕角色UI")]
    [SerializeField] Image leftTopHpBar;
    [SerializeField] Image leftTopPowerBar;
    [Header("角色頭上UI")]
    public GameObject UI_HpObj;
    public Image UI_HpBar;

    private void Start()
    {
        player = GetComponent<Player>();
        displayHpBarPos = GameObject.Find("Display_HpBarPos");
        UI_HpObj.transform.SetParent(displayHpBarPos.transform, false);
        if (photonView.isMine)
        {
            leftTopHpBar = GameObject.Find("hpBar_0022").GetComponent<Image>();
        }
    }

    private void FixedUpdate()
    {
        displayHpBar();
    }

    #region 顯示血條
    void displayHpBar()
    {
        Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 7.5f, 0));
        UI_HpObj.transform.position = screenPos;
    }
    #endregion

    #region 受到傷害
    [PunRPC]
    public void takeDamage(float _damage)
    {
        if (player.deadManager.checkDead)
            return;

        float tureDamage = CalculatorDamage(_damage);
        if (photonView.isMine)
        {
            if (player.playerData.Hp_original > 0)
            {
                player.playerData.Hp_original -= tureDamage;
            }
            if (player.playerData.Hp_original <= 0)
            {
                GetComponent<PhotonView>().RPC("NowDeath", PhotonTargets.All);
            }
        }
        openPopupObject(tureDamage);
    }
    #endregion

    void openPopupObject(float _damage)
    {
        FloatingTextController.instance.CreateFloatingText(_damage.ToString("0.0"), this.transform);
        if (!photonView.isMine)
            return;
        float _value = player.playerData.Hp_original / player.playerData.Hp_Max;
        GetComponent<PhotonView>().RPC("show_HPBar", PhotonTargets.All, _value);
        leftTopHpBar.fillAmount = _value;
    }

    #region 計算傷害
    protected virtual float CalculatorDamage(float _damage)
    {
        return _damage;
    }
    #endregion

    //同步血量
    [PunRPC]
    public void show_HPBar(float _value)
    {
        UI_HpBar.fillAmount = _value;       
    }

    //同步死亡
    [PunRPC]
    public void NowDeath()
    {
        StartCoroutine(Death());
    }

    IEnumerator Death()
    {
        yield return new WaitForSeconds(1.5f);
    }
}
