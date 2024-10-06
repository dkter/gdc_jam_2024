using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ManaSpawner : MonoBehaviour
{
    public float timer = 25f;
    public GameObject manaChild;

    private void LateUpdate()
    {
        if (timer >= 0)
        {
            timer -= Time.deltaTime;
            manaChild.SetActive(false);
        }
        else if (!manaChild.activeSelf)
        {
            manaChild.SetActive (true);
        }
        else
        {
            // fuck ass collision detect
            Vector3 p = GameManager.I.player.gameObject.transform.position;
            Vector2 flat_p = new Vector2(p.x, p.y);
            Vector2 t = new Vector2(transform.position.x, transform.position.y);
            if (Vector2.Distance(p, t) < 1)
            {
                timer = 25f;
                GameManager.I.ManaPickup();
            }
        } 
    }
}
