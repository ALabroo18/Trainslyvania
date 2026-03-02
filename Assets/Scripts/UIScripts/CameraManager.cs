using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera currentCam;

    public Camera mainCamera;
    public Camera cam1;
    public Camera cam2;
    public Camera cam3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = Camera.main;
        // currentCam.enabled = true;
        cam1.enabled = false;
        cam2.enabled = false;
        cam3.enabled = false;
        
    }
    public void SetCam1()
    {
        currentCam = cam1;
        cam1.enabled = true;
        if(cam2.enabled)
        {
            cam2.enabled = false;
        }
        else if (cam3.enabled)
        {
            cam3.enabled = false;
        }
        else if(mainCamera.enabled)
        {
            mainCamera.enabled = false;
        }
    }

    public void SetCam2()
    {
        currentCam = cam2;
        cam2.enabled = true;
        if(cam1.enabled)
        {
            cam1.enabled = false;
        }
        else if (cam3.enabled)
        {
            cam3.enabled = false;
        }
        else if(mainCamera.enabled)
        {
            mainCamera.enabled = false;
        }
    }
    public void SetCam3()
    {
        currentCam = cam3;
        cam3.enabled = true;
        if(cam1.enabled)
        {
            cam1.enabled = false;
        }
        else if (cam2.enabled)
        {
            cam2.enabled = false;
        }
        else if(mainCamera.enabled)
        {
            mainCamera.enabled = false;
        }
    }

    public void SetMainCam()
    {
        currentCam = mainCamera;

        mainCamera.enabled = true;

        if(cam1.enabled)
        {
            cam1.enabled = false;
        }
        else if (cam2.enabled)
        {
            cam2.enabled = false;
        }
        else if (cam3.enabled)
        {
            cam3.enabled = false;
        }
    }
}
