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

    //Weapon Effects
    //Muzzleflash
    public ParticleSystem muzzleflash;


    //Rate of Fire
    [SerializeField]
    private float rateOfFire;
    public float nextFire = 0;

    [SerializeField]
    public float weaponRange;
    // Start is called before the first frame update
    void Start()
    {
        muzzleflash.Stop();
        if (cam == null)
        {
            Debug.LogError("No Camera Referenced");
            this.enabled = false;
        }


    }

    // Update is called once per frame
    void Update()
    {    
        if(Input.GetButton("Fire1") && currentAmmo > 0)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        RaycastHit _hit;

        if(Time.time > nextFire)
        {
            nextFire = Time.time + rateOfFire;

            currentAmmo--;

            StartCoroutine(WeaponEffects());

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

    IEnumerator WeaponEffects()
    {
        muzzleflash.Play();
        yield return new WaitForEndOfFrame();
        muzzleflash.Stop();
    }

    void ShootRelease()
    {

    }
}
