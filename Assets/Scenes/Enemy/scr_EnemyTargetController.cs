using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scr_EnemyTargetController : scr_EnemyBase
{
    [Header("References")]
    public Text damageText;

    public void Start()
    {
        damageText.text = health.ToString();
    }
    public override void TakeDamage(float damage)
    {
        Debug.Log("Target Damaged");
        health += damage;

        damageText.text = health.ToString();
    }
}
