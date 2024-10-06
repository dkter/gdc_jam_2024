using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public GameObject enemyParent;
    public GameObject explosionPrefab;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(BombTimer());
    }

    private IEnumerator BombTimer()
    {
        yield return new WaitForSeconds(2);
        enemyParent.BroadcastMessage("Bomb", this.transform.position);
        GameObject newExplosion = Instantiate(explosionPrefab);
        newExplosion.transform.position = new Vector3(
            transform.position.x,
            transform.position.y,
            -20
        );
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
