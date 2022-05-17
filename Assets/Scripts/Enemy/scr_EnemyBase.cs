using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_EnemyBase : MonoBehaviour
{
    [Header("Enemy Settings")]
    public float health = 0;

    public virtual void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0) Destroy(gameObject);
    }
}
