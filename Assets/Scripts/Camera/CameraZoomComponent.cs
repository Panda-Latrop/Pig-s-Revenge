using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoomComponent : ActorComponent
{
    protected new Camera camera;
    protected float targetZoom = 13.0f;
    protected float defaultZoom = 13.0f;


    public void SetCamera(Camera camera)
    {
        this.camera = camera;
        defaultZoom = camera.orthographicSize;
    }
    public void To(float zoom)
    {
        enabled = true;
        targetZoom = zoom;
    }
    public void Default()
    {
        To(defaultZoom);
    }
    protected override void OnLateUpdate()
    {
        camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, targetZoom, 0.1f);
        if (Mathf.Abs(camera.orthographicSize - targetZoom) <= 0.01f)
        {
            enabled = false;
            camera.orthographicSize = targetZoom;
        }
    }
 
    protected void LateUpdate()
    {
        OnLateUpdate();
    }
}
