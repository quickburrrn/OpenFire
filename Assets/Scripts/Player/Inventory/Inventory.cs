using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public GameObject inventory;

    public List<bool> isFull;
    public List<GameObject> slots;
    public GameObject slotSelector;

    [HideInInspector]
    public int selectedSlot;

    Shoot weaponSystem;

    float slotTransparency;
    float selectedColorTransparency;
    float transparency;

    private void Start()
    {
        if (inventory == null)
        {
            Debug.LogError(transform.name + ": inventory script has no inventory gameobject attached to it");
        }

        //needs to assigns slots on start

        for (int i = 0; i < slots.Count; i++)
        {
            slots[i] = inventory.transform.GetChild(i).gameObject;
        }

        slotSelector = inventory.transform.GetChild(slots.Count).gameObject;

        weaponSystem = GetComponent<Shoot>();

        //sets the correct transparency
        slotTransparency = slots[0].GetComponent<Image>().color.a;
        selectedColorTransparency = slotSelector.GetComponent<Image>().color.a;
    }

    void Additem(GameObject item)
    {
        for (int i = 0; i < slots.Count; i++) { 
            if (!isFull[i])
            {
                isFull[i] = true;
                Instantiate(item, slots[i].transform, false);
                ChangeSelectedSlot(i);
                weaponSystem.ChangeWeapon(slots[i].GetComponentInChildren<weapon>());
                break;
            }
        }
    }

    public void ChangeSelectedSlot(int slot)
    {
        transparency = 1f;
        //checks if the given slot is in range of the slot amount
        //if not loop the inventory
        if (slot > slots.Count-1)
        {
            slot = 0;
        }else if (slot < 0)
        {
            slot = slots.Count - 1 ;
        }

        slotSelector.transform.position = slots[slot].transform.position;
        weaponSystem.ChangeWeapon(slots[slot].GetComponentInChildren<weapon>());
        selectedSlot = slot;
    }

    private void Update()
    {
        //all of this is subject to change

        //sets the transparency;
        if (transparency > 0)
        {
            transparency -= Time.deltaTime/1;

            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].GetComponent<Image>().color = new Color(0.7f, 0.7f, 0.7f, 1f);
                if (slots[i].transform.childCount > 0)
                {
                    slots[i].transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                }
            }
            slotSelector.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);

            //for (int i = 0;i < slots.Count;i++)
            //{
            //    slots[i].GetComponent<Image>().color = new Color(1f, 1f, 1f, Mathf.Clamp(transparency, 0f, slotTransparency));
            //    if (slots[i].transform.childCount > 0)
            //    {
            //        Debug.Log(slots[i].transform.GetChild(0));
            //        slots[i].transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, Mathf.Clamp(transparency, 0f, slotTransparency));
            //    }

            //}
            //slotSelector.GetComponent<Image>().color = new Color(1f, 1f, 1f, Mathf.Clamp(transparency, 0f, selectedColorTransparency));
        }
        else
        {
            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
                if (slots[i].transform.childCount > 0)
                {
                    slots[i].transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
                }
            }
            slotSelector.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "item")
        {
            Additem(collision.GetComponent<Item>().itembutton);
            Destroy(collision.gameObject);
        }
    }
}
