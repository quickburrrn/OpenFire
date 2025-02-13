using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weapon : MonoBehaviour
{
    [Header("Graphics")]
    [Tooltip("the empty bullet that fly out of the gun")]public GameObject emptyBulletParticle;
    public GameObject muzzleFlash;
    public GameObject bullet;
    public string id;
    public float shakeMagnitude = 0f, shakeTime = 0f;
    [Header("Audio")]
    [Tooltip("the shell that hits the floor")] public AudioClip shell;
    public AudioClip Reload;
    public AudioClip Shoot;
    public float ShootVolume = 1f;
    public AudioClip equip;
    public AudioClip StopClip;
    [Tooltip("Audiource for automatic weapons")]public AudioSource AutomaticSource;
    [Tooltip("Delay for automaticSource")]public float AutomaticSourceDelay;
    [Header("weapon stats")]
    public float cooldown = 0.2f;
    public float spred = 1f;
    public int magsize = 10;
    public int ammoInMag = 10;
    public float reloadTime = 1;
    [Space]
    [Tooltip("automatic")]
    public bool automatick = false;
    public float Firerate = 5f;
    [Space]
    public bool burst = false;
    public int bulletAmount = 5;
    public float bulletDelay = 0f;
    [Space]
    public bool bulletStatsOverride;

}
