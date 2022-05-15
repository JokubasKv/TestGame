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

    public AudioSource playSound;

    private void Start()
    {
        float settings = PlayerPrefs.GetFloat("Audio");
        playSound.volume = settings;
    }

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
                if(!characterController.gameOver)
                {
                    playSound.Play();
                    characterController.TakeDamage(damage, DamageType.Electric);
                    damageTicks = damageTimer;
                }
            }
        }
    }
}
