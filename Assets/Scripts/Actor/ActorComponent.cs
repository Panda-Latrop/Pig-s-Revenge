using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorComponent : MonoBehaviour, ISaveableComponent
{
    protected virtual void OnAwake()
    {

    }
    protected virtual void OnStart()
    {

    }
    protected virtual void OnUpdate()
    {

    }
    protected virtual void OnFixedUpdate()
    {

    }
    protected virtual void OnLateUpdate()
    {

    }
    protected virtual void OnDestroyLate()
    {

    }
    public JSONObject Load(JSONObject jsonObject)
    {
        return jsonObject;
    }
    public JSONObject Save(JSONObject jsonObject)
    {
        return jsonObject;
    }
}
