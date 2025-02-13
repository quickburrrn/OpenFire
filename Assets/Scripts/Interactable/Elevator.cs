using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Elevator : MonoBehaviour
{
    public static Elevator instance;

    
    public LayerMask mask;
    public Animator animator;
    public AudioClip bingClip;
    public Vector2 size = new Vector2(1f,1f);
    public Vector2 offset = Vector2.zero;
    
    //need to fix the objective system

    public bool openOnStart;
    public bool vipMission;
    public bool objectiveDone;
    

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("there are two ore more Elevators in the scene destroying component");
            Destroy(this);
        }

        if (openOnStart)
        {
            animator.SetTrigger("open");
            Gamemanager.AudioBox(bingClip, 1f, Vector3.zero, Quaternion.identity);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector2)transform.position+offset, new Vector2(size.x, size.y));
    }

    // Update is called once per frame
    void Update()
    {
        Collider2D collider = Physics2D.OverlapBox((Vector2)transform.position+ offset, new Vector2(size.x, size.y),mask);
        if (collider != null && collider.CompareTag("Player"))
        {
            if (objectiveDone)
            {
                animator.SetTrigger("open");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && objectiveDone)
        {
            animator.SetTrigger("close");
            StartCoroutine(CallNextLevel());
        }    
    }

    IEnumerator CallNextLevel()
    {
        yield return new WaitForSeconds(3);
        UIManager.instance.ChangeScene(SceneManager.GetActiveScene().buildIndex+1);
    }
}
