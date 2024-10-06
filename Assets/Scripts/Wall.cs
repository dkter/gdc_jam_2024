using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Wall : MonoBehaviour
{
    
    public Slider healthbar;
    public float health = 15f;

    // Update is called once per frame
    void Update()
    {
        if (health > 0)
        {
            health -= Time.deltaTime;
            healthbar.SetValueWithoutNotify(health / 15f);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
