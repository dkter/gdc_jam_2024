using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SummonState
{
    Success,
    Failed
}

public class Mana
{
    float mana;
    float max;
    public SummonState ConsumeMana(float amount)
    {
        Debug.Log("consuming mana " + mana + " minus " + amount);

        if (amount > mana)
            return SummonState.Failed;

        else
        {
            mana -= amount;
            GameManager.I.manaSlider.SetValueWithoutNotify(mana / max);
            return SummonState.Success;
        }
    }
    public void SetMax(float amount)
    {
        max = amount;
        GameManager.I.manaSlider.SetValueWithoutNotify(mana / max);
    }
    public void SetMana(float amount)
    {
        mana = amount;
        GameManager.I.manaSlider.SetValueWithoutNotify(mana / max);
    }

}

public class Player : MonoBehaviour
{
    private Vector3 direction;
    public float moveSpeed = 1f;
    public Mana _mana;


    // Update is called once per frame
    void Update()
    {
        DoMovement();

        
    }

    void DoMovement()
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

        if (transform.position.x < -30) direction.x = Mathf.Clamp(direction.x, 0, 1);
        if (transform.position.x > 30) direction.x = Mathf.Clamp(direction.x, -1, 0);
        if (transform.position.y < -30) direction.y = Mathf.Clamp(direction.y, 0, 1);
        if (transform.position.y > 30) direction.y = Mathf.Clamp(direction.y, -1, 0);

        transform.position += Time.deltaTime * moveSpeed * direction.normalized;
    }
}
