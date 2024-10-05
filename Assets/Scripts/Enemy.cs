using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float speed = 1f;
    private void Update()
    {
        Vector3 nextPosition = transform.position + Time.deltaTime * speed * -transform.position.normalized;
        nextPosition.z = -1 + (0.1f * transform.position.y); // prevent depth/overlap problems using the y-coordinate to determine z-position 
        transform.position = nextPosition;
    }
}
