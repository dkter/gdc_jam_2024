using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public GameObject enemyParent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3? nearestEnemy = null;
        foreach (var enemy in enemyParent.GetComponentsInChildren<Enemy>())
        {
            if (nearestEnemy == null || Vector3.Distance(transform.position, enemy.transform.position) < Vector3.Distance(transform.position, nearestEnemy ?? Vector3.zero) )
            {
                nearestEnemy = enemy.transform.position;
            }
        }
        if (nearestEnemy != null)
        {
            var lookTo = new Vector3(
                nearestEnemy?.x ?? 0f,
                nearestEnemy?.y ?? 0f,
                transform.position.z
            );
            var targetRotation = Quaternion.FromToRotation(transform.position, lookTo);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 5f);
        }
    }
}
