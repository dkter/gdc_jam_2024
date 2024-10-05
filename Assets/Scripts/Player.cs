using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Vector3 direction;
    public float moveSpeed = 1f;

    // Update is called once per frame
    void Update()
    {
        direction = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.A))
            direction.x -= 1;
        if (Input.GetKey(KeyCode.D))
            direction.x += 1;
        if (Input.GetKey(KeyCode.W))
            direction.y += 1;
        if (Input.GetKey(KeyCode.S))
            direction.y -= 1;

        transform.position += Time.deltaTime * moveSpeed * direction.normalized;
    }
}
