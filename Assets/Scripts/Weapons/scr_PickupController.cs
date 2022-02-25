using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scr_PickupController : MonoBehaviour
{

    public scr_WeaponController gunScript;
    public scr_CharacterController characterScript;
    public Rigidbody rb;
    public BoxCollider coll;
    public Text ammoText;

    public Transform player, gunContainer, fpsCam;

    public float pickupRange;
    public float dropForwarForce, dropUpwardForce;

    public bool equipped;
    public static bool slotFull;

    private void OnEnable()
    {

        scr_CharacterController.OnPickUpPressed += Pickup;
        scr_CharacterController.OnDropPressed += Drop;
    }
    private void OnDisable()
    {
        scr_CharacterController.OnPickUpPressed -= Pickup;
        scr_CharacterController.OnDropPressed -= Drop;
    }
    private void Start()
    {
        if (!characterScript)
        {
            characterScript = FindObjectOfType<scr_CharacterController>();
        }
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
            slotFull = true;
        }
    }
    private void Pickup()
    {
        Vector3 distanceToPlayer = player.position - transform.position;
        if (!equipped && CheckPickUp() && !slotFull)
        {
            equipped = true;
            slotFull = true;

            transform.SetParent(gunContainer);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.Euler(Vector3.zero);
            transform.localScale = Vector3.one;

            rb.isKinematic = true;
            coll.isTrigger = true;

            gunScript.enabled = true;
            characterScript.currentWeapon = gunScript.GetComponent<scr_WeaponController>();
        }
    }
    private void Drop()
    {
        if (equipped)
        {
            equipped = false;
            slotFull = false;

            transform.SetParent(null);
            rb.isKinematic = false;
            coll.isTrigger = false;

            rb.velocity = player.GetComponent<CharacterController>().velocity;

            rb.AddForce(fpsCam.forward * dropForwarForce, ForceMode.Impulse);
            rb.AddForce(fpsCam.up * dropUpwardForce, ForceMode.Impulse);

            float random = Random.Range(-1f, 1f);
            rb.AddTorque(new Vector3(random, random, random) * 10);

            gunScript.enabled = false;
            characterScript.currentWeapon = null;
            Debug.Log("Drop");
            UpdateAmmoText();

        }
    }

    private void UpdateAmmoText()
    {
        ammoText.text = "";
    }

    private bool CheckPickUp()
    {
        RaycastHit hit;

        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, pickupRange))
        {
            if (hit.transform.name == this.transform.name )
            {
                return true;
            }
            else
                return false;

        }
        return false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(fpsCam.transform.position, fpsCam.transform.forward*pickupRange);
    }
}
