using UnityEngine;

public class ElectroBot : AerialEnemyBase
{
    [SerializeField] private ParticleSystem ThrusterEffect;
    protected override void OnStart()
    {
        base.OnStart();
        ThrusterEffect.gameObject.SetActive(false);
        Invoke(nameof(MoveToPatrolStateOnceOpeningAnimationIsDone),3.50f);
    }

    private void MoveToPatrolStateOnceOpeningAnimationIsDone()
    {
        PlayThrusterEffect();
        SetEnemyState(EnemyState.patroling);
    }

    private void PlayThrusterEffect()
    {
        ThrusterEffect.gameObject.SetActive(true);
        ThrusterEffect.Play();
    }
}
