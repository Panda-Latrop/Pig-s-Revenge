using SimpleJSON;
using System;
using System.Collections;
using UnityEngine;

public class CameraActor : Actor
{
    [SerializeField]
    protected new Camera camera;
    [SerializeField]
    protected CameraMovementComponent movement;
    public Camera Instance => camera;
    public CameraMovementComponent Movement => movement;
    public float NearClipPlane { get => camera.nearClipPlane; }
    public Ray ScreenPointToRay(Vector3 pos, Camera.MonoOrStereoscopicEye eye)
    {
        return camera.ScreenPointToRay(pos, eye);
    }
    public virtual void Teleport(Vector3 point)
    {
        movement.Teleport(point);
    }
    public virtual void Lerp(Vector3 point)
    {
        movement.Lerp(point);
    }
    public virtual void Move(Vector3 direction)
    {
        movement.Move(direction);
    }
}
