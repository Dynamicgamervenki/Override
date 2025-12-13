using System;
using UnityEngine;

public class HackingCoordinator : MonoBehaviour
{
    [SerializeField] private Player player;
    public static HackingCoordinator Instance { get; private set; }
    
    public event Action<GameObject> TargetFound;

    public IHijack CurrentHijacked { get; private set; }
    public IHackable CurrentHackable { get; private set; } //only for suvilance
    
    private GameObject target;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    public void NotifyHijacked(IHijack newHijacked)
    {
        if (CurrentHackable != null && CurrentHackable.HackType == HackType.Cctv)
        {
            CurrentHackable.IsHacked = false;
        }
        
        if (CurrentHijacked != null && CurrentHijacked != newHijacked)
        {
            CurrentHijacked.IsHijacked = false;
        }
        CurrentHijacked = newHijacked;
        newHijacked.IsHijacked = true;
        
        player.SetPlayerState(ActionStates.Hijack);
    }

    public void NotifyHacked(IHackable hackableTarget)
    {
        if (hackableTarget.HackType != HackType.Cctv)
            return;

        if (CurrentHijacked != null)
        {
            CurrentHijacked.IsHijacked = false;
            CurrentHijacked = null;
        }
        
        if (CurrentHackable != null && CurrentHackable != hackableTarget)
        {
            CurrentHackable.IsHacked = false;
        }
        CurrentHackable = hackableTarget;
        hackableTarget.IsHacked = true;
        player.SetPlayerState(ActionStates.ControllingCctv);
    }

    public void RaiseTargetFound(GameObject target)
    {
        TargetFound?.Invoke(target);
    }
    
    
    
    public void NotifySwitchBack()
    {
        if (CurrentHijacked != null)
        {
            CurrentHijacked.IsHijacked = false;
            CurrentHijacked = null;
        }

        if (CurrentHackable != null && CurrentHackable.HackType == HackType.Cctv)
        {
            CurrentHackable.IsHacked = false;
            CurrentHackable = null;
        }

        player.SetPlayerState(ActionStates.Idle);
    }
    public void NotifySelfDestruct()
    {
        if (CurrentHijacked != null)
        {
            CurrentHijacked.IsHijacked = false;
            CurrentHijacked = null;
        }
        
        player.SetPlayerState(ActionStates.Idle);
    }
    
}
