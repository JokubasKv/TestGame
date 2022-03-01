using System;
using System.Collections.Generic;
using UnityEngine;
using static scr_Models;

public class scr_ElectricCrystal : MonoBehaviour
{
    public int damage = 1;
    public float damageTimer = 1;
    float damageTicks = 0;
    public DamageType damageType;

    private void Update()
    {
        if (damageTicks > 0)
        {
            damageTicks -= Time.deltaTime;
        }
    }

    private void OnTriggerStay(Collider c)
    {
        if (damageTicks <= 0)
        {
            if (c.gameObject.tag == "Player")
            {
                scr_CharacterController characterController = c.GetComponent<scr_CharacterController>();
                characterController.TakeDamage(damage, DamageType.Electric);
                damageTicks = damageTimer;
            }
        }
    }
}
