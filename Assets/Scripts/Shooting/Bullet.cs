using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Bullet : MonoBehaviour
{
    //public field
    [Tooltip("the raget of the bullet")]
    public string TargetTag = "enemy";
    [Header("stats")]
    public float speed = 10f;
    public float damage = 5f;
    public float lifetime = 4f;
    [Header("visual")]
    [Tooltip("particle when bullet hit something solid for instance walls")]
    public GameObject SolidImpactParticle;
    [Header("audio")]
    [Tooltip("impactsound")] public AudioClip ImpactClip;
    [Tooltip("volume of the impact")] public float volume = .2f;
    
    //private fields
    Rigidbody2D rb;
    bool hasSolidParticle = false;

    private void Start()
    {
        if (SolidImpactParticle != null)
            hasSolidParticle = true;
        StartCoroutine(Timer());
    }

    private void Update()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.up * speed;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "wall" || collision.tag ==  "door")
        {
            //spawns the solid Impact particle because bullet hit something solid
            if (hasSolidParticle)
            {
                GameObject impact = Instantiate(SolidImpactParticle, transform.position, Quaternion.identity);
                Gamemanager.AudioBox(ImpactClip, volume, Vector3.zero, Quaternion.identity);
                Destroy(impact, 2f);
            }
            Destroy(gameObject);
            
        }else if (collision.tag == "enemy" && TargetTag == "enemy")
        {
            collision.GetComponent<AIManager>().TakeDamage(damage);
            Destroy(gameObject);
        }else if (collision.tag == "descructable")
        {
            if (hasSolidParticle)
            {
                GameObject impact = Instantiate(SolidImpactParticle, transform.position, Quaternion.identity);
                Gamemanager.AudioBox(ImpactClip, volume, Vector3.zero, Quaternion.identity);
                Destroy(impact, 2f);
            }
            collision.GetComponent<Descructable>().TakeDamage(damage);
            Destroy(gameObject);
        }else if (collision.tag == "Player" && TargetTag == "Player")
        {
            collision.GetComponent<Interaction>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }
}
