using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 10;
    public float turnTimeout = 0.5f;
    public float boundsOffset = 0.02f;

    private Vector2 direction;
    private BoxCollider2D boxCollider;
    private float playerHalfWidth;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();

        playerHalfWidth = boxCollider.bounds.max.y - boxCollider.bounds.center.y;
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
        //float dstToCenter = Mathf.Abs(boxCollider.bounds.max.y - boxCollider.bounds.center.y);
        RaycastHit2D hit = Physics2D.Raycast(transform.position + transform.up * playerHalfWidth, transform.up, speed * Time.deltaTime);
        Debug.DrawRay(transform.position + transform.up * playerHalfWidth, transform.up * speed * Time.deltaTime, Color.red);

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
            // Raycast from left and right bounds

            StopAllCoroutines();
            StartCoroutine(MoveUpTimeout());
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

    private IEnumerator MoveUpTimeout()
    {
        float targetTime = Time.time + turnTimeout;
        bool leftRayClear = false;
        bool rightRayClear = false;
        bool shouldTurn = false;
        while (Time.time < targetTime)
        {
            RaycastHit2D hitLeft = Physics2D.Raycast(new Vector2(boxCollider.bounds.min.x + boundsOffset, boxCollider.bounds.max.y), Vector2.up);
            RaycastHit2D hitRight = Physics2D.Raycast(new Vector2(boxCollider.bounds.max.x - boundsOffset, boxCollider.bounds.max.y), Vector2.up);
            Debug.DrawRay(new Vector2(boxCollider.bounds.min.x + boundsOffset, boxCollider.bounds.max.y), Vector2.up, Color.red);
            Debug.DrawRay(new Vector2(boxCollider.bounds.max.x - boundsOffset, boxCollider.bounds.max.y), Vector2.up, Color.red);

            if (hitLeft.distance > 0.01f)
            {
                leftRayClear = true;
            }
            if (hitRight.distance > 0.01f)
            {
                rightRayClear = true;
            }
            if (leftRayClear && rightRayClear)
            {
                shouldTurn = true;
            }

            if (hitLeft.distance > 0.001f && hitRight.distance > 0.001f || shouldTurn)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                transform.position = new Vector2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
                break;
            }
            yield return null;
        }
    }
}
