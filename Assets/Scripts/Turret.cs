using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public GameObject enemyParent;
    public GameObject bulletPrefab;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ShootTimer());
    }

    private IEnumerator ShootTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(2);
            GameObject newBullet = Instantiate(bulletPrefab);
            newBullet.transform.position = this.transform.position + this.transform.rotation * new Vector3(0.5f, 0f, 0f);
            newBullet.GetComponent<Bullet>().velocity = this.transform.rotation * Vector3.right * 4f;
        }
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
