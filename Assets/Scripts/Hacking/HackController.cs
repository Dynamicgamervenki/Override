using System;
using UnityEngine;
public class HackController : MonoBehaviour
{
    [Header("Hacking Settings")]
    [SerializeField] private float scanRadius = 5f;
    [SerializeField] private Vector3 scanOffset = Vector3.zero;
    [SerializeField] private GameObject lineRendererPrefab;
    [SerializeField] private Color gizmoColor = Color.red;
    
    private bool _isActive;
    private HackSystemManager _manager;
    public bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;

            if (_isActive)
                SubscribeInputs();
            else
            {
                _manager.ClearCurrentTarget();
                UnsubscribeInputs();
            }
        }
    }

    private void Awake()
    {
        _manager = new HackSystemManager(lineRendererPrefab);
    }

    private void OnDisable()
    {
        var router = HackingInputRouter.Instance;
        if (router == null) return;
        UnsubscribeInputs();
    }

    private void ScanHandler()
    {
        if (!IsActive) return;
    
        _manager.CheckForHackableItems(
            transform.position,
            scanOffset, 
            scanRadius,
            transform
        );
    }
    private void HackHandler()
    {
        if (!IsActive) return;

        if (TryGetComponent(out Player p))
        {
            if (p.GetPlayerActionState() == ActionStates.UltimateAbility)
            {
                UltimateAtackHackHandler();
                return;
            }
            _manager.Hack();
        }
        _manager.Hack();
    }

    private void HijackHandler()
    {
        if (!IsActive) return;
        _manager.Hijack();
    }
    
    private void StunHandler()
    {
        if(!IsActive) return;
        _manager.Stun();
    }
    
    private void SubscribeInputs()
    {
        var router = HackingInputRouter.Instance;

        router.OnScanRequested += ScanHandler;
        router.OnHackRequested += HackHandler;
        router.OnHijackReqested += HijackHandler;
        router.OnStunRequested += StunHandler;
        router.OnUltimateAttackRequested += UltimateAttackScanHandler;
    }
    private void UnsubscribeInputs()
    {
        var router = HackingInputRouter.Instance;

        router.OnScanRequested -= ScanHandler;
        router.OnHackRequested -= HackHandler;
        router.OnHijackReqested -= HijackHandler;
        router.OnStunRequested -= StunHandler;
        router.OnUltimateAttackRequested -= UltimateAttackScanHandler;
    }

    #region UltiamteateHack
    
    bool _ultimateAttackScanDone = false;
    private void UltimateAttackScanHandler()
    {
        if (!IsActive)
            return;
        
        if (!TryGetComponent<Player>(out var player)) return;
        if (player.GetPlayerActionState() != ActionStates.UltimateAbility && !_ultimateAttackScanDone) return;
        _ultimateAttackScanDone = true;
        _manager.ToogleAllBots(transform.position, scanOffset, scanRadius, transform);
    }

    private void UltimateAtackHackHandler()
    {
        if (!IsActive) return;
        
        var canPerformUltimateHack = TryGetComponent<Player>(out var player) && player.GetPlayerActionState() == ActionStates.UltimateAbility;
        if(!canPerformUltimateHack) return;
        StartCoroutine(_manager.HackAllBots());
    }
    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position + scanOffset ,scanRadius);
    }
}
