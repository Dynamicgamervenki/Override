using System;
using System.Collections;
using UnityEngine;

public class Circuit : MonoBehaviour , IHackable,IZoom
{
    public HackType HackType => HackType.DestroyOnHack;
    public event Action OnHacked;
    public bool IsHacked { get; set; }
    
    private HackKillZone _hackKillZone;
    
    [SerializeField] private ParticleSystem electricEffect;
    [SerializeField] private ParticleSystem boomEffect;
    [SerializeField] private ParticleSystem electricSparks;
    [SerializeField] private float hearingRadius = 50.0f;
    [SerializeField] private float destroyRadius = 30.0f;
    public void Hack()
    {
        IsHacked = true;
        OnHacked?.Invoke();
        StartCoroutine(PlayParticleEffect(electricEffect));
        StartCoroutine(PlayParticleEffect(boomEffect));
        electricSparks.loop = true;
        StartCoroutine(PlayParticleEffect(electricSparks));
        
        _hackKillZone = new HackKillZone(transform.position,destroyRadius,gameObject,DamageType.Hack,true);
        SoundEvent soundEvent = new SoundEvent(transform.position,hearingRadius);
        SoundSystem.EmitSound(soundEvent);
        StartCoroutine(_hackKillZone.CheckForDestroyableItems(0.3f));
    }
    

    private IEnumerator PlayParticleEffect(ParticleSystem particleEffect, float delay = 0)
    {
        yield return new WaitForSeconds(delay); 
        particleEffect.gameObject.SetActive(true);
        particleEffect.Play();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hearingRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, destroyRadius);
    }

    public float FOV => 70.0f;
}
