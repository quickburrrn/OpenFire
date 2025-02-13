using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class IconPopup : MonoBehaviour
{
    public Sprite sprite;

    SpriteRenderer spriteRenderer;
    float transparency;

    // Start is called before the first frame update
    void Start()
    {
        transparency = 1.0f;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
    }

    // Update is called once per frame
    void Update()
    {
        transparency -= Time.deltaTime;
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, transparency);
        transform.position = transform.position + new Vector3(0f, Time.deltaTime, 0f);

        if (transparency < 0)
        {
            Destroy(gameObject);
        }
    }
}
