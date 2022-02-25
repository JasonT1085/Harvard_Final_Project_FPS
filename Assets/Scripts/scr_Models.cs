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
        
        public bool ViewXInverted;
        public bool ViewYInverted;

        [Header("Movement Settings")]
        public bool holdSprint;
        public float MovementSmoothing;

        [Header("Movement - Running")]
        public float RunningForwardSpeed;
        public float RunningStrafeSpeed;

        [Header("Movement - Walking")]
        public float WalkingFowardSpeed;
        public float WalkingStrafeSpeed;
        public float WalkingBackwardSpeed;

        [Header("Jumping")]
        public float JumpingHeight;
        public float JumpingFallOff;
        public float FallingSmoothing;

        [Header("Speed Multiplier")]
        public float SpeedMultiplier = 1;
        public float CrouchSpeed;
        public float ProneSpeed;
        public float FallingSpeed;

        [Header("Is Grounded / Falling")]
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
    public class WeaponSettingsModel
    {
        [Header("Weapon Sway")]
        public float SwayAmount;
        public float SwaySmoothing;

        public bool SwayYInverted;
        public bool SwayXInverted;
        public float SwayResetSmoothing;
        public float SwayClampX;
        public float SwayClampY;

        [Header("Weapon Movement Sway")]
        public float MovementSwayX;
        public float MovementSwayY;
        
        public bool MovementSwayYInverted;
        public bool MovementSwayXInverted;
        public float MovementSwaySmoothing;
    }

    #endregion
}

