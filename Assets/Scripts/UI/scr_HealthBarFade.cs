using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scr_HealthBarFade : MonoBehaviour
{
    private const float DAMAGED_HEALTH_FADE_TIMER_MAX = 1f;
    private Image barImage;
    private Image damageBarImage;
    private Color damageColor;
    public float damagedHealthFadeTimer;
    private void Awake()
    {
        barImage = transform.Find("Bar").GetComponent<Image>();
        damageBarImage = transform.Find("DamageBar").GetComponent<Image>();
        damageColor = damageBarImage.color;
        damageColor.a = 0f;
        damageBarImage.color = damageColor;
    }
    private void Update()
    {
        if (damageColor.a > 0)
        {
            damagedHealthFadeTimer -= Time.deltaTime;
            if (damagedHealthFadeTimer < 0)
            {
                float fadeAmount = 5f;
                damageColor.a -= fadeAmount * Time.deltaTime;
                damageBarImage.color = damageColor;
            }
        }

    }

    public void SetHealth(float healthNormalized)
    {
        //If damaged
        if (barImage.fillAmount > healthNormalized)
        {
            if (damageColor.a <= 0)
            {
                //damaged abr is invisible
                damageBarImage.fillAmount = barImage.fillAmount;
                damageColor.a = 1;
                damageBarImage.color = damageColor;
                damagedHealthFadeTimer = DAMAGED_HEALTH_FADE_TIMER_MAX;
            }
            else
            {
                //damaged bar is already visible
                damageColor.a = 1;
                damageBarImage.color = damageColor;
                damagedHealthFadeTimer = DAMAGED_HEALTH_FADE_TIMER_MAX;
            }
        }
        barImage.fillAmount = healthNormalized;
    }
}
