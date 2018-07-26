using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CameraEffect : MonoBehaviour
{
    public static CameraEffect instance;
    [SerializeField] PostProcessVolume mainCameraCon;
    [SerializeField] PostProcessLayer mainCameraLayer;

    [SerializeField] PostProcessProfile scene_Die;


    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void nowDie(bool _t)
    {
        if (_t)
        {
            mainCameraCon.profile = scene_Die;
            mainCameraCon.enabled = _t;
            mainCameraLayer.enabled = _t;
        }
        else
        {
            mainCameraCon.profile = null;
            mainCameraCon.enabled = _t;
            mainCameraLayer.enabled = _t;
        }
    }
}
