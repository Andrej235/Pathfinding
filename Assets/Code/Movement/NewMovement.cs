using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMovement : MonoBehaviour
{
    //Basic movement
    [SerializeField] private float movementSpeed;
    [SerializeField] private Rigidbody2D rigidBody2D;
    private Vector2 movementDirection;

    //Dash mehanic
    private bool isDashing = false;
    private bool canDash = true;
    [SerializeField] private float dashSpeed = 0f;
    [SerializeField] private float dashTime = 0f;
    [SerializeField] private float dashCooldown = 0f;

    private void Update()
    {
        if (isDashing)
        {
            return;
        }
        else
        {
            ProcessInputs();
        }
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }
        else
        {
            Move();
        }
    }
    private void ProcessInputs()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        movementDirection = new Vector2(moveX, moveY);

        if (canDash && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Dash());
        }
    }

    private void Move()
    {
        rigidBody2D.velocity = new Vector2(movementDirection.x, movementDirection.y).normalized * movementSpeed;
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;
        rigidBody2D.velocity = new Vector2(movementDirection.x, movementDirection.y).normalized * dashSpeed;
        yield return new WaitForSeconds(dashTime);
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}
