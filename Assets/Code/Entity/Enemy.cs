using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    private float health = 100;

    private void Update()
    {
        if(health < 1)
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
    }
}
