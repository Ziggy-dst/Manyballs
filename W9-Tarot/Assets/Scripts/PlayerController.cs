using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Slider = UnityEngine.UIElements.Slider;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5.0f;

    private Rigidbody2D rb;
    private Vector2 moveDirection;

    private Vector2 shootDirection;

    public Player currentPlayer;

    public static Action<Vector3, Player> OnShoot;
    public AudioSource kickSound;
    public AudioSource errorSound;
    public AudioSource dashSound;

    // shoot
    private float timeSinceLastPlay = 0f;
    public float playInterval = 1f;

    // dash
    private float timeSinceLastDash = 0f;
    public float dashInterval = 1f;
    public float dashSpeed = 15f;
    private float currentDashTime = 0;

    public SpriteRenderer dashSign;
    // private Color noDashColor;

    public Slider cdSlider;

    public enum Player
    {
        Right,
        Left,
        None
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        dashSign.color = new Color(dashSign.color.r, dashSign.color.g, dashSign.color.b, 0);
        // noDashColor = new Color(1, 1, 1, 0);
    }

    void Update()
    {
        ProcessInputs();

        if (timeSinceLastPlay < playInterval)
        {
            timeSinceLastPlay += Time.deltaTime; // 更新计时器
        }
        Shoot();

        if (timeSinceLastDash < dashInterval)
        {
            timeSinceLastDash += Time.deltaTime; // 更新计时器
            float newAlpha = Mathf.Lerp(0, 1, timeSinceLastDash / dashInterval);
            dashSign.color = new Color(dashSign.color.r, dashSign.color.g, dashSign.color.b, newAlpha);
        }
        Dash();

    }

    void FixedUpdate()
    {
        Move();
    }

    private void Dash()
    {
        if (currentPlayer.Equals(Player.Left))
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                if (timeSinceLastDash < dashInterval)
                {
                    errorSound.Play();
                    return;
                }
                DashAction();
                timeSinceLastDash = 0f;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                if (timeSinceLastDash < dashInterval)
                {
                    errorSound.Play();
                    return;
                }
                DashAction();
                timeSinceLastDash = 0f;
            }
        }
    }

    void DashAction()
    {
        print("dash");
        dashSound.Play();
        rb.AddForce(moveDirection * dashSpeed);
        dashSign.color = new Color(dashSign.color.r, dashSign.color.g, dashSign.color.b, 0);
    }



    private void Shoot()
    {
        if (currentPlayer.Equals(Player.Left))
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (timeSinceLastPlay < playInterval)
                {
                    errorSound.Play();
                    return;
                }
                kickSound.Play();
                OnShoot(shootDirection, currentPlayer);
                timeSinceLastPlay = 0f;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (timeSinceLastPlay < playInterval)
                {
                    errorSound.Play();
                    return;
                }
                kickSound.Play();
                OnShoot(shootDirection, currentPlayer);
                timeSinceLastPlay = 0f;
            }
        }
    }

    void ProcessInputs()
    {
        float moveX;
        float moveY;
        if (currentPlayer.Equals(Player.Left))
        {
            moveX = Input.GetAxisRaw("Horizontal");
            moveY = Input.GetAxisRaw("Vertical");
        }
        else
        {
            moveX = Input.GetAxisRaw("Debug Horizontal");
            moveY = Input.GetAxisRaw("Debug Vertical");
        }
        moveDirection = new Vector2(moveX, moveY).normalized;
        if (moveDirection != Vector2.zero) shootDirection = moveDirection;
    }

    void Move()
    {
        rb.velocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);

        if (moveDirection != Vector2.zero)
        {
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 30)); // 减90度，如果是朝向上方
        }
    }
}