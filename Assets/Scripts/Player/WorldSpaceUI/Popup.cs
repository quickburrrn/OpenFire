using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class Popup : MonoBehaviour
{
    public string message;

    Text text;
    float transparency;


    private void Start()
    {
        transparency = 1.0f;
        text = GetComponent<Text>();
        text.text = message;
    }

    private void Update()
    {
        transparency -= Time.deltaTime;
        text.color = new Color(text.color.r, text.color.g, text.color.b, transparency);
        transform.position = transform.position  + new Vector3(0f,Time.deltaTime,0f);

        if (transparency < 0)
        {
            Destroy(gameObject);
        }
    }
}
