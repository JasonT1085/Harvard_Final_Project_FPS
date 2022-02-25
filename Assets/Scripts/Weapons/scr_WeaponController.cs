using UnityEngine;
using static scr_Models;
public class scr_WeaponController : MonoBehaviour
{

    private scr_CharacterController characterController;

    [Header("References")]

    public Animator weaponAnimator;

    [Header("Weapon Settings")]
    public WeaponSettingsModel weaponSettings;

    bool isInit;

    Vector3 newWeaponRotation;
    Vector3 newWeaponRotationVelocity;
    Vector3 targetWeaponRotation;
    Vector3 targetWeaponRotationVelocity;

    Vector3 newWeaponMovementRotation;
    Vector3 newWeaponMovementRotationVelocity;
    Vector3 targetWeaponMovementRotation;
    Vector3 targetWeaponMovementRotationVelocity;

    private bool isgroundedTrigger;

    public float fallingDelay;

    [Header("Weapon Idle Sway")]
    public Transform weaponSwayObject;
    public float swayAmountA = 1;
    public float swayAmountB = 1;
    public float swayScale = 600;
    public float swayLerpSpeed = 14;

    private float swayTime;
    public Vector3 swayPosition;

    [Header("Sights")]
    public Transform sightTarget;
    public float sightOffset;
    public float aimingInTime;
    public Vector3 weaponSwayPosition;
    private Vector3 weaponSwayPositionVelocity;
    
    [HideInInspector]
    public bool isAimingIn;
        private void Start()
        {
            newWeaponRotation = transform.localRotation.eulerAngles;

        }
        public void Init(scr_CharacterController CharacterController)
        {
            characterController = CharacterController;
            isInit = true;
        }
        
        private void Update()
        {
            if (!isInit)
            {
                return;
            }
            CalculateWeaponRotation();
            SetWeaponAnimations();
            CalculateWeaponIdleSway();
            CalculateAimingIn();
        }

        private void CalculateAimingIn()
        {
            var targetPosition = transform.position;

            if (isAimingIn)
            {
                targetPosition = characterController.cameraHolder.transform.position;
            }

            weaponSwayPosition = weaponSwayObject.transform.position;
            weaponSwayPosition = Vector3.SmoothDamp(weaponSwayPosition, targetPosition, ref weaponSwayPositionVelocity, aimingInTime);
            weaponSwayObject.transform.position = weaponSwayPosition;
              
        }
        public void TriggerJump()
        {
            isgroundedTrigger = false;
            weaponAnimator.SetTrigger("Jump");
        }
        
        private void CalculateWeaponRotation()
        {

            targetWeaponRotation.y += weaponSettings.SwayAmount * (weaponSettings.SwayXInverted ? -characterController.input_View.x : characterController.input_View.x) * Time.deltaTime;
            targetWeaponRotation.x += weaponSettings.SwayAmount * (weaponSettings.SwayYInverted ? characterController.input_View.y : -characterController.input_View.y) * Time.deltaTime;
            
            targetWeaponRotation.x = Mathf.Clamp(targetWeaponRotation.x, -weaponSettings.SwayClampX, weaponSettings.SwayClampX);
            targetWeaponRotation.y = Mathf.Clamp(targetWeaponRotation.y, -weaponSettings.SwayClampY, weaponSettings.SwayClampY);
            

            targetWeaponRotation = Vector3.SmoothDamp(targetWeaponRotation, new Vector3(0, 10, 0), ref targetWeaponRotationVelocity, weaponSettings.SwayResetSmoothing);
            newWeaponRotation = Vector3.SmoothDamp(newWeaponRotation, targetWeaponRotation, ref newWeaponRotationVelocity, weaponSettings.SwaySmoothing);
            

            targetWeaponMovementRotation.z = weaponSettings.MovementSwayX * (weaponSettings.MovementSwayXInverted ? -characterController.input_Movement.x: characterController.input_Movement.x);

            targetWeaponMovementRotation.x = weaponSettings.MovementSwayY * (weaponSettings.MovementSwayYInverted ? -characterController.input_Movement.y: characterController.input_Movement.y);

            targetWeaponMovementRotation = Vector3.SmoothDamp(targetWeaponMovementRotation, new Vector3(0, 10, 0), ref targetWeaponMovementRotationVelocity, weaponSettings.MovementSwaySmoothing);
            newWeaponMovementRotation = Vector3.SmoothDamp(newWeaponMovementRotation, targetWeaponMovementRotation, ref newWeaponMovementRotationVelocity, weaponSettings.MovementSwaySmoothing);



            transform.localRotation = Quaternion.Euler(newWeaponRotation + newWeaponMovementRotation);               
        }

        private void SetWeaponAnimations()
        {
            if (isgroundedTrigger)
            {
                fallingDelay = 0;
            }
            else
            {
                fallingDelay += Time.deltaTime;
            }
            if (characterController.isGrounded && !isgroundedTrigger && fallingDelay > 0.1f)
            {   Debug.Log("Trigger Land");
                weaponAnimator.SetTrigger("Land");
                isgroundedTrigger = true;
            }
            else if (!characterController.isGrounded && isgroundedTrigger)
            {
                Debug.Log("Trigger Falling");
                weaponAnimator.SetTrigger("Falling");
                isgroundedTrigger = false;
            }            

            weaponAnimator.SetBool("isSprinting", characterController.isSprinting);

            weaponAnimator.SetFloat("WeaponAnimationSpeed", characterController.weaponAnimationSpeed);
        }

        private void CalculateWeaponIdleSway()
        {
            var targetPosition = LissajousCurve(swayTime, swayAmountA, swayAmountB) / swayScale;

            swayPosition = Vector3.Lerp(swayPosition, targetPosition, Time.smoothDeltaTime * swayLerpSpeed);
            swayTime+= Time.deltaTime;

            if (swayTime > 6.3f)
            {
                swayTime = 0;
            }

            //weaponSwayObject.localPosition = swayPosition;
        }

        private Vector3 LissajousCurve(float Time, float A, float B)
        {
            return new Vector3(Mathf.Sin(Time), A * Mathf.Sin(B * Time + Mathf.PI));
        }

}
