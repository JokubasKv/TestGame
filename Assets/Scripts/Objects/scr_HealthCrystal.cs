using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_HealthCrystal : MonoBehaviour
{
    public int healing = 1;

    private void OnTriggerEnter(Collider c)
    {

        if (c.gameObject.tag == "Player")
        {
            scr_CharacterController characterController = c.GetComponent<scr_CharacterController>();
            characterController.GetHealth(healing);
            Destroy(this.gameObject);
        }

    }
}
