using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public PlayerWeapon weapon;

    private DefaultInput defaultInput;
    [SerializeField]
    private Camera cam;

    [SerializeField]

    private LayerMask mask;
    // Start is called before the first frame update
    void Start()
    {
        defaultInput = new DefaultInput();
        defaultInput.Weapon.Fire1Pressed.performed += e => Shoot();
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
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, weapon.range, mask))
        {
            // hit something
            Debug.Log("We hit" + _hit.collider.name);

        }
    }   
    void ShootRelease()
    {

    }
}
