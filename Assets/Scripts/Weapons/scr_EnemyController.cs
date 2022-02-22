using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scr_EnemyController : MonoBehaviour
{
    [Header("References")]
    public Text damageText;
    [Header("Enemy Settings")]
    public float health=0;
    
    public void TakeDamage(float damage)
    {
        health -= damage;

        damageText.text = health.ToString();

        /*if (health < 0)
        {
            Debug.Log("Dead");
        }*/
    }
}
