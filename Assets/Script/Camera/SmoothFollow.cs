using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using DG.Tweening;

public class SmoothFollow : MonoBehaviour
{
    public static SmoothFollow instance;
    private MyCore coreManager;

    [Header("數據")]
    private Transform target;
    [SerializeField] Vector3 offsetPos;
    [SerializeField] Vector3 offsetRot;
    [SerializeField] float smoothSpeed;


    private Player playerScript;

    [Header("zoom")]
    [SerializeField]
    float minZoomZ = -35f;
    [SerializeField] float maxZoomZ = -9f;
    [SerializeField] float minZoomY = 15f;
    [SerializeField] float maxZoomY = 60f;
    [SerializeField] float scrollSpeed = 20f;

    [Header("UAV")]
    [SerializeField]
    bool openControl;
    private Camera UAV;
    private Camera followControll;
    [SerializeField] GameObject UAV_model;
    [SerializeField] float panSpeed = 20f;
    // [SerializeField] float panBorder = 3f;
    [SerializeField] float UAV_minZoomY = 6f;
    [SerializeField] float UAV_maxZoomY = 250f;
    private Vector3 UAV_originalPos;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        target = Creatplayer.instance.MyNowPlayer.transform;
        playerScript = Creatplayer.instance.Player_Script;
        coreManager = MyCore.instance;
        followControll = GetComponent<Camera>();
        UAV = UAV_model.GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        camera_Move();

        if (!coreManager.CoreOpen)
            camera_Zoom(offsetPos);

        if (openControl)
            camera_Control();
    }

   /* #region 目前為那個角色
    public void checkCurrentPlay()
    {
        switch (GameManager.instance.Meis)
        {
            case GameManager.meIs.Allen:
                target = GameObject.Find("Player_Allen(Clone)").transform;
                return;
            case GameManager.meIs.Queen:
                target = GameObject.Find("Player_Queen(Clone)").transform;
                return;
            default:
                return;
        }
    }
    #endregion*/

    #region UAV開關
    public void switch_UAV()
    {
        //變更隨
        if (openControl)
        {
            offsetPos.y = 55f;
            UAV_model.SetActive(false);
            openControl = false;
            UAV.enabled = false;
            playerScript.stopAnything_Switch(false);
            followControll.enabled = true;
        }
        //變不更隨
        else
        {
            offsetPos.y = 40;
            UAV_model.SetActive(true);
            UAV.transform.position = UAV_originalPos;
            openControl = true;
            followControll.enabled = false;
            playerScript.stopAnything_Switch(true);
            UAV.enabled = true;
        }
    }
    #endregion

    #region camera自動跟隨
    private void camera_Move()
    {
        if (!openControl)
        {
            Vector3 desiredPosition = target.position + offsetPos;

            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

            transform.position = smoothedPosition;
            UAV_originalPos = target.position;
            transform.rotation = Quaternion.Euler(offsetRot);
        }
    }
    #endregion

    private float tmpRot = 43;
    #region 滾輪縮放
    void camera_Zoom(Vector3 nowPos)
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (!openControl)
        {
            //斜率-9.5y=8z
            float _Rot = scroll * scrollSpeed * 35 * Time.deltaTime;
            Vector3 tmpPos = (target.position - transform.position).normalized * scroll * scrollSpeed * 100f * Time.deltaTime;
            tmpPos.x = 0;
            nowPos += tmpPos;
            tmpRot -= _Rot;
            nowPos.z = Mathf.Clamp(nowPos.z, minZoomZ, maxZoomZ);
            nowPos.y = Mathf.Clamp(nowPos.y, minZoomY, maxZoomY);
            tmpRot = Mathf.Clamp(tmpRot, 37, 43);

        }
        else
        {
            nowPos.y -= scroll * scrollSpeed * 100f * Time.deltaTime;
            nowPos.y = Mathf.Clamp(nowPos.y, UAV_minZoomY, UAV_maxZoomY);
        }

        offsetPos = nowPos;
        offsetRot = Vector3.Lerp(offsetRot, new Vector3(tmpRot, 0, 0), .4f);
    }
    #endregion

    #region UAV模式
    void camera_Control()
    {
        Vector3 newPos = UAV.transform.position;

        if (Input.GetKey("w") /*|| Input.mousePosition.y >= Screen.height - panBorder*/)
        {
            newPos.z += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("s") /*|| Input.mousePosition.y <= panBorder*/)
        {
            newPos.z -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("d") /*|| Input.mousePosition.x >= Screen.width - panBorder*/)
        {
            newPos.x += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("a") /*|| Input.mousePosition.x <= panBorder*/)
        {
            newPos.x -= panSpeed * Time.deltaTime;
        }

        newPos = Vector3.Lerp(newPos, new Vector3(newPos.x, offsetPos.y, newPos.z), smoothSpeed);
        UAV.transform.position = newPos;
    }
    #endregion

    // public bool isShake = false;
    #region 攝影機晃動
    public IEnumerator CameraShake(float _duration, float _power)
    {
        //Vector3 originalPos = transform.localPosition;

        float nowTime = 0.0f;

        while (nowTime < _duration)
        {
            float x = Random.Range(-1f, 1f) * _power;
            float y = Random.Range(-1f, 1f) * _power;
            transform.localPosition += new Vector3(x, y, 0);
            nowTime += Time.deltaTime;
            yield return null;
        }
        //transform.localPosition = originalPos;
    }
    #endregion
}