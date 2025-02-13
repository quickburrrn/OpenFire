using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shoot : MonoBehaviour
{
    public WorldCanvas worldCanvas;
    public CameraShake shake;

    public Transform bulletPoint;
    [SerializeField]
    int ammoInmag;
    public weapon DefultPistol;

    [SerializeField] PlayerInput input;

    public AudioSource audioSource;

    weapon Weapon;
    private bool reloading = false;
    private bool ready = true;
    private bool firing = false;

    private void OnValidate()
    {
        input = GetComponent<PlayerInput>();
    }

    void Awake()
    {
        input.actions["Shoot"].performed += ctx => AtemptShoot();
        input.actions["Shoot"].canceled += ctx => cancelShoot();
        input.actions["Reload"].performed += ctx => StartCoroutine(reload(Weapon.reloadTime));
    }

    private void Start()
    {
        //sets the weapon to defult
        Weapon = DefultPistol;
        shake = FindObjectOfType<CameraShake>();
    }

    void cancelShoot()
    {
        CancelInvoke("shoot");
        shake.StopCameraShaking();
        firing = false;

        //stops audio and play audio end if have one 
        if (Weapon.automatick && Weapon.AutomaticSource != null)
        {
            Weapon.AutomaticSource.Stop();
        }        
        if (Weapon.StopClip != null ) {
            Gamemanager.AudioBox(Weapon.StopClip,1f,Vector3.zero,Quaternion.identity);
        }
    }

    //atemts to shoot
    void AtemptShoot()
    {
        if (this == null)return;

        //checks if the gun is ready and is not reloading
        if (ready && !reloading)
        {
            if (Weapon.automatick)
            {
                InvokeRepeating("shoot", 0f, 1f / Weapon.Firerate);
                //firing = true;
                shake.ConstantShake(Weapon.shakeMagnitude, Weapon.shakeTime);
            }
            else
            {
                shoot();
                if (shake != null)
                {
                    shake.ShakeIt(Weapon.shakeMagnitude, Weapon.shakeTime);
                }
            }
            StartCoroutine(countdown(Weapon.cooldown));
        }
    }

    public void ChangeWeapon(weapon stats)
    {
    //    //checks if there is a weapon to swich to if not return
        if (stats == null)
        {
            //this is happening if there in no gun
            Weapon = DefultPistol;
            Gamemanager.AudioBox(Weapon.equip,1f, transform.position, Quaternion.identity);
            worldCanvas.popup(Weapon.id);
            return;
        }
        Weapon = stats;
        Gamemanager.AudioBox(Weapon.equip,1f, Vector3.zero, Quaternion.identity);
        worldCanvas.popup(Weapon.id);
    }

    public void shoot()
    {
        if (this == null)
        {
            return;
        }

        if (ammoInmag > 0)
        {
            //instantiates the enmpty bullet and the particles
            if (Weapon.emptyBulletParticle != null)
            {
                if (this != null)
                {
                    //sets the correct rotation and all of that
                    GameObject emptyBullet = Instantiate(Weapon.emptyBulletParticle, transform.position, Quaternion.identity);
                    Vector3 angle = new Vector3(0f, 0f, transform.rotation.ToEulerAngles().z);
                    emptyBullet.transform.rotation = transform.rotation;
                    emptyBullet.GetComponent<ParticleSystem>().startRotation3D = new Vector3(0.0f, 0.0f, angle.z);
                    emptyBullet.GetComponent<ParticleSystem>().Play();
                    Destroy(emptyBullet, 1f);
                }
            }
            //instantiates the muzzle flash
            Instantiate(Weapon.muzzleFlash, bulletPoint);
            
            
            if (Weapon.burst)
            {
                StartCoroutine(burstfire());
                return;
            }
            //fires the actual bullet
            Firebullet();
        }
        else
        {
            if (this != null)
                StartCoroutine(reload(Weapon.reloadTime));
        }
    }

    //this is spawining the bullet
    public void Firebullet()
    {
        GameObject _bullet = Instantiate(Weapon.bullet, bulletPoint.position, bulletPoint.rotation);
        _bullet.transform.Rotate(transform.forward * UnityEngine.Random.Range(-Weapon.spred, Weapon.spred), Space.Self);
        ammoInmag -= 1;
        worldCanvas.UpdateAmmo(ammoInmag);

        //plays the audio

        //uses Shoot if not has a automatic clip
        if (Weapon.Shoot != null && Weapon.AutomaticSource == null && !Weapon.burst)
            Gamemanager.AudioBox(Weapon.Shoot, Weapon.ShootVolume, Vector3.zero, Quaternion.identity);
        
        if (!firing)
        {            
            if (Weapon.automatick && Weapon.AutomaticSource != null)
            {
                Gamemanager.AudioBox(Weapon.Shoot, Weapon.ShootVolume, Vector3.zero, Quaternion.identity);
                Weapon.AutomaticSource.volume = Weapon.ShootVolume;
                Weapon.AutomaticSource.PlayDelayed(Weapon.AutomaticSourceDelay);
            }
        }
        firing = true;

        if (Weapon.shell != null)
        {
            Gamemanager.AudioBoxDelayed(Weapon.shell, Vector3.zero, Quaternion.identity, 0.5f);
        }

    }

    IEnumerator countdown(float amount)
    {
        ready = false;
        yield return new WaitForSeconds(amount);
        ready = true;
    }

    IEnumerator burstfire()
    {
        Gamemanager.AudioBox(Weapon.Shoot, Weapon.ShootVolume, Vector3.zero, Quaternion.identity);
        for (int i = 0; i < Weapon.bulletAmount; i++)
        {
            if (ammoInmag > 0)
            {
                Firebullet();
                shake.ShakeIt(Weapon.shakeMagnitude, Weapon.shakeTime);
                yield return new WaitForSeconds(Weapon.bulletDelay);
            }
        }
    }

    IEnumerator reload(float time)
    {
        cancelShoot();
        worldCanvas.popup("reloading...");
        audioSource.clip = Weapon.Reload;
        audioSource.Play();
        reloading = true;
        yield return new WaitForSeconds(time);
        ammoInmag = Weapon.magsize;
        reloading = false;
    }
}
