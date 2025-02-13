using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HingeJoint2D))]
[RequireComponent (typeof(Rigidbody2D))]
public class Door : MonoBehaviour
{
    public GameObject destructionParticles;
    [Header("Remember trigger collider")]
    [Header("Recomended Rigidbody settings")]
    [Header("Mass: 100")]
    [Header("Linear Drag: 2")]
    [Header("Angluar Drag: 4")]
    [Header("Gravity Scale: 0")]
    public HingeJoint2D joint;
    public AudioClip doorSmashClip;
    public AudioClip doorOpenClip;
    JointAngleLimits2D _limit = new JointAngleLimits2D();


    public float stunradius = 5f;
    public float stunduration = 1f;

    private void Start()
    {
        joint = GetComponent<HingeJoint2D>();
        joint.useLimits = true;
        _limit.max = 0f;
        _limit.min = 0f;
        joint.limits = _limit;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stunradius);
    }

    public void Open()
    {
        _limit.max = 100f;
        _limit.min = -100f;
        joint.limits = _limit;

        //plays open sound
        if (doorOpenClip != null)
            Gamemanager.AudioBox(doorOpenClip,1f,Vector3.zero,Quaternion.identity);
    }

    public void Demolish(Transform impact)
    {
        //instantiates the particles
        //Quaternion Rotation;
        //if (AngleDir(transform.position, impact.position) < 0)
        //{
        //    Rotation = Quaternion.EulerRotation(0, 0, -90);
        //    Debug.Log("Right");
        //}
        //else
        //{
        //    Debug.Log("Left");

        //}

        //plays the destroy audio
        if (doorSmashClip != null)
        {
            Gamemanager.AudioBox(doorSmashClip,1f, transform.position, Quaternion.identity);
        }
        else{ Debug.LogWarning(transform.name + "has no audo clip"); }

        GameObject particles = Instantiate(destructionParticles, transform.position, transform.rotation);

        //-1 because i don't know
        float angle = (AngleDir(transform.position, impact.position));
        if (angle < 0) 
        {
            if (transform.rotation.eulerAngles.z >= 180)
            {
                Debug.Log("Upright");
            }
            else
            {
                Debug.Log("Downright");
            }
        }
        else
        {
            //particles.transform.Rotate(0, 0, -180);
            if (transform.rotation.eulerAngles.z >= 180)
            {
                Debug.Log("Upleft");
                
            }
            else
            {
                Debug.Log("Downleft");
            }
        }

        //stunnes the targeted enemies
        List<GameObject> target = GetEffectedEnemies();
        for (int i = 0; i < target.Count; i++)
        {
            Debug.Log(target[i].name);
            target[i].GetComponent<AIManager>().StartStun(stunduration);
        }

        //destroy the particles and gameobject
        Destroy(particles, 5f);
        Destroy(gameObject);
    }

    //gets the enemies that are inside the door destruction radius
    List<GameObject> GetEffectedEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");
        List<GameObject> targets = new List<GameObject>();
        for (int i = 0; i < enemies.Length; i++)
        {
            if (Vector2.Distance(enemies[i].transform.position, transform.position) < stunradius)
            {
                targets.Add(enemies[i]);
            }
        }

        return targets;
    }

    //check what side of the object the player is on
    public float AngleDir(Vector2 A, Vector2 B)
    {
        return -A.x * B.y + A.y * B.x;
    }

}
