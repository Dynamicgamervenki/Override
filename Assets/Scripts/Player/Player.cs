using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
public class Player : MonoBehaviour
{
    #region inspectorVariables
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask clickableMask = 6;
    [SerializeField] private float lookRotationSpeed = 10;
    #endregion

    #region privateVariables
    private NavMeshAgent _navMeshAgent;
    private Vector3 _clickPosition;
    private BaseAction _pendingAction;
    private ActionStates _playerState = ActionStates.Idle;
    private HackSystemManager _hackSystemManager;
    private HackController _hackController;
    #endregion

    #region events
    public event Action<ActionStates> PlayerStateChanged;
    public event Action HackSystemManagerInitialized;
    #endregion

    private void Start()
    {
        _playerState = ActionStates.Idle;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        gameInput.OnMoveAction += GameInput_OnMoveAction;
        _hackController = GetComponent<HackController>();
        _hackController.IsActive = true;
        
     //   SetPlayerState(ActionStates.UltimateAbility);
    }
    
    private void Update()
    {
         FaceTarget(_navMeshAgent.destination);
    }

    private void GameInput_OnMoveAction(object sender, EventArgs e)
    {
        ClickToMove();
    }

    private void ClickToMove()
    {
        if(_playerState  == ActionStates.ControllingCctv || _playerState == ActionStates.Hijack) return;

        _clickPosition = Mouse.current.position.ReadValue();
        var raycast = (Physics.Raycast(Camera.main!.ScreenPointToRay(_clickPosition), out var hit, 100, clickableMask));

        if(!raycast) return;
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        _navMeshAgent.destination = hit.point;
        _playerState = ActionStates.Idle;
        if (!hit.transform.TryGetComponent(out BaseAction action)) return;
        _navMeshAgent.stoppingDistance = action.GetStoppingDistance();
        _pendingAction = action;
        StartCoroutine(CheckIfNeedToPerformAction());
    }

    private void FaceTarget(Vector3 targetPosition)
    {
        if(_navMeshAgent.velocity == Vector3.zero) return;

        Vector3 direction = (targetPosition - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed);
    }

    IEnumerator CheckIfNeedToPerformAction()
    {
        while (!HasReachedDestination())
        {
            yield return null;
        }
        
        _pendingAction.PerformAction();
        _playerState = _pendingAction.GetActionState();
        _pendingAction = null;
    }

    private bool HasReachedDestination()
    {
        if (_navMeshAgent.pathPending) return false;

        if (_navMeshAgent.remainingDistance > _navMeshAgent.stoppingDistance) return false;

        if (_navMeshAgent.hasPath && _navMeshAgent.velocity.sqrMagnitude != 0f) return false;
        return true;
    }

    public void SetPlayerState(ActionStates newState)
    {
        _playerState = newState;
        _hackController.IsActive = newState is ActionStates.Idle or ActionStates.Crouching or ActionStates.UltimateAbility;
        PlayerStateChanged?.Invoke(newState);
    }
    
    //TEMP
    public void TunrOnUltimateAbilityMode()
    {
        SetPlayerState(ActionStates.UltimateAbility);
    }


    #region Getteres
    public Vector3 GetVelocity() => _navMeshAgent.velocity;
    public ActionStates GetPlayerActionState() => _playerState;
    
    public HackSystemManager GetHackSystemManager() => _hackSystemManager;
    #endregion
}

public enum ActionStates
{
    Idle,
    Crouching,
    ControllingCctv,
    Hijack,
    UltimateAbility
}

