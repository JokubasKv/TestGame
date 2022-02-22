using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static scr_Models;

public class scr_CharacterController : MonoBehaviour
{
    private CharacterController characterController;
    private DefaultInput defaultInput;
    [HideInInspector]
    public Vector2 input_Movement;
    [HideInInspector]
    public Vector2 input_View;

    private Vector3 newCameraRotation;
    private Vector3 newCharacterRotation;

    //Events
    public delegate void ShootAction();
    public static event ShootAction OnShootPressed;
    public static event ShootAction OnShootReleased;

    [Header("References")]
    public Transform cameraHolder;
    public Transform feetTransform;

    [Header("Settings")]
    public PlayerSettingsModel playerSettings;
    public float viewClampYmin = -70;
    public float viewClampYmax = 80;
    public LayerMask playerMask;
    public LayerMask groundMask;

    [Header("Gravity")]
    public float gravityAmount;
    public float gravityMin;
    private float playerGravity;

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
    public float dropForwardForce, dropUpwardForce;
    public bool equipped;
    public static bool slotFull;


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

        defaultInput.Weapon.Fire1Pressed.performed += e => OnShootPressed();
        defaultInput.Weapon.Fire1Released.performed += e => OnShootReleased();

        defaultInput.Weapon.Pickup.performed += e => Pickup();

        defaultInput.Enable();

        newCameraRotation = cameraHolder.localRotation.eulerAngles;
        newCharacterRotation = transform.localRotation.eulerAngles;

        characterController = GetComponent<CharacterController>();

        cameraHeight = cameraHolder.localPosition.y;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;


        if (currentWeapon)
        {
            currentWeapon.Initialise(this);
        }
    }
    #endregion
    #region - Update -
    private void Update()
    {
        SetIsGrounded();
        SetIsFalling();

        CalculateView();
        CalculateMovement();
        CalculateJump();
        CalculateStance();
        CalculateAimingIn();

        StanceCheck(playerCrouchStance.StanceCollider.height);
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

        weaponAnimationSpeed = characterController.velocity.magnitude / (playerSettings.walkingForwardSpeed * playerSettings.SpeedEffector);
        if (weaponAnimationSpeed > 1)
        {
            weaponAnimationSpeed = 1;
        }

        verticalSpeed *= playerSettings.SpeedEffector;
        horizontalSpeed *= playerSettings.SpeedEffector;

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

        if (playerGravity < -0.1f && isGrounded)
        {
            playerGravity = -0.1f;
        }

       MovementSpeed.y += playerGravity;
       MovementSpeed += jumpingForce * Time.deltaTime;

        characterController.Move(MovementSpeed);// passs the movement
    }
    #endregion
    #region - Jumping -
    private void CalculateJump()
    {
        jumpingForce = Vector3.SmoothDamp(jumpingForce, Vector3.zero, ref jumpingForceVelocity, playerSettings.jumpingFalloff);
    }
    private void JumpPressed()
    {
        if (!isGrounded || playerStance == PlayerStance.Prone)
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
        if(playerStance == PlayerStance.Crouch)
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
    #region -Pickup and Drop-
    private void Pickup()
    {
        Vector3 distanceToPlayer = characterController.transform.position - transform.position;
        if(!equipped && distanceToPlayer.magnitude <= pickUpRange && !slotFull)
        {
            equipped = true;
            slotFull = true;


        }
    }
    private void Drop()
    {
        if (equipped)
        {

        }
    }
    #endregion
    #region -Shooting -

    #endregion
    #region - Gizmos -
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(feetTransform.position, playerSettings.isGroundedRadius);
    }
    #endregion
}

