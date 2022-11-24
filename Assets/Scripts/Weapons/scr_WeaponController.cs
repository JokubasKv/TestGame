using UnityEngine;
using static scr_Models;
using UnityEngine.UI;

public class scr_WeaponController : MonoBehaviour
{
    [SerializeField]
    private scr_CharacterController characterController;
    [SerializeField]
    private scr_GameOver pauseMenu;

    [Header("References")]
    public Animator weaponAnimator;
    public Camera fpsCam;
    public LayerMask IgnoreLayersShooting;
    [Header("UI References")]
    public Text ammoText;


    [Header("Settings")]
    public WeaponSettingModel settings;

    public bool isInitialised;

    Vector3 newWeaponRotation;
    Vector3 newWeaponRotationVelocity;

    Vector3 targetWeaponRotation;
    Vector3 targetWeaponRotationVelocity;


    Vector3 newWeaponMovementRotation;
    Vector3 newWeaponMovementRotationVelocity;

    Vector3 targetWeaponMovementRotation;
    Vector3 targetWeaponMovementRotationVelocity;

    private bool isGroundedTrigger;

    private float fallingDelay;


    [Header("Weapon Sway")]
    public Transform weaponSwayObject;
    public float swayAmountA = 1;
    public float swayAmountB = 2;
    public float swayScale = 600;
    public float swayLerpSpeed = 14;
    private float swayTime;
    private Vector3 swayPosition;


    [Header("Sights")]
    public Transform sightTarget;
    public float sightOffset;
    public float aimingInTime;
    private Vector3 weaponSwayPosition;
    private Vector3 weaponSwayPositionVelocity;
    [HideInInspector]
    public bool isAimingIn;

    [Header("Shooting")]
    [Header("References")]
    public Transform attackPoint;
    public GameObject bullet;
    [Header("Graphic References")]
    public GameObject muzzleFlash;
    [Header("Shooting Settings")]
    public float shootForce;
    public float upwardForce;
    public float timeBetweenShooting;
    public float spread;
    public float reloadTime;
    public float timeBetweenShots;
    public int MagazineSize;
    public int bulletsPerTap;
    public bool isShotgun;
    public bool allowButtonHold;
    public bool buttonPressed;
    [Header("Recoil Settings")]
    public float recoilForce;

    [SerializeField]
    public int bulletsLeft;
    private int bulletsShot;

    bool readyToShoot;
    bool reloading;

    public bool allowInvoke = true;

    [Header("Audio")]
    AudioSource shootingSound;
    public AudioClip shotSoundEffect, explosionSoundEffect, reloadSoundEffect;

    DefaultInput defaultInput;
    #region - OnEnable/OnDisable-
    private void OnEnable()
    {
        defaultInput.Weapon.Fire1Pressed.performed += _ => ShootPressed();
        defaultInput.Weapon.Fire1Released.performed += _ => ShootReleased();
        defaultInput.Weapon.Reload.performed += _ => Reload();

        defaultInput.Enable();

        weaponAnimator.speed = 1;

        // Update when active state is changed
        UpdateAmmoText(); // update ammo text
    }
    private void OnDisable()
    {
        defaultInput.Weapon.Fire1Pressed.performed -= _ =>  ShootPressed();
        defaultInput.Weapon.Fire1Released.performed -= _ => ShootReleased();
        defaultInput.Weapon.Reload.performed -= _ => Reload();

        defaultInput.Disable();

        weaponAnimator.speed = 0;
    }
    #endregion
    #region - Start/Awake -
    private void Awake()
    {
        defaultInput = new DefaultInput();


        bulletsLeft = MagazineSize;
        readyToShoot = true;
        buttonPressed = false;
    }
    private void Start()
    {
        newWeaponRotation = transform.localRotation.eulerAngles;
        shootingSound = GetComponent<AudioSource>();
        float settings = PlayerPrefs.GetFloat("Audio");
        shootingSound.volume = settings;
        UpdateAmmoText(); // update ammo text
    }
    public void Initialise(scr_CharacterController CharacterController)
    {
        characterController = CharacterController;
        isInitialised = true;
    }
    #endregion
    #region - Update -
    private void Update()
    {
        if (!isInitialised)
        {
            characterController = GetComponentInParent<scr_CharacterController>();
            isInitialised = true;
            return;
        }

        if (readyToShoot && buttonPressed && !reloading && bulletsLeft > 0 && allowButtonHold && !characterController.isSprinting && !pauseMenu.paused)
        {
            bulletsShot = 0;
            Shoot();

            shootingSound.clip = shotSoundEffect;
            shootingSound.Play();
        }

        CalculateWeaponRotation();
        SetWeaponAnimations();
        CalculateWeaponSway();
        CalculateAimingIn();
    }
    #endregion
    #region - Calculate Aiming In -
    private void CalculateAimingIn()
    {
        var targetPosition = transform.position;
        if (isAimingIn)
        {
            targetPosition = characterController.cameraHolder.transform.position + (weaponSwayObject.transform.position - sightTarget.position) + (characterController.cameraHolder.transform.forward * sightOffset);
        }
        weaponSwayPosition = weaponSwayObject.transform.position;
        weaponSwayPosition = Vector3.SmoothDamp(weaponSwayPosition, targetPosition, ref weaponSwayPositionVelocity, aimingInTime);

        weaponSwayObject.transform.position = weaponSwayPosition + swayPosition;
    }
    #endregion
    #region - Calculate Weapon Rotation -
    private void CalculateWeaponRotation()
    {
        //Sway From Looking around
        targetWeaponRotation.y += (isAimingIn ? settings.SwayAmount / 2 : settings.SwayAmount) * (settings.SwayXinverted ? -characterController.input_View.x : characterController.input_View.x) * Time.deltaTime;
        targetWeaponRotation.x += (isAimingIn ? settings.SwayAmount / 2 : settings.SwayAmount) * (settings.SwayYinverted ? characterController.input_View.y : -characterController.input_View.y) * Time.deltaTime;

        targetWeaponRotation.x = Mathf.Clamp(targetWeaponRotation.x, -settings.SwayClampX, settings.SwayClampX);
        targetWeaponRotation.y = Mathf.Clamp(targetWeaponRotation.y, -settings.SwayClampY, settings.SwayClampY);
        targetWeaponRotation.z = isAimingIn ? 0 : targetWeaponRotation.y;


        targetWeaponRotation = Vector3.SmoothDamp(targetWeaponRotation, Vector3.zero, ref targetWeaponRotationVelocity, settings.SwayResetSmoothing);
        newWeaponRotation = Vector3.SmoothDamp(newWeaponRotation, targetWeaponRotation, ref newWeaponRotationVelocity, settings.SwaySmoothing);
        //Sway From Movement
        targetWeaponMovementRotation.z = (isAimingIn ? settings.MovementSwayX / 3 : settings.MovementSwayX) * (settings.MovementSwayXInverted ? -characterController.input_Movement.x : characterController.input_Movement.x);
        targetWeaponMovementRotation.x = (isAimingIn ? settings.MovementSwayY / 3 : settings.MovementSwayY) * (settings.MovementSwayYInverted ? -characterController.input_Movement.y : characterController.input_Movement.y);

        targetWeaponMovementRotation = Vector3.SmoothDamp(targetWeaponMovementRotation, Vector3.zero, ref targetWeaponMovementRotationVelocity, settings.MovementSwaySmoothing);
        newWeaponMovementRotation = Vector3.SmoothDamp(newWeaponMovementRotation, targetWeaponMovementRotation, ref newWeaponMovementRotationVelocity, settings.MovementSwaySmoothing);

        //Apply rotations
        transform.localRotation = Quaternion.Euler(newWeaponRotation + newWeaponMovementRotation);
    }
    #endregion
    #region - Animations-
    private void SetWeaponAnimations()
    {
        if (isGroundedTrigger)
        {
            fallingDelay = 0;
        }
        else
        {
            fallingDelay += Time.deltaTime;
        }

        if (characterController.isGrounded && !isGroundedTrigger && fallingDelay > 0.1f)
        {
            Debug.Log("Land");
            weaponAnimator.SetTrigger("Land");
            isGroundedTrigger = true;
        }
        else if (!characterController.isGrounded && isGroundedTrigger)
        {
            Debug.Log("Falling");
            weaponAnimator.SetTrigger("Falling");
            isGroundedTrigger = false;
        }


        weaponAnimator.SetBool("isSprinting", characterController.isSprinting);
        weaponAnimator.SetFloat("WeaponAnimationSpeed", characterController.weaponAnimationSpeed); ;
    }

    public void TriggerJump()
    {
        Debug.Log("TriggerJump");
        isGroundedTrigger = false;
        weaponAnimator.SetTrigger("Jump");
    }
    #endregion
    #region - Calculate Idle Weapon Sway -
    private void CalculateWeaponSway()
    {
        var targetPosition = LissajousCurve(swayTime, swayAmountA, swayAmountB) / (isAimingIn ? swayScale * 3 : swayScale);

        swayPosition = Vector3.Lerp(swayPosition, targetPosition, Time.smoothDeltaTime * swayLerpSpeed);
        swayTime += Time.deltaTime;
        if (swayTime > 6.3f)
        {
            swayTime = 0;
        }
    }

    private Vector3 LissajousCurve(float Time, float A, float B)
    {
        return new Vector3(Mathf.Sin(Time), A * Mathf.Sin(B * Time + Mathf.PI));
    }
    #endregion
    #region - Shoot -
    public void Shoot()
    {
        readyToShoot = false;

        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit,~IgnoreLayersShooting))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(75);
        }

        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;


        if (isShotgun)
        {
            shootingSound.clip = shotSoundEffect;
            shootingSound.Play();
            for (int i = 0; i < bulletsPerTap; i++)
            {
                float x = Random.Range(-spread, spread);
                float y = Random.Range(-spread, spread);
                float z = Random.Range(-spread, spread);

                Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, z);
                GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);
                //currentBullet.transform.forward = Quaternion.Euler(x, y, z) * directionWithoutSpread.normalized;
                currentBullet.transform.forward = directionWithSpread.normalized;


                currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
                if(upwardForce!=0)
                    currentBullet.GetComponent<Rigidbody>().AddForce(fpsCam.transform.up * upwardForce, ForceMode.Impulse);
            }
        }
        else
        {
            float x = Random.Range(-spread, spread);
            float y = Random.Range(-spread, spread);

            Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);
            GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);
            currentBullet.transform.forward = directionWithSpread.normalized;

            currentBullet.GetComponent<Rigidbody>().AddForce(currentBullet.transform.forward * shootForce, ForceMode.Impulse);
            currentBullet.GetComponent<Rigidbody>().AddForce(fpsCam.transform.up * upwardForce, ForceMode.Impulse);
        }



        //Instantiate muzzle flash, if you have one
        if (muzzleFlash != null)
        {
                Instantiate(muzzleFlash, attackPoint.position, fpsCam.transform.rotation);
        }

        bulletsLeft--;
        bulletsShot++;
        UpdateAmmoText(); // update ammo text

        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;

            if (recoilForce > 0)
            {
                characterController.playerGravity = -0.05f;
                characterController.AddImpact(-directionWithoutSpread.normalized, recoilForce);
            }

        }

        if (bulletsShot < bulletsPerTap && bulletsLeft > 0 && !isShotgun)
        {
            Invoke("Shoot", timeBetweenShots);
        }

    }
    private void ShootPressed()
    {
        if (!allowButtonHold && !buttonPressed)
        {
            Shoot();
        }
        buttonPressed = true;

    }
    private void ShootReleased()
    {
        buttonPressed = false;
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    private void UpdateAmmoText()
    {
        ammoText.text = $"Ammo {bulletsLeft}";
    }
    #endregion
    #region - Reload -
    public void Reload()
    {
        if (bulletsLeft == MagazineSize && reloading) return;

        reloading = true;
        if(bulletsLeft != MagazineSize)
        {
            if (shootingSound != null)
            {
                shootingSound.clip = reloadSoundEffect;
                shootingSound.Play();
            }
        }
        bulletsLeft = MagazineSize;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = MagazineSize;
        reloading = false;
        UpdateAmmoText(); // update ammo text
    }
    #endregion

}
