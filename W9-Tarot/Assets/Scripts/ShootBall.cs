using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootBall : MonoBehaviour
{
    public float launchForce = 10f;

    private SpriteRenderer _spriteRenderer;

    private Rigidbody2D ballRb;

    public AudioSource hitSound;

    private bool canPlaySound = true;
    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
         ballRb = GetComponent<Rigidbody2D>();
        PlayerController.OnShoot += Shoot;
    }

    private void OnDestroy()
    {
        PlayerController.OnShoot -= Shoot;
    }

    private void Shoot(Vector3 dir, PlayerController.Player player)
    {
        if (_spriteRenderer.color.Equals(Color.cyan) & player.Equals(PlayerController.Player.Left))
        {
            // print("left ball shoot");
            // print(dir);
            ballRb.AddForce(dir * launchForce, ForceMode2D.Impulse); // 给球体施加力
        }

        if (_spriteRenderer.color.Equals(Color.red) & player.Equals(PlayerController.Player.Right))
        {
            // print("right ball shoot");
            // print(dir);
            ballRb.AddForce(dir * launchForce, ForceMode2D.Impulse); // 给球体施加力
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag.Equals("LeftGate") )
        {
            GameManager.Instance.AddScore(PlayerController.Player.Right);
            Destroy(gameObject);
        }
        else if (col.tag.Equals("RightGate"))
        {
            GameManager.Instance.AddScore(PlayerController.Player.Left);
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag.Equals("Wall") & canPlaySound) StartCoroutine(PlaySound());
    }

    IEnumerator PlaySound()
    {
        hitSound.Play();
        canPlaySound = false;
        yield return new WaitForSeconds(2f); // 等待两秒
        canPlaySound = true;
    }
}
