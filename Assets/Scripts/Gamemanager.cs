using System.Collections;
using UnityEngine;

//need to make it serializable or it won't show up in the inspector
[System.Serializable]
public class Floor
{
    public string material;
    public Vector2 offset;
    public Vector2 scale;
    public AudioClip[] Clips;

    public Floor(string material, Vector2 offset, Vector2 scale, AudioClip[] clips)
    {
        this.material = material;
        this.offset = offset;
        this.scale = scale;
        this.Clips = clips;
    }
}

public class Gamemanager : MonoBehaviour
{
    [HideInInspector]
    public static Gamemanager instance;

    public GameObject audioBox;

    public LayerMask mask;

    public Floor[] floors;
    public Floor defultFloor; 

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("there are two or more gamemanagers in scene, Destroying component");
            Destroy(this);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        //draws every floor
        if (floors != null)
        {
            for (int i = 0; i < floors.Length; i++)
            {
                //assign random colors using i as seeds
                Gizmos.color = new Color(Mathf.Sin((i + 1) * 4892934), Mathf.Sin((i + 1) * 4892894), Mathf.Sin((i + 1) * 4862834), .3f);
                Gizmos.DrawCube((Vector2)transform.position + floors[i].offset, floors[i].scale);
            }
        }
    }

    //returns the floor that the player is standing on
    //PERFORMANCE
    public AudioClip[] GetWalkClips()
    {
        if (floors != null)
        {
            //goes trough each floor
            for (int i = 0; i < floors.Length; i++)
            {
                RaycastHit2D ray = Physics2D.BoxCast((Vector2)transform.position + floors[i].offset, floors[i].scale, 0f, Vector2.zero, Mathf.Infinity, mask);
                if (ray)
                {
                    return floors[i].Clips;
                }
            }
        }
        return defultFloor.Clips;
    }

    public static void AudioBox(AudioClip clip, float audioLevel, Vector3 pos, Quaternion rotation)
    {
        GameObject audiobox = Instantiate(Gamemanager.instance.audioBox, pos, rotation);
        AudioSource audioboxsource = audiobox.GetComponent<AudioSource>();
        audioboxsource.volume = audioLevel;
        audioboxsource.clip = clip;
        audioboxsource.Play();
        Destroy(audiobox, 5f);
    }

    #region AudioBox delayedfunction

    public static void AudioBoxDelayed(AudioClip clip, Vector3 pos, Quaternion rotation, float delay)
    {
        instance.StartAudiBoxCountdown(clip, pos, rotation, delay);
    }

    public void StartAudiBoxCountdown(AudioClip clip, Vector3 pos, Quaternion rotation, float delay)
    {
        StartCoroutine(AudiBoxCountdown(clip, pos, rotation, delay));
    }

    public IEnumerator AudiBoxCountdown(AudioClip clip, Vector3 pos, Quaternion rotation, float delay)
    {
        //cals regular audiobox when the count down is complete
        yield return new WaitForSeconds(delay);
        AudioBox(clip, 1f, pos, rotation);
    }

    #endregion
}
