using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public PlayerWeapon weapon;
    public float weaponDamage = 10f;

    [SerializeField]
    private DefaultInput defaultInput;
    [SerializeField]
    private Camera cam;

    [SerializeField]
    public int currentAmmo;
    private LayerMask mask;

    //Rate of Fire
    [SerializeField]
    private float rateOfFire;
    public float nextFire = 0;

    [SerializeField]
    public float weaponRange;
    // Start is called before the first frame update
    void Start()
    {
        defaultInput = new DefaultInput();
        if (currentAmmo > 0) { defaultInput.Weapon.Fire1Pressed.performed += e => Shoot(); } 
        defaultInput.Weapon.Fire1Released.performed += e => ShootRelease();        
        defaultInput.Enable();
        if (cam == null)
        {
            Debug.LogError("No Camera Referenced");
            this.enabled = false;
        }


    }

    // Update is called once per frame
    void Update()
    {    
    }

    void Shoot()
    {
        RaycastHit _hit;

        if(Time.time > nextFire)
        {
            nextFire = Time.time + rateOfFire;

            currentAmmo--;

            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, weapon.range))
            {
                // hit something
                if (_hit.transform.tag == "Enemy")
                {
                    Debug.Log("Enemy Hit");
                    EnemyHealth enemyHealthScript = _hit.transform.GetComponent<EnemyHealth>();
                    enemyHealthScript.takeDamage(weaponDamage);
                }
                else
                {
                    Debug.Log("We hit" + _hit.collider.name);
                }
                
            }            
        }

    }   
    void ShootRelease()
    {

    }
}
