using Pathfinding;
using UnityEngine;
using System.Collections;
public class AIManager : MonoBehaviour
{
    //public field
    [Header("graphics")]
    public GameObject stunnedIcon;
    public GameObject exclamationmark;
    public GameObject bloodParticle;
    [Header("stats")]
    public float Health;
    public float detectionAngle;
    public float detectionRadius;
    [Header("audio")]
    [Tooltip("audio clip when spottet player")] public AudioClip spotPlayerClip;
    [Space]
    public GameObject player;
    public GameObject player2;
    public float updateDelay = 0.1f;
    [Tooltip("mask for raycast")]public LayerMask raycastMask;
    [Tooltip("mask for Collition")]public LayerMask collisonDetectionMask;

    public Transform gunPoint;
    [Header("special enemies")]
    [Space]
    [Header("vip")]
    [Tooltip("this will freeze the enemy")] public bool vip;
    [Header("boss")]
    public bool boss;
    public Healthbar healthBar;
    [Tooltip("item that will spawned once the boss have been killed")]public GameObject DeathSpawnItem;
    [Header("bomber")]
    [Tooltip("this will make the enemy explode")] public bool bomber;
    public GameObject explodeParticle;
    [Tooltip("particles that will spwn before he explodes")]public GameObject sparkParticle;
    [Tooltip("AudioSource for the fuse sfx")]public AudioSource SparkSource;
    public AudioClip ExplodeClip;
    public float explotionRadius = 2.5f;
    public float explotionDamage = 80f;

    //private field
    AIDestinationSetter target;
    weapon Weapon;

    private bool Stunned;

    bool InlineP2;
    bool Inline;
    bool Inrange;

    bool reloading = false;
    bool readyToShoot;
    bool firing = false;

    private void Start()
    {
        Weapon = GetComponent<weapon>();
        target = GetComponent<AIDestinationSetter>();
        if (!vip)
            target.enabled = false;
        StartCoroutine(update());
        readyToShoot = true;
        Stunned = false;

        //sets the healthbar values if its a boss
        if (boss)
        {
            StartCoroutine(LateStart());
        }
    }

    IEnumerator LateStart()
    {
        yield return new WaitForEndOfFrame();
        healthBar.SmugleValue(Health, Health);
    }

    private void Update()
    {
        if (Health <= 0f)
        {
            //if bomber then explode rather than die
            if (bomber != true)
            {
                Die();
            }
            else
            {
                Explode();
            }

        }

        //shoots if the ai has been activated and in line of sight
        if (!vip)
        {
            if (!bomber)
            {
                if (target.enabled && Inline && readyToShoot && !reloading || target.enabled && InlineP2 && readyToShoot && !reloading)
                {
                    if (Weapon.ammoInMag != 0)
                    {
                        if (Weapon.automatick && !firing)
                        {
                            InvokeRepeating("Shoot", 0f, 1f / Weapon.Firerate);
                        }
                        else
                        {
                            Shoot();
                        }
                        StartCoroutine(Cooldown());
                    }
                    else
                    {
                        StartCoroutine(Reload());
                    }

                }
                else
                {
                    CancelInvoke("Shoot");
                    firing = false;
                }
            }   
        }
    } 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!vip)
        {
            target.target = player.transform;
            target.enabled = true;
            //activetes the explode timer once
            if (bomber && target.enabled)
                StartCoroutine(ExplodeTimer());
        }

        if (exclamationmark != null)
        {
            if (target == null)
            {
                Instantiate(exclamationmark, transform.position, Quaternion.identity);
            }
        }
        else
        {
            Debug.LogWarning("need to assign exclamationmark prefab");
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Color r = new Color(1f,0f,0f,0.5f);
        Gizmos.color = r;

        Color g = new Color(0f, 1f, 0f, 0.5f);
        if (Inrange)
        {
            Gizmos.color = g;
        }
        Gizmos.DrawWireSphere(transform.position + transform.up*4, detectionRadius);
        
        Gizmos.color=r;

        if (player != null)
        {
            if (Inline)
            {
                Gizmos.color = g;
            }
            Gizmos.DrawLine(transform.position, player.transform.position);
        }

        Gizmos.color = r;

        if (player2 != null && player2.activeSelf)
        {
            Debug.Log("Drawing gozmos for " + player2);
            if (InlineP2)
            {
                Gizmos.color = g;
            }
            Gizmos.DrawLine(transform.position, player2.transform.position);
        }

        if (bomber)
        {
            //Debugs the explotion radius
            Gizmos.color = new Color(1f, 0.5f, 0f, 1f);
            Gizmos.DrawWireSphere(transform.position, explotionRadius);
        }
    }

    public void TakeDamage(float damage)
    {
        Health -= damage;
        GameObject particle = Instantiate(bloodParticle, transform.position, transform.rotation);
        particle.transform.GetComponent<ParticleSystem>().Play();
        Destroy(particle, 5f);

        if (boss)
        {
            healthBar.ChangeValue(Health);
        }
    }

    public void CheckForTarget()
    {
        //shoots a ray at player1
        RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, player.transform.position-transform.position, Mathf.Infinity, raycastMask);
        if (raycastHit)
        {
            if (raycastHit.collider.CompareTag("Player"))
            {
                Inline = true;
            }
            else
            {
                Inline = false;
            }
        }

        //shoot a ray at player2
        if (player2 != null && player2.activeSelf )
        {
            RaycastHit2D raycastHit2 = Physics2D.Raycast(transform.position, player2.transform.position - transform.position, Mathf.Infinity, raycastMask);
            if (raycastHit2)
            {
                if (raycastHit2.collider.CompareTag("Player"))
                {
                    InlineP2 = true;
                }
                else
                {
                    InlineP2 = false;
                }
            }
        }
        
        Collider2D collider = Physics2D.OverlapCircle((Vector2)transform.position + (Vector2)transform.up * 4, detectionRadius, collisonDetectionMask);
        if (collider)
        {
            if (collider.CompareTag("Player")){
                Inrange = true;
            }
            else
            {
                Inrange = false;
            }
        }

        if (Inline && Inrange) {
            if (exclamationmark != null)
            {
                //this is called when the ai spottet the player
                //need to activate cooldown here
                if (target.enabled != true)
                {
                    //this is the reaction time thing
                    if (Weapon != null)
                        StartCoroutine(Cooldown());

                    //plays stop audio
                    if (spotPlayerClip != null)
                    {
                        Gamemanager.AudioBox(spotPlayerClip, 1, Vector3.zero, Quaternion.identity);
                    }

                    Instantiate(exclamationmark, transform.position, Quaternion.identity);
                    
                    //explode itself if bomber
                    if (bomber)
                    {
                        StartCoroutine(ExplodeTimer());
                    }
                }
            }
            else
            {
                Debug.LogWarning("need to assign exclamationmark prefab");
            }
            
            if (!vip)
            {
                if (GetComponent<AIDestinationSetter>())
                {
                    target.target = player.transform;
                    target.enabled = true;
                }
                else
                {
                    Debug.LogWarning("i dont have a AIDestinationSetter give me one or assign me as a vip");
                }

            }

        }else if (InlineP2 && Inrange)
        {
            if (exclamationmark != null)
            {
                if (target.enabled != true)
                {
                    //this is called when the ai spottet the player 2
                    if (Weapon != null)
                        StartCoroutine(Cooldown());

                    Instantiate(exclamationmark, transform.position, Quaternion.identity);

                    if (bomber)
                    {
                        StartCoroutine(ExplodeTimer());
                    }
                }
            }
            else
            {
                Debug.LogWarning("need to assign exclamationmark prefab");
            }
            StartCoroutine(ExplodeTimer());
            target.target = player2.transform;
            target.enabled = true;
        }
    }

    public void Shoot()
    {
        if (Weapon != null)
        {
            //spawns audio 
            if (Weapon.AutomaticSource != null)
            {
                Weapon.AutomaticSource.volume = Weapon.ShootVolume;
                Weapon.AutomaticSource.Play();
            }
               

            if (Weapon.Shoot != null)
                Gamemanager.AudioBox(Weapon.Shoot, Weapon.ShootVolume, Vector3.zero, Quaternion.identity);

            Instantiate(Weapon.muzzleFlash, gunPoint.transform);
            if (Weapon.burst)
            {
                StartCoroutine(Burstfire());
                return;
            }
            Weapon.ammoInMag -= 1;
            Bullet bullet = Instantiate(Weapon.bullet, gunPoint.position, gunPoint.rotation).GetComponent<Bullet>();
            bullet.gameObject.transform.Rotate(transform.forward * Random.Range(-Weapon.spred, Weapon.spred), Space.Self);
            bullet.TargetTag = "Player";
        }
    }

    //kills the enemy
    public void Die()
    {
        //spawns blod
        GameObject particle = Instantiate(bloodParticle, transform.position, transform.rotation);
        particle.transform.GetComponent<ParticleSystem>().Play();
        Destroy(particle, 5f);
        //Checks if there is enemies left in the scene
        EnemiesManager.instance.RemoveEnemy(gameObject);

        //exists boss mode
        if (boss)
        {
            BossManger.execBoss();
            if (DeathSpawnItem != null)
            {
                Instantiate(DeathSpawnItem, transform.position, Quaternion.identity);
            }
        }

        Destroy(gameObject);
    }

    //explodes the enemy
    public void Explode()
    {
        GameObject particle = Instantiate(bloodParticle, transform.position, transform.rotation);
        particle.transform.GetComponent<ParticleSystem>().Play();
        Destroy(particle, 5f);
        GameObject exploparticle = Instantiate(explodeParticle, transform.position, Quaternion.identity);
        Destroy(exploparticle, 5f);

        //plays the audio
        Gamemanager.AudioBox(ExplodeClip, 1f, Vector3.zero, Quaternion.identity);

        //calculate the damage
        Collider2D collider = Physics2D.OverlapCircle(transform.position, explotionRadius, collisonDetectionMask);
        if (collider)
        {
            float distance = Vector2.Distance(transform.position, collider.transform.position);
            //calculate the damage based on the distance
            if (distance < explotionRadius)
            {
                float damagavalue = Mathf.InverseLerp(explotionRadius, 0,distance);
                damagavalue += 0.4f;
                collider.GetComponent<Interaction>().TakeDamage(explotionDamage * damagavalue);
                Debug.Log(damagavalue);
                Debug.Log(transform.name + "delth" + damagavalue*explotionDamage);
            }
        }
        EnemiesManager.instance.RemoveEnemy(gameObject);
        Destroy(gameObject);
    }

    //this is for calling stun from another script
    public void StartStun(float _lenght)
    {
        //spawns the stun icon and starts the sun coroutine
        StartCoroutine(Stun(_lenght));
    }

    IEnumerator update()
    {
        if (player != null && !Stunned)
        {
            CheckForTarget();           
        }
        yield return new WaitForSeconds(updateDelay);
        StartCoroutine(update());
    }

    IEnumerator Stun(float lenght)
    {

        GameObject stun = Instantiate(stunnedIcon, transform.position + new Vector3(0f,0.45f,0f), Quaternion.identity);
        Stunned = true;
        Destroy(stun, lenght);
        yield return new WaitForSeconds(lenght);
        Stunned = false;
        
    }
    //cooldown for shooting
    IEnumerator Cooldown()
    {
        readyToShoot = false;
        yield return new WaitForSeconds(Weapon.cooldown);
        readyToShoot = true;
    }

    IEnumerator Reload()
    {
        reloading = true;
        yield return new WaitForSeconds(Weapon.reloadTime);
        Weapon.ammoInMag = Weapon.magsize;
        reloading = false;
    }

    IEnumerator Burstfire()
    {
        for (int i = 0; i < Weapon.bulletAmount; i++)
        {
            if (Weapon.ammoInMag > 0)
            {
                Weapon.ammoInMag -= 1;
                GameObject _bullet = Instantiate(Weapon.bullet, gunPoint.position, gunPoint.rotation);
                _bullet.transform.Rotate(transform.forward * Random.Range(-Weapon.spred, Weapon.spred), Space.Self);
                _bullet.GetComponent<Bullet>().TargetTag = "Player";
                yield return new WaitForSeconds(Weapon.bulletDelay);
            }
        }
    }

    IEnumerator ExplodeTimer()
    {
        SparkSource.Play();
        Instantiate(sparkParticle, transform);
        yield return new WaitForSeconds(2f);
        //spawns the particle
        GameObject exploparticle = Instantiate(explodeParticle, transform.position, Quaternion.identity);
        Destroy(exploparticle, 5f);
        Explode();
    }
}
