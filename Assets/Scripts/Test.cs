using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Camera cam;
    public ProjectileActor projectile;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mp = Input.mousePosition;
            mp.z = 1;
            mp = cam.ScreenToWorldPoint(mp);
            mp.z = 0;
            var p = GameInstance.Instance.PoolManager.Pop(projectile);
            p.SetPosition(mp);
            
        }
    }
}
