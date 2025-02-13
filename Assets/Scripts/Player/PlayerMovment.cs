using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovment : MonoBehaviour
{
    public float walkspeed = 5f;
    public float runspeed = 7f;
    public float dashforce = 100f;

    public bool player2;

    [Tooltip("AudioSoruce that will play dash sound")][SerializeField] AudioSource dashSource; 
    [SerializeField] PlayerInput input;

    Camera cam;
    CameraShake shake;

    Rigidbody2D rb;
    Vector2 movement;
    Vector2 mousePos;
    [HideInInspector]
    public Vector2 lookDir;

    //public for testing
    public float speed;
    bool dashready = true;

    public bool moving;

    public AudioClip[] walkClips;
    public float walkVolume = 1f;

    private void OnValidate()
    {
        input = GetComponent<PlayerInput>();
    }

    void Awake()
    {
        input.actions["Dash"].performed += ctx => StartCoroutine(dash());

        input.actions["Move"].performed += ctx => movement = ctx.ReadValue<Vector2>();
        input.actions["Move"].canceled += ctx => movement = Vector2.zero;

        if (input.currentControlScheme != "KeyboardAndMouse")
            input.actions["Rotate"].performed += ctx => lookDir = ctx.ReadValue<Vector2>();
    }

    void Start()
    {
        cam = FindObjectOfType<Camera>();
        shake = cam.GetComponent<CameraShake>();

        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        //moves the player
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
        
        
        if (input.currentControlScheme == "KeyboardAndMouse")
        {
            mousePos = Mouse.current.position.ReadValue();
            mousePos = cam.ScreenToWorldPoint(mousePos);
            lookDir = mousePos - rb.position;
        }

        //rotates the player
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
    }

    public void Update()
    {
        if (movement != Vector2.zero && moving != true)
        {
            InvokeRepeating("PlayWalk", 0, 0.4f);
            moving = true;
        }
        else if (movement == Vector2.zero && moving == true)
        {
            CancelInvoke("PlayWalk");
            moving = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "door" && !dashready)
        {
            collision.GetComponent<Door>().Demolish(transform);
            shake.ShakeIt(0.1f, 0.2f);
        }
    }

    void PlayWalk()
    {
        //plays them randomly
        walkClips = Gamemanager.instance.GetWalkClips();
        Gamemanager.AudioBox(walkClips[Random.Range(0, walkClips.Length)], walkVolume, Vector3.zero, Quaternion.identity);
    }

    IEnumerator dash()
    {
        dashSource.Play();
        dashready = false;
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(0.01f);
            rb.AddForce(movement * dashforce, ForceMode2D.Force);
        }
        dashready = true;
    }
}
