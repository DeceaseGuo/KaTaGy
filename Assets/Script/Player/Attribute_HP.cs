using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Attribute_HP : Photon.MonoBehaviour
{
    private Player player;
    [Header("左上螢幕血量UI")]
    private Image leftTopHpBar;
    //角色頭上血量
    public Image UI_HpBar;
    private Animator ani;
    [Header("改變顏色")]
    private float maxValue;
    [SerializeField] Renderer myRender;

    private void Awake()
    {
        player = GetComponent<Player>();
        ani = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        GetComponent<PhotonTransformView>().enabled = true;
        UI_HpBar.fillAmount = 1;
        if (photonView.isMine)
        {
            if (leftTopHpBar == null)
                leftTopHpBar = GameObject.Find("hpBar_0022").GetComponent<Image>();

            leftTopHpBar.fillAmount = 1;
        }
        if (player != null)
            player.formatData();
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown("z"))
        {
            takeDamage(15f, Vector3.zero, true);
        }
    }

    #region 打中效果
    void BeHitChangeColor()
    {
        if (maxValue == 0)
        {
            maxValue = 10;
            myRender.material.SetColor("_EmissionColor", new Color(255, 0, 0, maxValue));
            myRender.material.EnableKeyword("_EMISSION");            
            StartCoroutine(OriginalColor());
        }
        else
        {
            maxValue = 10;
            myRender.material.SetColor("_EmissionColor", new Color(255, 0, 0, maxValue));
        }
    }

    IEnumerator OriginalColor()
    {
        while (maxValue > 0)
        {
            maxValue -= Time.deltaTime * 70;
            myRender.material.SetColor("_EmissionColor", new Color(255, 0, 0, maxValue));
            if (maxValue <= 0)
            {
                maxValue = 0;
                myRender.material.DisableKeyword("_EMISSION");                
                yield break;
            }
            yield return null;
        }
    }
    #endregion

    #region 受到傷害
    [PunRPC]
    public void takeDamage(float _damage, Vector3 _dir, bool ifHit)
    {
        if (player.deadManager.checkDead)
            return;

        float tureDamage = CalculatorDamage(_damage);

        if (player.playerData.Hp_original > 0)
        {
            player.playerData.Hp_original -= tureDamage;
            BeHitChangeColor();
            ani.SetBool(player.AniControll.aniHashValue[8], true);
            if (player.playerData.Hp_original <= 0)
            {
                player.deadManager.ifDead(true);
                ani.SetBool(player.AniControll.aniHashValue[15], true);
                player.Death();
            }
            openPopupObject(tureDamage);
            if (ifHit && !player.deadManager.checkDead && !player.deadManager.notFeedBack && !player.NowCC)
            {
                player.CancelNowSkill();
                ani.SetTrigger(player.AniControll.aniHashValue[14]);
                player.beHit(_dir);
            }
        }
    }
    #endregion

    void openPopupObject(float _damage)
    {
        FloatingTextController.instance.CreateFloatingText(_damage.ToString("0.0"), this.transform);
        UI_HpBar.fillAmount = player.playerData.Hp_original / player.playerData.Hp_Max;
        if (photonView.isMine)
            leftTopHpBar.fillAmount = player.playerData.Hp_original / player.playerData.Hp_Max;        
    }

    #region 計算傷害
    protected virtual float CalculatorDamage(float _damage)
    {
        return _damage;
    }
    #endregion
}
