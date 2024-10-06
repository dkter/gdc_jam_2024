using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SplashText : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float lifetime = 1f;
    public Color color;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void SetData(string message, Color c)
    {
        text.text = message;
        color = c;
    }

    void Update()
    {
        if(lifetime> 0)
        {
            lifetime -= Time.deltaTime;
            text.color = Color.Lerp(Color.white, color, Mathf.Cos(lifetime*6.28f) + 0.5f);
            text.fontSize = 50 + 5 * Mathf.Cos(lifetime * 6.28f);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
