using System.Collections;
using TMPro.EditorUtilities;
using UnityEngine;

[RequireComponent(typeof(Gamemanager))]
public class BossManger : MonoBehaviour
{
    [SerializeField]
    //is there a boss on the level?
    private bool isboss = false;

    [Header("This script is managing the bossfight")]
    public Floor bosszone;
    public Gamemanager gamemanager;
    public GameObject Camera;
    [Header("there can only be one boss")]
    public GameObject bossFight;
    public Healthbar healthbar;
    
    [HideInInspector]
    public static BossManger instance;

    bool bossDefeated;

    private void OnValidate()
    {
        gamemanager = GetComponent<Gamemanager>();
    }

    private void Start()
    {
        bossDefeated = false;

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log("Two or more bossManagers in scene");
        }

        Camera = FindObjectOfType<Camera>().gameObject;

        StartCoroutine(update());

        for (int i = 0; i < EnemiesManager.instance.enemies.Count; i++)
        {
            if (EnemiesManager.instance.enemies[i].GetComponent<AIManager>().boss)
            {
                if (bossFight == null)
                {
                    bossFight = EnemiesManager.instance.enemies[i];
                    
                    //assigns the slider to the boss
                    if (healthbar != null)
                    {
                        bossFight.GetComponent<AIManager>().healthBar = healthbar;
                    }else
                    {
                        Debug.LogWarning("No assigned Healthslider to the boss");
                    }
                    
                }
                else
                {
                    Debug.LogError("there are two bosses in the scene");
                }
                StartCoroutine(lateUpdate());
            }
        }
        
    }

    IEnumerator lateUpdate()
    {
        //need to to this late because values has to be changed before it deactivates
        yield return new WaitForEndOfFrame();
        healthbar.gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b,.3f);
        Gizmos.DrawCube((Vector2)transform.position + bosszone.offset, bosszone.scale);
    }

    //gets caled when the bossfight starts
    void initBoss()
    {
        if (!bossDefeated)
        {
            StartCoroutine(MoveCamera());
            Camera.GetComponent<CameraLook>().enabled = false;

            //turn on the health bar
            healthbar.gameObject.SetActive(true);
        }
    }

    //moves the camera smootly to position
    IEnumerator MoveCamera()
    {
        Vector2 endPosition = ((Vector2)transform.position + bosszone.offset);
        Vector3 startPosition = Camera.transform.position;
        for (float t = 0; t <= 1; t += .01f) 
        {
            yield return new WaitForSeconds(.02f);
            Camera.transform.position = Vector3.Lerp(startPosition, new Vector3(endPosition.x, endPosition.y, Camera.transform.position.z), t);
        }
    }

    //gets caled when the boss if defeted
    public static void execBoss()
    {
        //turn of the health bar
        instance.healthbar.gameObject.SetActive(false);

        instance.Camera.GetComponent<CameraLook>().enabled=true;
        instance.bossDefeated = true;
    }

    //update function
    IEnumerator update()
    {
        //checks if the player is inside the bossfight area
        RaycastHit2D raycast = Physics2D.BoxCast((Vector2)transform.position + bosszone.offset, bosszone.scale, 0f, Vector2.zero, Mathf.Infinity, gamemanager.mask);
        if (raycast && raycast.collider.CompareTag("Player") && isboss)
        {
            initBoss();
            yield return null;
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(update());
        }
    }
}
