using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject target;

    private void Awake()
    {
    }

    public float speed = 1f;

    private void Update()
    {
        if (target == null)
        {
            target = GameManager.I.player;
            return;
        }

        transform.position = new Vector3(target.transform.position.x, target.transform.position.y, -50); // prevent depth/overlap problems using the y-coordinate to determine z-position 
    }
}
