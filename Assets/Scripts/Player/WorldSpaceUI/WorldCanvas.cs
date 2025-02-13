using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class WorldCanvas : MonoBehaviour
{
    public Transform player;

    public GameObject popupPrefab;

    public Text ammodisplay;
    float transparency;

    private void Update()
    {

        if (transparency > 0f)
        {
            transparency -= Time.deltaTime;
        }
        ammodisplay.color = new Color(
            ammodisplay.color.r,
            ammodisplay.color.g,
            ammodisplay.color.b,
            transparency);

        if (player == null)
            Debug.LogError(gameObject.name + ": Hey i have no player to follow!");
    }

    private void FixedUpdate()
    {
        transform.position = player.position;
    }

    public void UpdateAmmo(int value)
    {
        ammodisplay.text = value.ToString();
        transparency = 1f;
    }

    public void popup(string message)
    {
        GameObject popup = Instantiate(popupPrefab, transform, false);
        //gives an offset to the popup
        popup.transform.position += new Vector3(1.1f, 0.8f,0f);
        popup.GetComponent<Popup>().message = message;
    }
}
