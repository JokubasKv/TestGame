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
    [Header("Control References")]
    public InputActionReference shootInputActionReference;
    public InputActionReference reloadInputActionReference;
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
    [Header("Weapon Recoil Settings")]
    public CharacterController characterController;
    public float recoilForce;

    [SerializeField]
    private int bulletsLeft;
    private int bulletsShot;

    bool shooting;
    bool readyToShoot;
    bool reloading;


    public bool allowInvoke = true;

    private InputAction ShootInputAction
    {
        get
        {
            var action = shootInputActionReference.action;
            if (!action.enabled)
            {
                action.Enable();
            }
            return action;
        }
    }
    private InputAction ReloadInputAction
    {
        get
        {
            var action = reloadInputActionReference.action;
            if (!action.enabled)
            {
                action.Enable();
            }
            return action;
        }
    }

    private void OnEnable()
    {
       ShootInputAction.performed += e => Shoot();
       ReloadInputAction.performed += e => Reload();
    }
    private void OnDisable()
    {
        ShootInputAction.performed -= e => Shoot();
        ReloadInputAction.performed -= e => Reload();
    }


    DefaultInput defaultInput;
    private void Awake()
    {
        bulletsLeft = MagazineSize;
        readyToShoot = true;
        shooting = false;
    }


    public void Shoot()
    {
        if (readyToShoot && !shooting && !reloading && bulletsLeft > 0)
        {
            Debug.Log("Pew");
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
