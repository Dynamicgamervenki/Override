using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.iOS;

public class Player : MonoBehaviour
{
    #region inspectorVariables
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask clickableMask = 6;
    [SerializeField] private float lookRotationSpeed = 10;
    #endregion

    #region privateVariables
    private NavMeshAgent navMeshAgent;
    private Vector3 clickPosition;
    private BaseAction pendingAction = null;
    private ActionStates playerState;
    #endregion

    private void Start()
    {
        playerState = ActionStates.Idle;
        navMeshAgent = GetComponent<NavMeshAgent>();
        gameInput.OnMoveAction += GameInput_OnMoveAction;
    }

    private void Update()
    {
        FaceTarget();
    }

    private void GameInput_OnMoveAction(object sender, System.EventArgs e)
    {
        ClickToMove();
    }

    private void ClickToMove()
    {
        RaycastHit hit;
        clickPosition = Mouse.current.position.ReadValue();
        bool raycast = (Physics.Raycast(Camera.main.ScreenPointToRay(clickPosition), out hit, 100, clickableMask));

        if(!raycast) return;
        navMeshAgent.destination = hit.point;
        playerState = ActionStates.Idle;
        if(hit.transform.TryGetComponent<BaseAction>(out BaseAction action))
        {
            navMeshAgent.stoppingDistance = action.GetStoppingDistance();
            pendingAction = action;
            StartCoroutine(CheckIfNeedToPerformAction());
        }
    }

    private void FaceTarget()
    {
        if(navMeshAgent.velocity == Vector3.zero) return;

        Vector3 direction = (navMeshAgent.destination - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed);
    }

    IEnumerator CheckIfNeedToPerformAction()
    {
        while (!HasReachedDestination()) 
        {
            yield return null;
        }
        pendingAction.PerformAction();
        playerState = pendingAction.GetActionState();
        pendingAction = null;
    }

    private bool HasReachedDestination()
    {
        if (navMeshAgent.pathPending) return false;

        if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance) return false;

        if (navMeshAgent.hasPath && navMeshAgent.velocity.sqrMagnitude != 0f) return false;

        return true;
    }

    #region Getteres
    public Vector3 GetVelocity() => navMeshAgent.velocity;
    public ActionStates GetPlayerActionState() => playerState;
    #endregion
}

public enum ActionStates
{
    Idle,
    Crouching,
    Hacking
}

