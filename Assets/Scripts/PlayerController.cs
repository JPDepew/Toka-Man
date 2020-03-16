using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 10;
    public float turnTimeout = 0.5f;
    public float boundsOffset = 0.02f;
    public LayerMask collisionLayerMask;
    public GameObject dataPointCollect;

    private Vector2 direction;
    private BoxCollider2D boxCollider;
    private float playerHalfWidth;
    private const float hitDstOffset = 0.001f;
    private enum Direction { UP, DOWN, LEFT, RIGHT }

    public delegate void OnDataPickup();
    public static event OnDataPickup onDataPickup;

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
        RaycastHit2D hit = ForwardsRaycast();

        if (hit)
        {
            transform.Translate(Vector2.up * hit.distance);

        }
        else
        {
            transform.Translate(Vector2.up * speed * Time.deltaTime);
        }
    }

    private RaycastHit2D ForwardsRaycast()
    {
        Debug.DrawRay(transform.position + transform.up * playerHalfWidth, transform.up * speed * Time.deltaTime, Color.red);
        return Physics2D.Raycast(transform.position + transform.up * playerHalfWidth, transform.up, speed * Time.deltaTime, collisionLayerMask);
    }

    private void HandleInputUp()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            Direction dir = Direction.UP;
            if (IsMovingInDirection(dir)) return;
            StopAllCoroutines();
            StartCoroutine(MoveTimeout(dir));
        }
    }

    private void HandleInputDown()
    {
        Direction dir = Direction.DOWN;
        if (IsMovingInDirection(dir)) return;
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            StopAllCoroutines();
            StartCoroutine(MoveTimeout(dir));
        }
    }

    private void HandleInputRight()
    {
        Direction dir = Direction.RIGHT;
        if (IsMovingInDirection(dir)) return;
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            StopAllCoroutines();
            StartCoroutine(MoveTimeout(dir));
        }
    }

    private void HandleInputLeft()
    {
        Direction dir = Direction.LEFT;
        if (IsMovingInDirection(dir)) return;

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            StopAllCoroutines();
            StartCoroutine(MoveTimeout(dir));
        }
    }

    private IEnumerator MoveTimeout(Direction direction)
    {
        float targetTime = Time.time + turnTimeout;
        bool leftRayClear = false;
        bool rightRayClear = false;
        bool shouldTurn = false;
        while (Time.time < targetTime)
        {
            (RaycastHit2D hit1, RaycastHit2D hit2, RaycastHit2D hitMid) = RaycastsBasedOnDirection(direction);

            if (hit1.distance > 0.01f)
            {
                leftRayClear = true;
            }
            if (hit2.distance > 0.01f)
            {
                rightRayClear = true;
            }
            if (leftRayClear && rightRayClear)
            {
                if (hitMid.distance > 0.01f)
                {
                    shouldTurn = true;
                }
            }

            if (hit1.distance > hitDstOffset && hit2.distance > hitDstOffset || shouldTurn)
            {
                transform.rotation = GetQuaternionDirection(direction);
                transform.position = new Vector2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
                break;
            }
            yield return null;
        }
    }

    private (RaycastHit2D, RaycastHit2D, RaycastHit2D) RaycastsBasedOnDirection(Direction direction)
    {
        Vector2 vector1;
        Vector2 vector2;
        Vector2 vectorMid;
        Vector2 directionVector;
        switch (direction)
        {
            case Direction.UP:
                vector1 = new Vector2(boxCollider.bounds.min.x + boundsOffset, boxCollider.bounds.max.y);
                vector2 = new Vector2(boxCollider.bounds.max.x - boundsOffset, boxCollider.bounds.max.y);
                vectorMid = new Vector2(boxCollider.bounds.center.x - boundsOffset, boxCollider.bounds.max.y);
                directionVector = Vector2.up;
                break;
            case Direction.DOWN:
                vector1 = new Vector2(boxCollider.bounds.min.x + boundsOffset, boxCollider.bounds.min.y);
                vector2 = new Vector2(boxCollider.bounds.max.x - boundsOffset, boxCollider.bounds.min.y);
                vectorMid = new Vector2(boxCollider.bounds.center.x - boundsOffset, boxCollider.bounds.min.y);
                directionVector = Vector2.down;
                break;
            case Direction.LEFT:
                vector1 = new Vector2(boxCollider.bounds.min.x, boxCollider.bounds.min.y + boundsOffset);
                vector2 = new Vector2(boxCollider.bounds.min.x, boxCollider.bounds.max.y - boundsOffset);
                vectorMid = new Vector2(boxCollider.bounds.min.x, boxCollider.bounds.center.y - boundsOffset);
                directionVector = Vector2.left;
                break;
            case Direction.RIGHT:
                vector1 = new Vector2(boxCollider.bounds.max.x, boxCollider.bounds.min.y + boundsOffset);
                vector2 = new Vector2(boxCollider.bounds.max.x, boxCollider.bounds.max.y - boundsOffset);
                vectorMid = new Vector2(boxCollider.bounds.max.x, boxCollider.bounds.center.y - boundsOffset);
                directionVector = Vector2.right;
                break;
            default:
                vector1 = new Vector2();
                vector2 = new Vector2();
                vectorMid = new Vector2();
                directionVector = new Vector2();
                break;
        }

        RaycastHit2D hit1 = Physics2D.Raycast(vector1, directionVector);
        RaycastHit2D hit2 = Physics2D.Raycast(vector2, directionVector);
        RaycastHit2D hitMid = Physics2D.Raycast(vectorMid, directionVector);
        Debug.DrawRay(vector1, directionVector, Color.red);
        Debug.DrawRay(vector2, directionVector, Color.red);
        Debug.DrawRay(vectorMid, directionVector, Color.red);

        return (hit1, hit2, hitMid);
    }

    private Quaternion GetQuaternionDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.UP:
                return Quaternion.Euler(new Vector3(0, 0, 0));
            case Direction.DOWN:
                return Quaternion.Euler(new Vector3(0, 0, 180));
            case Direction.LEFT:
                return Quaternion.Euler(new Vector3(0, 0, 90));
            case Direction.RIGHT:
                return Quaternion.Euler(new Vector3(0, 0, -90));
            default:
                return Quaternion.Euler(new Vector3());
        }
    }

    private bool IsMovingInDirection(Direction direction)
    {
        return transform.rotation == GetQuaternionDirection(direction);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Data Point"))
        {
            Instantiate(dataPointCollect, collision.transform.position, transform.rotation);
            onDataPickup?.Invoke();
            Destroy(collision.gameObject);
        }
    }
}
