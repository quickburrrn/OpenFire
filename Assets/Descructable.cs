using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Descructable : MonoBehaviour
{
    public AudioClip destructionClip;
    public GameObject destructionParticles;
    public float health;

    void Demolish()
    {
        if (destructionClip != null)
        {
            Gamemanager.AudioBox(destructionClip, 1f, Vector3.zero, Quaternion.identity);
        }

        if (destructionParticles != null)
        {
            GameObject particle = Instantiate(destructionParticles, transform.position, transform.rotation);
            Destroy(particle, 1f);
        }
        else
        {
            Debug.Log("no destruction Particle for " + name);
        }
        Destroy(gameObject);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0f)   
        {
            Demolish();
        }
    }
}
