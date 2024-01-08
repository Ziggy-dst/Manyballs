using System;
using UnityEngine;
using System.Collections.Generic;

public class FollowPlayer : MonoBehaviour
{
    public Transform player1; // 玩家的Transform
    public Transform player2; // 玩家的Transform
    public float followSpeed = 5f; // 跟随速度
    public float minDistanceToFollow = 2f; // 开始跟随的最小距离
    public float cohesionStrength = 1f; // 凝聚力量
    public float alignmentStrength = 1f; // 对齐力量
    public float separationStrength = 1f; // 分离力量
    public float neighborRadius = 2f; // 邻居检测半径

    private Vector2 currentVelocity;

    public float stopDistance = 1f;

    private bool isFollowing = false;

    private SpriteRenderer _spriteRenderer;

    public float maxControlDistance;


    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float distanceToPlayer1 = Vector2.Distance(transform.position, player1.position);
        float distanceToPlayer2 = Vector2.Distance(transform.position, player2.position);

        if (distanceToPlayer1 > maxControlDistance & distanceToPlayer2 > maxControlDistance)
        {
            isFollowing = false;
            _spriteRenderer.color = Color.white;
            return;
        }

        float distanceToPlayer;
        Vector3 currentPlayerPos;
        if (distanceToPlayer1 < distanceToPlayer2)
        {
            distanceToPlayer = distanceToPlayer1;
            currentPlayerPos = player1.position;
            if (isFollowing) _spriteRenderer.color = Color.cyan;
        }
        else
        {
            distanceToPlayer = distanceToPlayer2;
            currentPlayerPos = player2.position;
            if (isFollowing) _spriteRenderer.color = Color.red;
        }


        if (distanceToPlayer < minDistanceToFollow || isFollowing)
        {
            isFollowing = true;
            if (distanceToPlayer > stopDistance) // 跟随玩家，但保持一定距离
            {
                // 实现跟随行为
                Vector2 followDirection = (currentPlayerPos - transform.position).normalized;
                currentVelocity = Vector2.Lerp(currentVelocity, followDirection * followSpeed, Time.deltaTime);

                // 实现flocking行为
                Vector2 cohesionVector = ComputeCohesionVector() * cohesionStrength;
                Vector2 alignmentVector = ComputeAlignmentVector() * alignmentStrength;
                Vector2 separationVector = ComputeSeparationVector() * separationStrength;

                Vector2 flockingDirection = cohesionVector + alignmentVector + separationVector;
                currentVelocity = Vector2.Lerp(currentVelocity, flockingDirection, Time.deltaTime);

                transform.position += (Vector3)currentVelocity * Time.deltaTime;
            }
            else
            {
                currentVelocity = Vector2.zero; // 在足够接近时停止移动
            }

            transform.position += (Vector3)currentVelocity * Time.deltaTime;
        }
    }

    Vector2 ComputeCohesionVector()
    {
        // 计算附近单位的平均位置，从而确定中心点
        Vector2 cohesion = Vector2.zero;
        int count = 0;
        foreach (var neighbor in FindNeighbors())
        {
            cohesion += (Vector2)neighbor.position;
            count++;
        }
        if (count > 0)
        {
            cohesion /= count;
            return (cohesion - (Vector2)transform.position).normalized;
        }
        return Vector2.zero;
    }

    Vector2 ComputeAlignmentVector()
    {
        // 计算附近单位的平均方向
        Vector2 alignment = Vector2.zero;
        int count = 0;
        foreach (var neighbor in FindNeighbors())
        {
            alignment += neighbor.GetComponent<Rigidbody2D>().velocity;
            count++;
        }
        if (count > 0)
        {
            alignment /= count;
            return alignment.normalized;
        }
        return transform.up;
    }

    Vector2 ComputeSeparationVector()
    {
        // 计算以避免与附近单位过于接近的方向
        Vector2 separation = Vector2.zero;
        foreach (var neighbor in FindNeighbors())
        {
            Vector2 oppositeDirection = transform.position - neighbor.position;
            separation += oppositeDirection.normalized / oppositeDirection.magnitude;
        }
        return separation;
    }

    List<Transform> FindNeighbors()
    {
        List<Transform> neighbors = new List<Transform>();
        Collider2D[] neighborColliders = Physics2D.OverlapCircleAll(transform.position, neighborRadius, LayerMask.GetMask("Follower"));
        foreach (var collider in neighborColliders)
        {
            if (collider.transform != transform) // 排除自身
            {
                neighbors.Add(collider.transform);
            }
        }
        return neighbors;
    }
}