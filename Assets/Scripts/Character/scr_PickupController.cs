using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_PickupController : MonoBehaviour
{

    public scr_ProjectileWeapon gunScript;
    public Rigidbody rb;
    public BoxCollider coll;

    public Transform player, gunContainer, fpsCam;

    public float pickupRange;
    public float dropForwarForce, dropUpwardForce;

    public bool equipped;
    public static bool slotFull;

    private void Update()
    {
        Vector3 distanceToPlayer = player.position - transform.position;
    }
    private void Pickup()
    {

    }
    private void Drop()
    {

    }
}
