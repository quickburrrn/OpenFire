using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

//this is camera folow player method
public class CameraLook : MonoBehaviour
{
    public static CameraLook instance;

    public float speed = 7;
    public float minsize;

    //this is only suppost to be used on single player
    public Transform CameraOffset;

    public Transform target;
    public Transform target2;

    public bool Playerassigned = false;

    Camera cam;
    PixelPerfectCamera pixelCam;
    float camsizemultiplayer;

    float targetDistance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.LogWarning("Camera look instance already exist, destroing object");
            Destroy(this);
        }
    }

    private void Start()
    {
        pixelCam = GetComponent<PixelPerfectCamera>();
        cam = GetComponent<Camera>();

        if (CameraOffset != null)
        {
            CameraOffset.transform.GetComponent<CameraOffset>().cam = cam;
        }
    }

    private void Update()
    {
        if (target2 != null && target2.gameObject.activeSelf)
        {
            targetDistance = Vector2.Distance(target.position, target2.position);
            Vector2 midPoint = Vector2.Lerp(target.position, target2.position, 0.5f);
                                                                                                                             //no need for the pluss one
            transform.position = Vector3.Lerp(transform.position, new Vector3(midPoint.x, midPoint.y, -10f), Time.deltaTime * (speed+1));

            //the scaling of the camera does not work due to the pixel perfect camera
            if (targetDistance+0.1f < cam.orthographicSize && cam.orthographicSize > minsize)
            {
                cam.orthographicSize -= Time.deltaTime * (speed * 1.3f);
            }else if (targetDistance-0.1f > cam.orthographicSize)
            {
                cam.orthographicSize += Time.deltaTime * speed;
            }
            else
            {
                
            }
        }
        else
        {
            if (target != null)
            {
                //checks if cameraoffset is null if it is then do defult camera movment
                if (CameraOffset != null)
                {
                    transform.position = CameraOffset.position;
                }
                else
                {
                    transform.position = new Vector3(target.position.x, target.position.y, -10f);
                }
                
            }
        }
    }

    public void AssignPlayer(Transform _target)
    {
        if (!Playerassigned)
        {
            target = _target;

            //assigns the target to the cameraoffset to
            if (CameraOffset != null)
            {
                CameraOffset.GetComponent<CameraOffset>().player = target;
            }
        }
        else
        {
            target2 = _target;
        }
        Playerassigned = true;

        

    }
}
