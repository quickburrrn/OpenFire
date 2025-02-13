using UnityEngine;
using UnityEngine.InputSystem;

public class CameraOffset : MonoBehaviour
{
    public Camera cam;
    public Transform player;
    [SerializeField] float threshold;

    private void Update()
    {
        if (player != null)
        {
            if (cam != null)
            {
                Vector3 mousePos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());

                Vector3 targetPos = (player.position + mousePos) / 2f;
                targetPos.z = -10f;

                targetPos.x = Mathf.Clamp(targetPos.x, -threshold + player.position.x, threshold + player.position.x);
                targetPos.y = Mathf.Clamp(targetPos.y, -threshold + player.position.y, threshold + player.position.y);

                this.transform.position = targetPos;
            }
            else
            {
                //insted of throwing an exeption do this
                this.transform.position = new Vector3 (player.position.x,player.position.y,-10f);
                Debug.LogWarning(transform.name + " has no camera that it can calculate mouse position on");
            }
            
        }
    }
}
