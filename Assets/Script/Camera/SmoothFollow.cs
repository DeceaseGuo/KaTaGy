using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
    public static SmoothFollow instance;
    private MyCore coreManager;
    public GameObject colliderCancel;

    private Transform myCachedTransform;

    [Header("基本數據")]
    [SerializeField] Vector3 originalPos;
    private Vector3 offsetPos;
    [SerializeField] Vector3 offsetRot;
    [SerializeField] float smoothSpeed = 2.2f;
    private Transform target;
    public Material[] occMaterials;

    [Header("zoom需要")]
    [SerializeField] float minZoomZ = -48f;
    [SerializeField] float maxZoomZ = -9f;
    [SerializeField] float minZoomY = 5f;
    [SerializeField] float maxZoomY = 41f;
    [SerializeField] float scrollSpeed = 100f;
    private float scroll;
    private Vector3 tmpScrollPos;
    //private Camera followControll;

    [Header("不鎖視角需要")]
    public bool isLockCamera = true;
    [SerializeField] float panSpeed = 160f;
    [SerializeField] float panBorder = 15f;
    private Vector3 nOLockPos;
    [SerializeField] float maxZ_Border = /*96f;*/120;
    [SerializeField] float minZ_Border =/* -185f;*/-220;
    [SerializeField] float maxX_Border = /*171f;*/320;
    [SerializeField] float minX_Border = /*-181f;*/-305;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        myCachedTransform = this.transform;
    }

    private void Start()
    {
        myCachedTransform.rotation = Quaternion.Euler(offsetRot);
        target = Creatplayer.instance.Player_Script.transform;
       // playerScript = Creatplayer.instance.Player_Script;
        coreManager = MyCore.instance;
       // followControll = GetComponent<Camera>();
        GoBackMyPos();
    }


    public void NeedToLateUpdate()
    {
        ColliderCancel();

        if (!isLockCamera)
            camera_NOLock();
        else
            camera_Move();

        if (!coreManager.CoreOpen)
        {
            scroll = Input.GetAxis("Mouse ScrollWheel");
            camera_Zoom();
        }

        if (Input.GetKeyDown("c"))
        {
            myCachedTransform.rotation = Quaternion.Euler(offsetRot);
            offsetPos = originalPos;
        }
    }

    void ColliderCancel()
    {
        for (int i = 0; i < occMaterials.Length; i++)
        {
            occMaterials[i].SetVector("_PlayerPos", target.position);
        }

        if (target.localPosition.y < 10)
        {
            if (target.localPosition.x > -245 && target.localPosition.x < 280 && target.localPosition.z > -100 && target.localPosition.z < 100)
                colliderCancel.layer = 2;
            else
                colliderCancel.layer = 9;
        }
    }

    #region 鎖視角開關
    public void switch_UAV()
    {
        //不鎖
        if (isLockCamera)
        {
            isLockCamera = false;
            GoBackMyPos();
        }
        //鎖住
        else
        {
            isLockCamera = true;
            GoBackMyPos();
        }
    }
    #endregion

    #region 回到角色目前位置
    public void GoBackMyPos()
    {
        offsetPos = originalPos;

        if (isLockCamera)
            myCachedTransform.position = (target.position + target.up * 5f) + offsetPos;
        else
            nOLockPos = (target.position + target.up * 5f) + offsetPos;
    }
    #endregion

    #region camera自動跟隨
    private void camera_Move()
    {
        myCachedTransform.position = Vector3.Lerp(myCachedTransform.position, ((target.position + target.up * 5f) + offsetPos), smoothSpeed * Time.deltaTime);
    }
    #endregion

    #region 滾輪縮放
    void camera_Zoom()
    {
        if (scroll != 0 )
        {
            tmpScrollPos = myCachedTransform.forward * scroll * scrollSpeed * 20 * Time.deltaTime;
            if (isLockCamera)
            {
                offsetPos += tmpScrollPos;
                offsetPos.z = Mathf.Clamp(offsetPos.z, minZoomZ, maxZoomZ);
                offsetPos.y = Mathf.Clamp(offsetPos.y, minZoomY, maxZoomY);
            }
            else
            {
                if (nOLockPos.y > maxZoomY -1 || nOLockPos.y < 10)
                {
                    nOLockPos += new Vector3(0, tmpScrollPos.y, 0);
                    nOLockPos.y = Mathf.Clamp(nOLockPos.y, 9.9f, maxZoomY+4.9f);
                    return;
                }

                nOLockPos += tmpScrollPos;
                nOLockPos.y = Mathf.Clamp(nOLockPos.y, minZoomY, maxZoomY);
            }
        }
    }
    #endregion

    #region 不鎖視角
    void camera_NOLock()
    {
        if (Input.mousePosition.y >= Screen.height - panBorder)
        {
            if (nOLockPos.z < maxZ_Border)
                nOLockPos.z += panSpeed * Time.deltaTime;
            else
                nOLockPos.z = maxZ_Border;
        }
        if (Input.mousePosition.y <= panBorder)
        {
            if (nOLockPos.z > minZ_Border)
                nOLockPos.z -= panSpeed * Time.deltaTime;
            else
                nOLockPos.z = minZ_Border;
        }
        if (Input.mousePosition.x >= Screen.width - panBorder)
        {
            if (nOLockPos.x < maxX_Border)
                nOLockPos.x += panSpeed * Time.deltaTime;
            else
                nOLockPos.x = maxX_Border;
        }
        if (Input.mousePosition.x <= panBorder)
        {
            if (nOLockPos.x > minX_Border)
                nOLockPos.x -= panSpeed * Time.deltaTime;
            else
                nOLockPos.x = minX_Border;
        }

        myCachedTransform.position = Vector3.Lerp(myCachedTransform.position, nOLockPos, smoothSpeed);
    }
    #endregion

    #region 攝影機晃動
    public IEnumerator CameraShake(float _duration, float _power)
    {
        //Vector3 originalPos = transform.localPosition;

        float nowTime = 0.0f;

        while (nowTime < _duration)
        {
            float x = Random.Range(-1f, 1f) * _power;
            float y = Random.Range(-1f, 1f) * _power;
            myCachedTransform.localPosition += new Vector3(x, y, 0);
            nowTime += Time.deltaTime;
            yield return null;
        }
        //transform.localPosition = originalPos;
    }
    #endregion
}