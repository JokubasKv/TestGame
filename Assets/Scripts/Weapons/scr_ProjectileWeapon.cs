using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class scr_ProjectileWeapon : MonoBehaviour
{
    [Header ("References")]
    public Camera playerCamera;
    public Transform attackPoint;
    public GameObject bullet;
    [Header("Graphic References")]
    public GameObject muzzleFlash;
    [Header ("Weapon Settings")]
    public float shootForce;
    public float upwardForce;
    public float timeBetweenShooting;
    public float spread;
    public float reloadTime;
    public float timeBetweenShots;
    public int MagazineSize;
    public int bulletsPerTap;
    public bool allowButtonHold;
    public bool buttonPressed;
    [Header("Weapon Recoil Settings")]
    public CharacterController characterController;
    public float recoilForce;

    [SerializeField]
    private int bulletsLeft;
    private int bulletsShot;

    bool readyToShoot;
    bool reloading;


    public bool allowInvoke = true;


    private void OnEnable()
    {
        scr_CharacterController.OnShootPressed += ShootPressed;
        scr_CharacterController.OnShootReleased += ShootReleased;
    }
    private void OnDisable()
    {
        scr_CharacterController.OnShootPressed -= ShootPressed;
        scr_CharacterController.OnShootReleased += ShootReleased;
    }


    DefaultInput defaultInput;
    private void Awake()
    {
        bulletsLeft = MagazineSize;
        readyToShoot = true;
        buttonPressed = false; 
    }

    private void Update()
    {
        if (readyToShoot && buttonPressed && !reloading && bulletsLeft > 0 && allowButtonHold)
        {
            Shoot();
        }
        
    }


    public void Shoot()
    {

        Debug.Log("Pew");
        buttonPressed = true;
        bulletsShot = 0;
        readyToShoot = false;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(75);
        }

        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);

        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);
        currentBullet.transform.forward = directionWithSpread.normalized;

        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(playerCamera.transform.up * upwardForce, ForceMode.Impulse);

        //Instantiate muzzle flash, if you have one
        if (muzzleFlash != null)
            Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);


        bulletsLeft--;
        bulletsShot++;

        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;

            characterController.Move(-directionWithSpread.normalized * recoilForce);

        }

        if (bulletsShot < bulletsPerTap && bulletsLeft > 0)
        {
            Invoke("Shoot", timeBetweenShots);
        }

    }
    private void ShootPressed()
    {
        if(!allowButtonHold && !buttonPressed)
        {
            Shoot();
        }
        buttonPressed = true;
        
    }
    private void ShootReleased()
    {
        buttonPressed = false;
        Debug.Log("Released M1");
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Reload()
    {
        if (bulletsLeft == MagazineSize && reloading) return;

        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = MagazineSize;
        reloading = false;
    }
}
