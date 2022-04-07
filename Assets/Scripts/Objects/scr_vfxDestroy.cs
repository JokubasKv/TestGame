using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_vfxDestroy : MonoBehaviour
{
    public ParticleSystem sys;

    void Start()
    {
        if(sys==null)
            Destroy(this.gameObject, 1);
        else
        {
            Destroy(this.gameObject, sys.main.duration);
        }
    }
}
