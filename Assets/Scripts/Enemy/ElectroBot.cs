using DG.Tweening;
using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;

public class ElectroBot : AerialEnemyBase, IHackable , IDamgable ,IHijack,IStun
{
    [Header("Effects")]
    [SerializeField] private ParticleSystem thrusterEffect;
    [SerializeField] private ParticleSystem explosionEffect;
    
    [Header("Parameters")]
    [SerializeField] private float destroyRadius = 15.0f;
    [SerializeField] private float stunDuration = 5.0f;
    
    [Header("References")]
    [SerializeField] private CinemachineCamera primaryCamera;
    [SerializeField] private GameObject botCamera;
    [SerializeField] private MeshRenderer[] eyeRenderers;

    #region PrivateVariables
    private HackKillZone _hackKillZone;
    private HackController _hackController;
    private CinemachineCamera _botCinemachineCamera;
    private HackSystemManager _hackSystemManager;
    private bool _isHijacked;
    private bool _isActive;
    #endregion

    #region  IHackable
        public HackType HackType => HackType.DestroyOnHack;
        public event Action OnHacked;
        public bool IsHacked { get; set; }
    #endregion

    public bool IsHijacked
    {
        get => _isHijacked;
        set
        {
            _isHijacked = value;
            _hackController.IsActive = value;
        }
    }
        
    protected override void OnStart()
    {
        base.OnStart();
        _hackController = GetComponent<HackController>();
        DisableEffects();
        Invoke(nameof(MoveToPatrolStateOnceOpeningAnimationIsDone),3.50f);
        HackingInputRouter.Instance.OnSwitchBackRequested += SwitchToNormalCamera;
        HackingInputRouter.Instance.OnSelfDestructRequested += SelfDestruct;
        HackingInputRouter.Instance.OnRightPatrolPathSelected += InstanceOnOnRightPatrolPathSelected;
        HackingInputRouter.Instance.OnLeftPatrolPathSelected += InstanceOnOnLeftPatrolPathSelected;
    }
    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (IsHijacked)
        {
            CheckForSplineJunction();
        }
    }


    private void InstanceOnOnRightPatrolPathSelected()
    {
        if (!Junction && Junction.direction != Direction) return;
        ChangeSplineContainer(Junction.GetRightSplineContaier());
        NewsplineContainer = null;
        Junction = null;
    }
    private void InstanceOnOnLeftPatrolPathSelected()
    {
        if (!Junction && Junction.direction != Direction) return;
        ChangeSplineContainer(Junction.GetLeftSplineContaier());
        NewsplineContainer = null;
        Junction = null;
    }

    private void SwitchToNormalCamera()
    {
        if(!IsHijacked) return;
        if(!_botCinemachineCamera) return;
        
        _botCinemachineCamera.Target.TrackingTarget = null;
        _botCinemachineCamera.Priority = 9;
        HackingInputRouter._currentMaxPriority++;
        primaryCamera.Priority = HackingInputRouter._currentMaxPriority;
        HackingCoordinator.Instance.NotifySwitchBack();
    }


    private void MoveToPatrolStateOnceOpeningAnimationIsDone()
    {
        StartCoroutine(PlayParticleEffect(thrusterEffect));
        
        if(GetEnemyState() == EnemyState.Stunned) return;
        SetEnemyState(EnemyState.Patrolling);
    }
    public void Hack()
    {
        IsHacked = true;
        SetEnemyState(EnemyState.Hacked);
        Vector3 target = transform.position - new Vector3 (0, 1, 0);    
        transform.DOMove(target,2.0f);

        transform.DORotate(new Vector3(0, 0, 15), 0.5f)
      .SetLoops(4, LoopType.Yoyo)
      .SetEase(Ease.InOutSine);

        Invoke(nameof(DestoryNearByBots),3.0f);
        StartCoroutine(ParticleOnOffEffect(thrusterEffect, 2, 0.5f));
        StartCoroutine(PlayParticleEffect(explosionEffect,3.0f));
       

        OnHacked?.Invoke();
        SwitchToNormalCamera();
        Destroy(gameObject,1.5f + explosionEffect.main.duration);
    }
    
    private void DestoryNearByBots()
    {
        _hackKillZone  = new HackKillZone(transform.position, destroyRadius, this.gameObject,DamageType.ImmediateKill);
        StartCoroutine(_hackKillZone.CheckForDestroyableItems());
    }
    
    public void TakeDamage()
    {
        SetEnemyState(EnemyState.Hacked);
        Destroy(gameObject,explosionEffect.main.duration -1.2f);
        StartCoroutine(PlayParticleEffect(explosionEffect));
    }
    
    public void Hijack()
    {
        ChangeEyeColor(Color.red);
        if (_botCinemachineCamera == null)
        {
            if (!Instantiate(botCamera.gameObject).TryGetComponent(out CinemachineCamera cam)) return;
            _botCinemachineCamera = cam;
        }
        _botCinemachineCamera.Target.TrackingTarget = transform;
        HackingInputRouter._currentMaxPriority++;
        _botCinemachineCamera.Priority = HackingInputRouter._currentMaxPriority;
    }

    public void ChangeEyeColor(Color color)
    {
        foreach (var rendere in eyeRenderers)
        {
            var mat = rendere.material;
            mat.SetColor("_BaseColor", color);
            mat.SetColor("_EmissionColor", color * 7.7f);
        }
    }

    public void SelfDestruct()
    {
        if(!IsHijacked || !gameObject) return;
        
        SwitchToNormalCamera();
        Hack();
        Destroy(_botCinemachineCamera);
        HackingCoordinator.Instance.NotifySelfDestruct();
    }
    
    private void OnDestroy()
    {
        if (_hackKillZone == null) return;
        _hackKillZone.DestoryFieldForce();
    }

    #region ParticleEffects
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
    #endregion

    public void Stun(float duration)
    {
        if (IsStunned) return;
        IsStunned = true;
        transform.DORotate(new Vector3(0, 0, 15), 0.5f)
            .SetLoops(4, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
        StartCoroutine(StopParticleEffect(thrusterEffect));
        SetEnemyState(EnemyState.Stunned);
        StartCoroutine(RecoverFromStunAfterDelay(duration));
    }

    private IEnumerator RecoverFromStunAfterDelay(float duration)
    {
        yield return new WaitForSeconds(duration);
        IsStunned = false;
        if (IsHacked) yield break;
        SetEnemyState(EnemyState.Patrolling);
        StartCoroutine(PlayParticleEffect(thrusterEffect));
    }

    public bool IsStunned { get; set; }
}