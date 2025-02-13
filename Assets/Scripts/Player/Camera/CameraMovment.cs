using System.Collections;
using UnityEngine;

//this is anchorpoint based movment
public class CameraMovment : MonoBehaviour
{
    public float cameraSpeed;
    public Transform[] Anchorpoints;

    [HideInInspector]
    public static CameraMovment instance;

    Transform player1;
    Transform player2;

    //lerp the camera between the two positions
    public Vector2[] Anchors;

    public Vector2 targetPosition;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }else if (instance != null)
        {
            Debug.LogWarning("two or more cameras in the scene, activation self destruct sequence");
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player1 != null)
        {

            targetPosition.x = Mathf.InverseLerp(transform.TransformPoint(Anchors[0]).x, transform.TransformPoint(Anchors[1]).x, player1.position.x);
            targetPosition.y = Mathf.InverseLerp(transform.TransformPoint(Anchors[0]).y, transform.TransformPoint(Anchors[1]).y, player1.position.y);

            targetPosition = Vector3.Lerp(Anchors[0], Anchors[1], targetPosition.x / targetPosition.y);

            transform.position = Vector3.Lerp(transform.position, new Vector3(Anchors[0].x, Anchors[0].y, transform.position.z), Time.deltaTime * (cameraSpeed + 1));
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        if (Anchors.Length > 0)
        {
            Gizmos.DrawLine(Anchors[0], Anchors[1]);
            Gizmos.DrawSphere(Anchors[0], .15f);
            Gizmos.DrawSphere(Anchors[1], .15f);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(targetPosition, .2f);
        }
    }

    //finds the closest anchorpoint and sets it to the target
    Vector2[] GetClosestAnchorpoint()
    {
        //gets the closest anchorpoint
        Vector3 closestAnchor = new Vector3(999f, 999f, 999f);
        for (int i = 0; i < Anchorpoints.Length; i++)
        {
            float distance = Vector2.Distance(player1.position, Anchorpoints[i].position);
            //999 > 6
            if (Vector2.Distance(player1.position, closestAnchor) > distance)
            {
                closestAnchor = Anchorpoints[i].position;
            }
        }

        //get the seconds closest anchorpoint
        Vector3 secondClosestAnchor = new Vector3(999f, 999f, 999f);
        for (int j = 0; j < Anchorpoints.Length; j++)
        {
            float distance = Vector2.Distance(player1.position, Anchorpoints[j].position);
            if (Vector2.Distance(player1.position, secondClosestAnchor) > distance)
            {
                if (Anchorpoints[j].position != closestAnchor)
                {
                    secondClosestAnchor = Anchorpoints[j].position;
                }
            }
        }

        Vector2[] n = new Vector2[2];
        n[0] = closestAnchor;
        n[1] = secondClosestAnchor;
        return n;
    }

    //updates the target
    IEnumerator UpdateTarget()
    {
        Anchors = GetClosestAnchorpoint();
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(UpdateTarget());
    }

    public void Assignplayer(Transform player)
    {
        if (player1 == null)
        {
            player1 = player;
            //start updating when the player is assigned
            StartCoroutine(UpdateTarget());
        }
        else if (player2 == null)
        {
            player2 = player;
        }
    }
}
