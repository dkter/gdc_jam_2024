using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Vector3 target;

    private float speed = 3f;
    private float lifetime = 15f;
    public void UpdateTarget()
    {
        Vector3 playerPos = GameManager.I.player.transform.position;
        target = new Vector3(playerPos.x, playerPos.y, -1);
    }

    private void Update()
    {
        Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.velocity = speed * (target - transform.position).normalized;
        // Vector3 nextPosition = transform.position + Time.deltaTime * speed * (target - transform.position).normalized;
        Vector3 nextPosition = transform.position;
        nextPosition.z = -1 + (0.01f * transform.position.y); // prevent depth/overlap problems using the y-coordinate to determine z-position 
        transform.position = nextPosition;

        lifetime -= Time.deltaTime;
        if (lifetime < 0)
        {
            Destroy(gameObject);
        }
    }

    public void Bomb(Vector3 bombPosition)
    {
        float distanceThreshold = 3;
        float distance = Vector3.Distance(this.transform.position, bombPosition);
        if (distance < distanceThreshold)
        {
            GameManager.I.score += 1;
            Destroy(gameObject);
        }
    }
}
