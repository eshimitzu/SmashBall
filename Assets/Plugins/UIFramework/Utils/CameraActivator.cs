using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CameraActivator
{
    private readonly Camera _uiCamera;
    private UniversalAdditionalCameraData _uiCameraData;

    List<Camera> cameras3D = new();

    public CameraActivator(Camera uiCamera)
    {
        _uiCamera = uiCamera;
        _uiCameraData = _uiCamera.GetComponent<UniversalAdditionalCameraData>();
        _uiCameraData.renderType = CameraRenderType.Base;
    }

    public void ActivateGameCamera(Camera gameCamera)
    {
        cameras3D.Add(gameCamera);
        _uiCameraData.renderType = CameraRenderType.Overlay;
        var camData = gameCamera.GetComponent<UniversalAdditionalCameraData>();
        camData.renderType = CameraRenderType.Base;
        camData.cameraStack.Add(_uiCamera);
    }

    public void DeactivateGameCamera(Camera gameCamera)
    {
        cameras3D.Remove(gameCamera);

        var camData = gameCamera.GetComponent<UniversalAdditionalCameraData>();
        if (camData.cameraStack.Contains(_uiCamera))
        {
            camData.cameraStack.Remove(_uiCamera);
            if (cameras3D.Count > 0)
            {
                cameras3D[0].GetComponent<UniversalAdditionalCameraData>().cameraStack.Add(_uiCamera);
            }
        }
        
        if (cameras3D.Count == 0)
        {
            _uiCameraData.renderType = CameraRenderType.Base;
        }
    }
}