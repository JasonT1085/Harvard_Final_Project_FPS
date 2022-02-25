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

    [Header("References")]
    public Transform cameraHolder;
    public Transform feetTransform;

    [Header("Settings")]
    public PlayerSettingsModel playerSettings;
    public float viewClampYMin = -70;
    public float viewClampYMax = 80;
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

    private Vector3 newMovementSpeed;
    private Vector3 newMovementSpeedVelocity;

    [Header("Weapon")]
    public scr_WeaponController curWeapon;
    
    public float weaponAnimationSpeed;

    [HideInInspector]
    public bool isGrounded;
    [HideInInspector]
    public bool isFalling;

    [Header("Aiming In")]
    public bool isAiming;


    #region - Awake -

    private void Awake()
    {
        defaultInput = new DefaultInput();

        defaultInput.Character.Movement.performed += e => input_Movement = e.ReadValue<Vector2>();
        defaultInput.Character.View.performed += e => input_View = e.ReadValue<Vector2>();
        defaultInput.Character.Jump.performed += e => Jump();
        defaultInput.Character.Crouch.performed += e => Crouch();
        defaultInput.Character.Prone.performed += e => Prone();
        defaultInput.Character.Sprinting.performed += e => ToggleSprint();
        defaultInput.Character.SprintRelease.performed += e => StopSprint();

        defaultInput.Weapon.Fire2Pressed.performed += e => AimingInPressed();
        defaultInput.Weapon.Fire2Released.performed += e => AimingInReleased();
        defaultInput.Enable();

        newCameraRotation = cameraHolder.localRotation.eulerAngles;
        newCharacterRotation = transform.localRotation.eulerAngles;

        characterController = GetComponent<CharacterController>();

        cameraHeight = cameraHolder.localPosition.y;

        if (curWeapon)
        {
            curWeapon.Init(this);
        }
    }
    #endregion

    #region - Update -
    private void Update()
    {
        SetisGrounded();
        SetIsFalling();
        CalculateView();
        CalculateMovement();
        CalculateJump();
        CalculateStance();
        CalculateAiming();
    }
    #endregion

    #region - Aiming -
    private void AimingInPressed()
    {
        isAiming = true;
    }

    private void AimingInReleased()
    {
        isAiming = false;
    }

    private void CalculateAiming()
    {
        if (!curWeapon)
        {
            return;
        }
        curWeapon.isAimingIn = isAiming;
    }

    #endregion

    #region - IsFalling / IsGrounded -

    private void SetisGrounded()
    {
        isGrounded = Physics.CheckSphere(feetTransform.position, playerSettings.isGroundedRadius, groundMask);

    }

    private void SetIsFalling()
    {

        isFalling = (!isGrounded && characterController.velocity.magnitude > playerSettings.isFallingSpeed);
    }

    #endregion

    #region - View / Movement - 
    private void CalculateView()
    {
        newCharacterRotation.y += playerSettings.ViewXSensitivity * (playerSettings.ViewXInverted ? -input_View.x : input_View.x) * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(newCharacterRotation);

        newCameraRotation.x += playerSettings.ViewYSensitivity * (playerSettings.ViewYInverted ? input_View.y : -input_View.y) * Time.deltaTime;
        newCameraRotation.x = Mathf.Clamp(newCameraRotation.x, viewClampYMin, viewClampYMax);

        cameraHolder.localRotation = Quaternion.Euler(newCameraRotation);
    }

    private void CalculateMovement()
    {
        if(input_Movement.y <= 0.2f)
        {
           isSprinting = false; 
        }

        var verticalSpeed = playerSettings.WalkingFowardSpeed;
        var horizontalSpeed = playerSettings.WalkingStrafeSpeed;

        if(isSprinting)
        {
            verticalSpeed = playerSettings.RunningForwardSpeed;
            horizontalSpeed = playerSettings.RunningStrafeSpeed;
        }

        //Speed Multipliers
        if (!isGrounded)
        {
            playerSettings.SpeedMultiplier = playerSettings.FallingSpeed;
        }
        else if (playerStance == PlayerStance.Crouch) 
        {
            playerSettings.SpeedMultiplier = playerSettings.CrouchSpeed;
            isSprinting = false;
        }
        else if (playerStance == PlayerStance.Prone) 
        {
            playerSettings.SpeedMultiplier = playerSettings.ProneSpeed;
            isSprinting = false;
        }
        else
        {
            playerSettings.SpeedMultiplier = 1;
        }

        weaponAnimationSpeed = characterController.velocity.magnitude / (playerSettings.WalkingFowardSpeed * playerSettings.SpeedMultiplier);

        if (weaponAnimationSpeed > 1)
        {
            weaponAnimationSpeed = 1;
        }

        verticalSpeed *= playerSettings.SpeedMultiplier;
        horizontalSpeed *= playerSettings.SpeedMultiplier;




        newMovementSpeed = Vector3.SmoothDamp(newMovementSpeed, new Vector3(horizontalSpeed * input_Movement.x * Time.deltaTime, 0, verticalSpeed * input_Movement.y * Time.deltaTime), ref newMovementSpeedVelocity, isGrounded ? playerSettings.MovementSmoothing : playerSettings.FallingSmoothing);

        var movementSpeed = transform.TransformDirection(newMovementSpeed);

        if (playerGravity > gravityMin)
        {
            playerGravity -= gravityAmount * Time.deltaTime;
        }

        if (playerGravity < -0.1f && isGrounded)
        {
            playerGravity = -0.1f;
        }


        movementSpeed.y += playerGravity;
        movementSpeed += jumpingForce * Time.deltaTime;
        characterController.Move(movementSpeed);
    }
    #endregion

    #region - Jumping -
    private void CalculateJump()
    {
        jumpingForce = Vector3.SmoothDamp(jumpingForce, Vector3.zero, ref jumpingForceVelocity, playerSettings.JumpingFallOff);
    }

        private void Jump()
    {
        if (!isGrounded)
        {
            return;
        }
        if (playerStance == PlayerStance.Crouch || playerStance == PlayerStance.Prone)
        {
            if (StanceCheck(playerStandStance.StanceCollider.height))
            {
                return;
            }                   
            playerStance = PlayerStance.Stand;
            return;
        }
        
        
        jumpingForce = Vector3.up * playerSettings.JumpingHeight;
        playerGravity = 0;

        curWeapon.TriggerJump();
    }

    #endregion

    #region - Stances -
    private void CalculateStance()
    {
        var currentStance = playerStandStance;

        if (playerStance == PlayerStance.Crouch)
        {
            currentStance = playerCrouchStance;
        }
        else if (playerStance == PlayerStance.Prone)
        {
            currentStance = playerProneStance;
        }

        cameraHeight = Mathf.SmoothDamp(cameraHolder.localPosition.y, currentStance.CameraHeight, ref cameraHeightVelocity, playerStanceSmoothing);

        cameraHolder.localPosition = new Vector3(cameraHolder.localPosition.x, cameraHeight, cameraHolder.localPosition.z);


        characterController.height = Mathf.SmoothDamp(characterController.height, currentStance.StanceCollider.height, ref stanceCapsuleHeightVelocity, playerStanceSmoothing);
        characterController.center = Vector3.SmoothDamp(characterController.center, currentStance.StanceCollider.center, ref stanceCapsuleCenterVelocity, playerStanceSmoothing);


    }


    private void Crouch()
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
    
    private void Prone()
    {
        if (playerStance == PlayerStance.Prone)
        {   
            if (StanceCheck(playerStandStance.StanceCollider.height))
            {
                return;
            }            
            playerStance = PlayerStance.Stand;
            return;
        }
        playerStance = PlayerStance.Prone;
    }

    private bool StanceCheck(float stanceCheckHeight)
    {
        var start = new Vector3(feetTransform.position.x, feetTransform.position.y + characterController.radius + stanceCheckErrorMargin, feetTransform.position.z);
        var end = new Vector3(feetTransform.position.x, feetTransform.position.y - characterController.radius - stanceCheckErrorMargin + stanceCheckHeight, feetTransform.position.z);


        return Physics.CheckCapsule(start, end, characterController.radius, playerMask);
    }
    #endregion

    #region - Sprinting -

    private void ToggleSprint()
    {
        if(input_Movement.y <= 0.2f)
        {
           isSprinting = false; 
           return;
        }        
        isSprinting = !isSprinting;


    }

        private void StopSprint()
    {
        if (playerSettings.holdSprint)
        {
        isSprinting = false;
        }
    }
    #endregion

    #region - Gizmos
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(feetTransform.position, playerSettings.isGroundedRadius);
    }

    #endregion
}

