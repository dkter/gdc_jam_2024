using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public GameObject enemyParent;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(BombTimer());
    }

    private IEnumerator BombTimer()
    {
        yield return new WaitForSeconds(2);
        enemyParent.BroadcastMessage("Bomb", this.transform.position);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
