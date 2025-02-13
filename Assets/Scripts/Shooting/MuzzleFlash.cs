using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    public float livetime = 0.1f;
    float maxtime;

    public UnityEngine.Rendering.Universal.Light2D[] lights;
    bool useLight;

    void Start()
    {
        maxtime = livetime;
        if (lights.Length < 0)
        {
            if (lights[0] != null)
            {
                useLight = true;
            }
            else
            {
                Debug.LogWarning(name+"Lights array has empty values mby you should check on this");
            }
        }
    }

    private void Update()
    {
        livetime -= Time.deltaTime;
        if (useLight )
        {
            for (int i = 0; i < lights.Length; i++)
            {
                lights[i].intensity = Mathf.InverseLerp(0, maxtime, livetime);
            }
        }
        if (livetime < 0)
        {
            Destroy(gameObject);
        }
    }

}
