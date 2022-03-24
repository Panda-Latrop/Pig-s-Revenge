using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShootState
{
    initiated,
    process,
    ended,
    unready,
    none,
}
public abstract class WeaponActor : Actor
{
    protected Pawn owner;
    protected bool isFire;
    [SerializeField]
    protected bool isAutomatic;
    [SerializeField]
    protected Transform shootPoint;
    [SerializeField]
    protected float damage = 1.0f, damageMultiply = 1.0f;
    [SerializeField]
    protected float power = 100.0f, distance = 500.0f, speed = 5.0f;
    [SerializeField]
    protected float fireRate = 1.0f;
    [SerializeField]
    protected bool friendlyFire = false;
    [SerializeField]
    protected bool playMuzzleFlash;
    [SerializeField]
    protected ParticleSystem muzzleFlash;
    [SerializeField]
    protected AudioSource source;
    [SerializeField]
    protected AudioCueScriptableObject shootSound;
    [SerializeField]
    protected WeaponCreateBaseComponent projectile;
    protected float timeToShoot, nextShoot;
    protected ShootState shootState = ShootState.ended;



    public bool IsFire => isFire;
    public virtual float Damage => damage * damageMultiply;
    public virtual float Power => power * damageMultiply;
    public ShootState ShootState => shootState;
    public float DamageMultiply { get => damageMultiply; set => damageMultiply = value; }
    public Transform ShootPoint => shootPoint;
    public override void OnPush()
    {
        damageMultiply = 1.0f;
        gameObject.SetActive(false);
        StopSound();
        StopMuzzleFlash();
    }
    public void SetOwner(Pawn owner)
    {
        this.owner = owner;
    }
    protected virtual void PlaySound()
    {
        source.Stop();
        source.clip = shootSound;
        source.time = 0.0f;
        source.Play();
    }
    protected virtual void StopSound()
    {
        source.loop = false;
        source.Stop();
    }
    protected virtual void PlayMuzzleFlash()
    {
        if (playMuzzleFlash)
        {
            muzzleFlash.Stop(true);
            muzzleFlash.time = 0.0f;
            muzzleFlash.Play(true);
        }
    }
    protected virtual void StopMuzzleFlash()
    {
        muzzleFlash.Stop(true);
    }
    protected virtual bool CanShoot()
    {
        if (isFire && Time.time >= nextShoot)
        {
            if (!isAutomatic)
                isFire = false;
            nextShoot = Time.time + timeToShoot;
            return true;
        }
        return false;
    }
    public virtual void SetFire(bool fire)
    {
        isFire = fire;
    }
    public abstract ShootState Shoot(Vector3 position, Vector3 direction);
    protected abstract HurtResult CreateProjectile(Vector3 position, Vector3 direction);
    protected override void OnAwake()
    {
        timeToShoot = 1.0f / fireRate;
    }
    protected void Awake()
    {
        OnAwake();
    }
}
