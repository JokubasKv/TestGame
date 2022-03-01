using System;
using System.Collections.Generic;
using UnityEngine;

public static class scr_Models
{

    #region - Player -
    public enum PlayerStance
    {
        Stand,
        Crouch,
        Prone
    }
    [Serializable]
    public class PlayerSettingsModel
    {
        [Header("View Settings")]
        public float ViewXSensitivity;
        public float ViewYSensitivity;

        public float AiminigSensitivityEffector;

        public bool ViewXInverted;
        public bool ViewYInverted;

        [Header("Movement  Settings")]
        public bool SprintingHold;
        public float MovementSmoothing;

        [Header("Movement - Walking Settings")]
        public float walkingForwardSpeed;
        public float walkingStrafeSpeed;
        public float walkingBackwardSpeed;

        [Header("Movement - Running Settings")]
        public float RunningForwardSpeed;
        public float RunningStrafeSpeed;

        [Header("Jumping Settings")]
        public float jumpingHeight;
        public float jumpingFalloff;
        public float fallingMovementSmoothing;

        [Header("Speed Effectors")]
        public float SpeedEffector=1;
        public float CrouchSpeedEffector;
        public float ProneSpeedEffector;
        public float FallingSpeedEffector;
        public float AimingSpeedEffector;

        [Header("Is Grounded")]
        public float isGroundedRadius;
        public float isFallingSpeed;

    }
    [Serializable]
    public class CharacterStance
    {
        public float CameraHeight;
        public CapsuleCollider StanceCollider;
    }
    #endregion

    #region - Weapons -
    [Serializable]
    public class WeaponSettingModel
    {
        [Header("Weapon Sway")]
        public bool SwayYinverted;
        public bool SwayXinverted;
        public float SwayAmount;
        public float SwaySmoothing;
        public float SwayResetSmoothing;
        public float SwayClampX;
        public float SwayClampY;

        [Header("Weapon Movement Sway")]

        public bool MovementSwayYInverted;
        public bool MovementSwayXInverted;
        public float MovementSwayX;
        public float MovementSwayY;
        public float MovementSwaySmoothing;
    }
    #endregion

    public enum DamageType
    {
        Blunt,
        Electric,
        Exsplosion
    }
}
