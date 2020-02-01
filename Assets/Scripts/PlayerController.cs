using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 10;

    private Vector2 direction;
    private BoxCollider2D boxCollider;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        HandleInput();
        Move();
    }

    private void HandleInput()
    {
        HandleInputUp();
        HandleInputDown();
        HandleInputRight();
        HandleInputLeft();
    }

    private void Move()
    {
        float dstToCenter = Mathf.Abs(boxCollider.bounds.max.y - boxCollider.bounds.center.y);
        RaycastHit2D hit = Physics2D.Raycast(transform.position + transform.up * dstToCenter, transform.up, speed * Time.deltaTime);
        Debug.DrawRay(transform.position + transform.up * dstToCenter, transform.up * speed * Time.deltaTime, Color.red);

        if (hit)
        {
            transform.Translate(Vector2.up * hit.distance);

        }
        else
        {
            transform.Translate(Vector2.up * speed * Time.deltaTime);
        }
    }

    private void RaycastForward()
    {
        //RaycastHit2D hit = Physics2D.Raycast(new Vector2(boxCollider.bounds.center.x, boxCollider.bounds.max.y), transform.up * 5);
    }

    private void HandleInputUp()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
    }

    private void HandleInputDown()
    {
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
        }
    }

    private void HandleInputRight()
    {
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
        }
    }

    private void HandleInputLeft()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
        }
    }
}
