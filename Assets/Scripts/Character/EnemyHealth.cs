using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float enemyHealth = 100f;

    public void takeDamage(float dmg)
    {
        enemyHealth-= dmg;
        if (enemyHealth <= 0) { death(); }
    }

    public void death()
    {
        Destroy(this.gameObject);
    }
}
