using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class HackSystemManager
{
    private GameObject _selectedHackableIObject;
    private Collider[] _hackableColliders;
    private List<GameObject> _hackableObjects;

    private int _currentHackableIndex = -1;
    private HackLine _line;
    
    private readonly GameObject _lineRenderer;

    public HackSystemManager(GameObject lineRenderer)
    {
        _lineRenderer = lineRenderer;
    }
    
    public void CheckForHackableItems(Vector3 startPosition, Vector3 offset, float radius, Transform parent)
    {
        var colliders = Physics.OverlapSphere(startPosition + offset, radius);
        _hackableObjects = new List<GameObject>();

        foreach (var col in colliders)
        {
            if (!col.TryGetComponent(out IHackable hackable))
                continue;

            _hackableObjects.Add(col.gameObject);
            hackable.OnHacked += H_OnHacked;
        }

        if (_hackableObjects.Count == 0)
            return;
        
        int count = _hackableObjects.Count;
        int nextIndex = _currentHackableIndex;
        bool foundValid = false;

        for (int i = 0; i < count; i++)
        {
            nextIndex = (nextIndex + 1) % count;

            var go = _hackableObjects[nextIndex];
            
            if (go.transform == parent)
                continue;
            
            if (go.TryGetComponent(out IHackable hackable) && hackable.IsHacked && hackable.HackType == HackType.DestroyOnHack)
                continue;

            foundValid = true;
            break;
        }
        
        if (!foundValid)
            return;

        _currentHackableIndex = nextIndex;
        
        _selectedHackableIObject = _hackableObjects[_currentHackableIndex];
        
        HackingCoordinator.Instance.RaiseTargetFound(_selectedHackableIObject);
        
        if (_line == null)
        {
            HackLineManager hackLineManager = new HackLineManager(_lineRenderer);
            _line = hackLineManager.CreateLine(parent, _selectedHackableIObject.transform);
        }
        else
        {
            _line.ChangePoints(parent, _selectedHackableIObject.transform);
        }
    }

    private void H_OnHacked()
    {
        if (!_selectedHackableIObject) return;
        if (_selectedHackableIObject.TryGetComponent<IHackable>(out var h))
        {
            h.OnHacked -= H_OnHacked;
        }
        _line.ResetLine();
    }

    public void Hack()
    {
        if (!_selectedHackableIObject) { return; }

        if(_selectedHackableIObject.TryGetComponent<IHackable>(out var hack))
        {
            if(hack.HackType == HackType.DestroyOnHack && hack.IsHacked) return;
            
            hack.Hack();
            HackingCoordinator.Instance.NotifyHacked(hack);
            _selectedHackableIObject = null;
        }
    }

    public void Hijack()
    {
        if(!_selectedHackableIObject) {return;}

        if (_selectedHackableIObject.gameObject.TryGetComponent<IHijack>(out var hijack))
        {
            HackingCoordinator.Instance.NotifyHijacked(hijack);
            hijack.Hijack();
            _line.ResetLine();
        }
    }
    public void Stun()
    {
        if(!_selectedHackableIObject) {return;}

        if (_selectedHackableIObject.gameObject.TryGetComponent<IStun>(out var stun))
        {
          //  HackingCoordinator.Instance.NotifyHijacked(stun);
            stun.Stun(5.0f);
            _line.ResetLine();
        }
    }
    
    public void ClearCurrentTarget()
    {
        if(_selectedHackableIObject == null) return;
        
        _selectedHackableIObject = null; 
        _line.ResetLine();  
    }

}
