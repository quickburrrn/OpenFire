using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerMovment))]
public class Interaction : MonoBehaviour
{
    [Space]
    public float health;
    public Healthbar healthbar;
    public LayerMask mask;

    Inventory inventory;
    PlayerMovment playerController;
    Shoot weaponSystem;
    [SerializeField] PlayerInput input;

    private float maxhealth;
    private int selectedSlot;

    [HideInInspector]
    public bool hasKey = false;

    private void OnValidate()
    {
        input = GetComponent<PlayerInput>();
    }

    void Awake()
    {
        input.actions["SwichItemRight"].performed += ctx => inventory.ChangeSelectedSlot(inventory.selectedSlot + 1);
        input.actions["SwichItemLeft"].performed += ctx => inventory.ChangeSelectedSlot(inventory.selectedSlot - 1);

        input.actions["Interact"].performed += ctx => Interact();
    }

    void Start()
    {
        EnemiesManager.instance.AddTarget(gameObject);
        
        if (CameraLook.instance != null)
            CameraLook.instance.AssignPlayer(transform);
        if (CameraMovment.instance != null)
            CameraMovment.instance.Assignplayer(transform);
        healthbar.maxvalue = health;
        weaponSystem = GetComponent<Shoot>();
        playerController = GetComponent<PlayerMovment>();
        inventory = GetComponent<Inventory>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("key"))
        {
            weaponSystem.worldCanvas.popup("aquired key");
            Elevator.instance.objectiveDone = true;
            Destroy(collision.gameObject);
        }
    }

    void Interact()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, playerController.lookDir, 3f, mask);
        if (hit)
        {
            Debug.Log("Interacting with " + hit.collider.name);
            if (hit.collider.tag == "door")
            {
                hit.collider.GetComponent<Door>().Open();
            }
        }
    }

    public void Death()
    {
        UIManager.instance.ChangeScene(SceneManager.GetActiveScene().buildIndex);
        Destroy(input);
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health < 0)
        {
            Death();
        }
        healthbar.ChangeValue(health);
    }
}
