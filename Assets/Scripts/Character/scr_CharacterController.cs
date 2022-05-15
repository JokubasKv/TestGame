using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static scr_Models;

public class scr_CharacterController : MonoBehaviour
{
    [HideInInspector]
    public CharacterController characterController;
    private DefaultInput defaultInput;
    [HideInInspector]
    public Vector2 input_Movement;
    [HideInInspector]
    public Vector2 input_View;

    private Vector3 newCameraRotation;
    private Vector3 newCharacterRotation;

    [Header("References")]
    public Transform cameraHolder;
    public Transform feetTransform;
    [Header("UI References")]
    public Text ammoText;
    public scr_HealthBarFade healthBar;
    public Text finalText;
    public Text startScoreText;
    public Text scoreText;
    public Image deathOverlay;
    public Button restartButton;

    [Header("Audio")]
    public AudioSource source;
    public AudioClip gameOverSound;
    public AudioClip damageSound;

    [Serializable]
    public class KeyValuePair
    {
        public DamageType damageType;
        public Image image;
    }

    public List<KeyValuePair> hurtImageList = new List<KeyValuePair>();
    private Dictionary<DamageType, Image> hurtImages=new Dictionary<DamageType, Image>();

    [Header("Settings")]
    public PlayerSettingsModel playerSettings;
    public float viewClampYmin = -70;
    public float viewClampYmax = 80;
    public float maxHitPoints=100;
    float hitpoints;
    public int numOfJumps;
    public LayerMask playerMask;
    public LayerMask groundMask;

    [Header("Gravity")]
    public float gravityAmount;
    public float gravityMin;
    public float playerGravity;

    public Vector3 jumpingForce;
    private Vector3 jumpingForceVelocity;

    [Header("Stance")]
    public PlayerStance playerStance;
    public float playerStanceSmoothing;
    public CharacterStance playerStandStance;
    public CharacterStance playerCrouchStance;
    public CharacterStance playerProneStance;
    private float stanceCheckErrorMargin = 0.05f;
    private float cameraHeight;
    private float cameraHeightVelocity;

    private Vector3 stanceCapsuleCenterVelocity;
    private float stanceCapsuleHeightVelocity;

    [HideInInspector]
    public bool isSprinting;
    [HideInInspector]
    public bool isFalling;
    [HideInInspector]
    public bool isGrounded;


    private Vector3 newMovementSpeed;
    private Vector3 newMovementSpeedVelocity;


    [Header("Weapon")]
    public scr_WeaponController currentWeapon;
    public float weaponAnimationSpeed;

    [Header("Aiming In")]
    public bool isAimingIn;
    [Header("Shooting")]
    public bool isShooting;
    [Header("Pickup Settings")]
    public float pickUpRange;
    public static bool slotFull;
    public scr_PickupController currentSlot;
    public LayerMask weaponLayer;

    public Vector3 impact = Vector3.zero;


    public bool gameOver = false;

    [Header("Audio")]
    AudioSource sfx;
    public AudioClip walkEffect, jumpEffect;


    #region - Awake -
    private void Awake() 
    {
        defaultInput = new DefaultInput();

        defaultInput.Character.Movement.performed += e => input_Movement = e.ReadValue<Vector2>();
        defaultInput.Character.View.performed += e => input_View = e.ReadValue<Vector2>();
        defaultInput.Character.Jump.performed += e => JumpPressed();
        defaultInput.Character.Crouch.performed += e => CrouchPressed();
        defaultInput.Character.Prone.performed += e => PronePressed();
        defaultInput.Character.Sprint.performed += e => ToggleSprint();
        defaultInput.Character.SprintReleased.performed += e => StopSprint();

        defaultInput.Weapon.Fire2Pressed.performed += e => AimingInPressed();
        defaultInput.Weapon.Fire2Released.performed += e => AimingInReleased();

        defaultInput.Weapon.Pickup.performed += e => PickUpPressed();
        defaultInput.Weapon.Drop.performed += e => DropPressed();

        defaultInput.Enable();

        foreach (var kvp in hurtImageList)
        {
            hurtImages[kvp.damageType] = kvp.image;
        }




        newCameraRotation = cameraHolder.localRotation.eulerAngles;
        newCharacterRotation = transform.localRotation.eulerAngles;

        characterController = GetComponent<CharacterController>();

        cameraHeight = cameraHolder.localPosition.y;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        hitpoints = maxHitPoints;

        if (currentWeapon)
        {
            currentWeapon.Initialise(this);
        }
    }
    #endregion
    #region - Update/Start -
    private void Update()
    {
        if (!gameOver)
        {
            SetIsGrounded();
            SetIsFalling();

            CalculateView();
            CalculateJump();
            CalculateStance();
            CalculateAimingIn();



            StanceCheck(playerCrouchStance.StanceCollider.height);
        }
    }
    private void FixedUpdate()
    {
        CalculateMovement();
        CalculateImpact();
    }

    private void Start()
    {
        UpdateAmmoText();
        UpdateHealth();
    }
    #endregion
    #region - Aiming In -
    private void AimingInPressed()
    {
        isAimingIn = true;
        isSprinting = false;
    }
    private void AimingInReleased()
    {
        isAimingIn = false;
    }
    private void CalculateAimingIn()
    {
        if (!currentWeapon)
        {
            return;
        }
        currentWeapon.isAimingIn = isAimingIn;
    }
    #endregion
    #region - IsFalling/IsGrounded-
    private void SetIsGrounded()
    {
        isGrounded = Physics.CheckSphere(feetTransform.position, playerSettings.isGroundedRadius, groundMask);

        if(isGrounded) numOfJumps = 1;
    }
    private void SetIsFalling()
    {
        isFalling = (!isGrounded && characterController.velocity.magnitude > playerSettings.isFallingSpeed);
    }
    #endregion
    #region - View/Movement -
    private void CalculateView()
    {
        newCharacterRotation.y += (isAimingIn ? playerSettings.ViewXSensitivity * playerSettings.AiminigSensitivityEffector : playerSettings.ViewXSensitivity) * (playerSettings.ViewXInverted ? -input_View.x : input_View.x) * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(newCharacterRotation);//Look horizontollly

        newCameraRotation.x += (isAimingIn ? playerSettings.ViewYSensitivity * playerSettings.AiminigSensitivityEffector : playerSettings.ViewYSensitivity) * (playerSettings.ViewYInverted ? input_View.y : -input_View.y) * Time.deltaTime;// look vertically
        newCameraRotation.x = Mathf.Clamp(newCameraRotation.x, viewClampYmin, viewClampYmax); //Clamp so player cant look up or down too far
        cameraHolder.localRotation = Quaternion.Euler(newCameraRotation);//Pass the looking rotation
    }
    private void CalculateMovement()
    {
        if (input_Movement.y <= 0.2f)//Check Player stopped to disable sprinting
        {
            isSprinting = false;
        }
        //Set Speed
        var verticalSpeed = playerSettings.walkingForwardSpeed;
        var horizontalSpeed = playerSettings.walkingStrafeSpeed;

        if (isSprinting)
        {
            verticalSpeed = playerSettings.RunningForwardSpeed;
            horizontalSpeed = playerSettings.RunningStrafeSpeed;
        }
        //Effectors
        if (!isGrounded)
        {
            playerSettings.SpeedEffector = playerSettings.FallingSpeedEffector;
        }
        else if (playerStance == PlayerStance.Crouch)
        {
            playerSettings.SpeedEffector = playerSettings.CrouchSpeedEffector;
        }
        else if (playerStance == PlayerStance.Prone)
        {
            playerSettings.SpeedEffector = playerSettings.ProneSpeedEffector;
        }
        else if (isAimingIn)
        {
            playerSettings.SpeedEffector = playerSettings.AimingSpeedEffector;
        }
        else
        {
            playerSettings.SpeedEffector = 1;
        }
        //Set animation Speed for weapon
        weaponAnimationSpeed = characterController.velocity.magnitude / (playerSettings.walkingForwardSpeed * playerSettings.SpeedEffector);
        if (weaponAnimationSpeed > 1)
        {
            weaponAnimationSpeed = 1;
        }
        //Change movement speed according to effectors
        verticalSpeed *= playerSettings.SpeedEffector;
        horizontalSpeed *= playerSettings.SpeedEffector;
        //Create new movement speed according to what happened before
        newMovementSpeed = Vector3.SmoothDamp(
            newMovementSpeed,
            new Vector3(horizontalSpeed * input_Movement.x * Time.deltaTime, 0, verticalSpeed * input_Movement.y * Time.deltaTime),
            ref newMovementSpeedVelocity,
            isGrounded ? playerSettings.MovementSmoothing : playerSettings.fallingMovementSmoothing
            ); 
        var MovementSpeed = transform.TransformDirection(newMovementSpeed);//Set movement relative to where player looking

        if (playerGravity > gravityMin)
        {
            playerGravity -= gravityAmount * Time.deltaTime;
        }

        if (playerGravity < -0.05f && isGrounded)
        {
            playerGravity = -0.05f;
        }

       MovementSpeed.y += playerGravity;
       MovementSpeed.y += jumpingForce.y * Time.deltaTime;

        characterController.Move(MovementSpeed);// passs the movement
    }
    #endregion
    #region - Jumping -
    private void CalculateJump()
    {
        jumpingForce = Vector3.SmoothDamp(jumpingForce, Vector3.up, ref jumpingForceVelocity, playerSettings.jumpingFalloff);
    }
    private void JumpPressed()
    {
        if (numOfJumps >= playerSettings.maxNumberOfJumps || playerStance == PlayerStance.Prone)
        {
            return;
        }
        if (playerStance == PlayerStance.Crouch)
        {
            if (StanceCheck(playerStandStance.StanceCollider.height))
            {
                return;
            }
            playerStance = PlayerStance.Stand;
            return;
        }

        //Jump
        jumpingForce = Vector3.up * playerSettings.jumpingHeight;
        playerGravity = 0;
        numOfJumps++;
        if(currentWeapon)
            if(currentWeapon.enabled)
                currentWeapon.TriggerJump();
    }
    #endregion
    #region - Stance -
    private void CalculateStance()
    {
        var currentStance=playerStandStance;
        if (playerStance == PlayerStance.Crouch)
        {
            currentStance = playerCrouchStance;
        }
        else if(playerStance == PlayerStance.Prone)
        {
            currentStance = playerProneStance;
        }

        cameraHeight = Mathf.SmoothDamp(cameraHolder.localPosition.y, currentStance.CameraHeight, ref cameraHeightVelocity,playerStanceSmoothing);

        cameraHolder.localPosition = new Vector3(cameraHolder.localPosition.x, cameraHeight, cameraHolder.localPosition.z);
        characterController.height = Mathf.SmoothDamp(characterController.height, currentStance.StanceCollider.height, ref stanceCapsuleHeightVelocity, playerStanceSmoothing);
        characterController.center = Vector3.SmoothDamp(characterController.center, currentStance.StanceCollider.center, ref stanceCapsuleCenterVelocity, playerStanceSmoothing);
    }
    private void CrouchPressed()
    {
        if (playerStance == PlayerStance.Crouch)
        {
            if (StanceCheck(playerStandStance.StanceCollider.height))
            {
                return;
            }
            playerStance = PlayerStance.Stand;
            return;
        }
        if (StanceCheck(playerCrouchStance.StanceCollider.height))
        {
            return;
        }
        playerStance = PlayerStance.Crouch;
    }
    private void PronePressed()
    {
        playerStance = PlayerStance.Prone;
    }
    #endregion
    #region - Sprinting -
    private bool StanceCheck(float stanceCheckHeight)
    {
        var start = new Vector3(feetTransform.position.x, feetTransform.position.y + characterController.radius + stanceCheckErrorMargin, feetTransform.position.z);
        var end = new Vector3(feetTransform.position.x, feetTransform.position.y + stanceCheckHeight - characterController.radius + stanceCheckErrorMargin, feetTransform.position.z);

        return Physics.CheckCapsule(start,end,characterController.radius,playerMask);
    }

    private void ToggleSprint()
    {
        if (input_Movement.y <= 0.2f || playerStance != PlayerStance.Stand || isAimingIn)
        {
            isSprinting = false;
            return;
        }
        isSprinting = !isSprinting;
    }

    private void StopSprint()
    {
        if (playerSettings.SprintingHold)
        {
            isSprinting = false;
        }
    }
    #endregion
    #region - Pickup/Drop -
    private void PickUpPressed()
    {
        if (!slotFull)
        {
            RaycastHit hit;
            if (Physics.Raycast(cameraHolder.transform.position, cameraHolder.transform.forward, out hit, pickUpRange, weaponLayer))
            {

                if (hit.transform.CompareTag("CanPickUp"))
                {
                    Debug.Log("Hit");
                    currentSlot = hit.transform.GetComponent<scr_PickupController>();
                    currentWeapon = hit.transform.GetComponent<scr_WeaponController>();

                    currentSlot.Pickup();

                    slotFull = true;
                }
            }
        }
        RaycastHit bhit;
        if (Physics.Raycast(cameraHolder.transform.position, cameraHolder.transform.forward, out bhit, pickUpRange))
        {
            Debug.Log(bhit);
            if (bhit.transform.CompareTag("Button"))
            {
                Debug.Log("Press");
                bhit.transform.GetComponent<scr_ButtonController>().Press();
            }
        }
    }

    private void DropPressed()
    {
        if (slotFull)
        {
            currentSlot.Drop(cameraHolder);
            slotFull = false;
        }
    }
    #endregion
    #region - Impact Force -
    private void CalculateImpact()
    {
        // apply the impact force:
        if (impact.magnitude > 0.2F) characterController.Move(impact * Time.deltaTime);
        // consumes the impact energy each cycle:
        impact = Vector3.Lerp(impact, Vector3.zero, 3 * Time.deltaTime);
    }
    public void AddImpact(Vector3 dir, float force)
    {
        dir.Normalize();
        impact += dir.normalized * force / gravityAmount;
    }
    #endregion
    #region - Damage -
    /*private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Crystal")
        {
            TakePlayerDamage();
        }
    }*/
    public void TakeDamage(int damage, DamageType damageType)
    {
        hitpoints -= damage;
        if (hitpoints < 0) hitpoints = 0;
        Image img;
        if(hurtImages.TryGetValue(damageType,out img))
        {
            img.gameObject.SetActive(true);
            img.canvasRenderer.SetAlpha(1);
            img.CrossFadeAlpha(0, 0.5f, false);
            //StartCoroutine(FadeOutUIImage(img, 0.5f, 0.2f));
        }
        UpdateHealth();
        if(hitpoints > 0)
        {
            source.clip = damageSound;
            source.Play();
        }

        if (hitpoints <= 0 && !gameOver)
        {
            StopGame();
        }
    }

    private void StopGame()
    {
        if (currentWeapon) currentWeapon.enabled = false;

        source.clip = gameOverSound;
        source.Play();
        
        finalText.gameObject.SetActive(true);
        finalText.CrossFadeAlpha(0, 0f, false);
        finalText.CrossFadeAlpha(1, 2f, false);

        startScoreText.gameObject.SetActive(false);
        int fianlScore = src_ScoreScript.GetScore();
        scoreText.text = "Final score: " + fianlScore;
        scoreText.gameObject.SetActive(true);
        scoreText.CrossFadeAlpha(0, 0f, false);
        scoreText.CrossFadeAlpha(1, 2f, false);

        restartButton.gameObject.SetActive(true);

        src_ScoreScript.SetScore(0);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        deathOverlay.gameObject.SetActive(true);
        deathOverlay.CrossFadeAlpha(0, 0f, false);
        deathOverlay.CrossFadeAlpha(1, 2f, false);
        gameOver = true;
    }
    #endregion
    #region - Health -
    public void Heal(int value)
    {
        hitpoints += value;
        if(hitpoints > maxHitPoints)
        {
            hitpoints = maxHitPoints;
        }
        UpdateHealth();
    }
    public void Heal(float value)
    {
        hitpoints += value;
        UpdateHealth();
    }

    public float GetHealthNormalized()
    {
        return (float)hitpoints / maxHitPoints;
    }
    #endregion
    #region -UI-
    private void UpdateAmmoText()
    {
        ammoText.text = "";
    }
    private void UpdateHealth()
    {
        healthBar.SetHealth(GetHealthNormalized());
    }
    IEnumerator FadeOutUIImage(Image img, float time, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        img.CrossFadeAlpha(0, time, false);
        yield return new WaitForSeconds(time);
        img.gameObject.SetActive(false);
    }
    #endregion
    #region - Gizmos -
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(feetTransform.position, playerSettings.isGroundedRadius);
        Gizmos.DrawRay(cameraHolder.transform.position, cameraHolder.transform.forward * pickUpRange);
    }
    #endregion
}

