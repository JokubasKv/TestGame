using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scr_PickupController : MonoBehaviour
{
    public scr_WeaponController gunScript;
    public Rigidbody rb;
    public BoxCollider coll;


    public Transform player, gunContainer;


    public float dropForwarForce, dropUpwardForce;

    public bool equipped;
    private void Start()
    {
        if (!equipped)
        {
            gunScript.enabled = false;
            rb.isKinematic = false;
            coll.isTrigger = false;
        }
        if (equipped)
        {
            gunScript.enabled = true;
            rb.isKinematic = true;
            coll.isTrigger = true;
        }
    }
    public void Pickup()
    {
        Debug.Log("Pickup");
        if (!equipped)
        {
            equipped = true;

            transform.SetParent(gunContainer);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.Euler(Vector3.zero);
            transform.localScale = Vector3.one;

            rb.isKinematic = true;
            coll.isTrigger = true;

            gunScript.enabled = true;
        }
    }
    public void Drop(Transform viewDirection)
    {
        if (equipped)
        {
            equipped = false;

            transform.SetParent(null);
            rb.isKinematic = false;
            coll.isTrigger = false;

            rb.velocity = player.GetComponent<CharacterController>().velocity;

            rb.AddForce(viewDirection.forward * dropForwarForce, ForceMode.Impulse);
            rb.AddForce(viewDirection.up * dropUpwardForce, ForceMode.Impulse);

            float random = Random.Range(-1f, 1f);
            rb.AddTorque(new Vector3(random, random, random) * 10);

            gunScript.enabled = false;

            UpdateAmmoText();

        }
    }

    private void UpdateAmmoText()
    {
        gunScript.ammoText.text = "";
    }
}
