using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class ElectroBot : AerialEnemyBase, IHackable
{
    [SerializeField] private ParticleSystem thrusterEffect;
    [SerializeField] private ParticleSystem explosionEffect;

    public HackType HackType => HackType.DestroyOnHack;

    public event Action OnHacked;

    protected override void OnStart()
    {
        base.OnStart();
        DisableEffects();
        Invoke(nameof(MoveToPatrolStateOnceOpeningAnimationIsDone),3.50f);
    }

    private void MoveToPatrolStateOnceOpeningAnimationIsDone()
    {
        StartCoroutine(PlayParticleEffect(thrusterEffect));
        SetEnemyState(EnemyState.patroling);
    }

    public void Hack()
    {
        enemyState = EnemyState.hacked;
        Vector3 target = transform.position - new Vector3 (0, 1, 0);    
        transform.DOMove(target,2.0f);

        transform.DORotate(new Vector3(0, 0, 15), 0.5f)
      .SetLoops(4, LoopType.Yoyo)
      .SetEase(Ease.InOutSine);

        StartCoroutine(ParticleOnOffEffect(thrusterEffect, 2, 0.5f));
        StartCoroutine(PlayParticleEffect(explosionEffect,3.0f));

        OnHacked?.Invoke();
        Destroy(gameObject,1.5f + explosionEffect.main.duration);

    }
    private IEnumerator PlayParticleEffect(ParticleSystem particleEffect, float delay = 0)
    {
        yield return new WaitForSeconds(delay); 
        particleEffect.gameObject.SetActive(true);
        particleEffect.Play();
    }

    private IEnumerator StopParticleEffect(ParticleSystem particleEffect ,float delay =  0)
    {
        yield return new WaitForSeconds(delay);
        if (particleEffect.isPlaying) { particleEffect.Stop(); }
        particleEffect.gameObject.SetActive(false);
    }

    private IEnumerator ParticleOnOffEffect(ParticleSystem particleFX,float totalBlinkTime,float blinkInterval)
    {
        float timer = 0f;

        while (timer < totalBlinkTime)
        {
            particleFX.Play();
            yield return new WaitForSeconds(blinkInterval);
            particleFX.Stop();
            yield return new WaitForSeconds(blinkInterval);

            timer += blinkInterval * 2f;
        }
        particleFX.Stop();
    }

    private void DisableEffects()
    {
        thrusterEffect.gameObject.SetActive(false);
        explosionEffect.gameObject.SetActive(false);
    }
}
