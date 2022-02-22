using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_RaycastWeapon : MonoBehaviour
{
    [Header("References")]
    public Camera fpsCam;

    [Header("Shooting")]
    public float damage = 10f;
    public float range = 100f;


    public bool isShooting;

    #region - Shooting -
    public void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            scr_EnemyController target = hit.transform.GetComponent<scr_EnemyController>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }
        }

    }
    #endregion
}
