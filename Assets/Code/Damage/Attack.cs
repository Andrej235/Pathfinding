using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    private float damage = 30;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        if(damageable != null)
        {
            damageable.TakeDamage(damage);
        }
    }
}
