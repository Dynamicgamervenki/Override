using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.iOS;

public class Player : MonoBehaviour
{
    #region inspectorVariables
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask clickableMask = 6;
    [SerializeField] private float lookRotationSpeed = 10;
    [SerializeField] private float hackRadius = 5.0f;
    [SerializeField] private Vector3 hackRadiusOffset = new Vector3(0, 5, 0);
    #endregion

    #region privateVariables
    private NavMeshAgent navMeshAgent;
    private Vector3 clickPosition;
    private BaseAction pendingAction = null;
    private ActionStates playerState;
    private bool canHack = false;
    #endregion

    private void Start()
    {
        playerState = ActionStates.Idle;
        navMeshAgent = GetComponent<NavMeshAgent>();
        gameInput.OnMoveAction += GameInput_OnMoveAction;
    }

    private void Update()
    {
         FaceTarget(navMeshAgent.destination);
  
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
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        navMeshAgent.destination = hit.point;
        playerState = ActionStates.Idle;
        if(hit.transform.TryGetComponent<BaseAction>(out BaseAction action))
        {
            navMeshAgent.stoppingDistance = action.GetStoppingDistance();
            pendingAction = action;
            StartCoroutine(CheckIfNeedToPerformAction());
        }
    }

    private void FaceTarget(Vector3 targetPosition)
    {
        if(navMeshAgent.velocity == Vector3.zero) return;

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

    private GameObject selectedHackableIObject;
    public event Action<GameObject> hackableFound;
    private Collider[] hackableColliders;
    //public void CheckForHackableItems()
    //{
    //    hackableColliders = Physics.OverlapSphere(transform.position + new Vector3(0,5,0), hackRadius);
    //    if (hackableColliders.Length == 0) return;


    //    foreach(var col in hackableColliders)
    //    {
    //        if(col.TryGetComponent<IHackable>(out IHackable i))
    //        {
    //            selectedHackableIObject = col.transform.gameObject;
    //            hackableFound.Invoke(selectedHackableIObject);
    //            break;
    //        }
    //    }
    //}

    private int currentHackableIndex = -1;

    public void CheckForHackableItems()
    {
        hackableColliders = Physics.OverlapSphere(transform.position + hackRadiusOffset, hackRadius);

        List<GameObject> hackableObjects = new List<GameObject>();

        foreach (var col in hackableColliders)
        {
            if (col.TryGetComponent<IHackable>(out IHackable h))
            {
                hackableObjects.Add(col.gameObject);
            }
        }

        if (hackableObjects.Count == 0)
            return;


        currentHackableIndex = (currentHackableIndex + 1) % hackableObjects.Count;

        selectedHackableIObject = hackableObjects[currentHackableIndex];
        hackableFound.Invoke(selectedHackableIObject);
    }


    public void Hack()
    {
        if (!selectedHackableIObject) { return; }

        if(selectedHackableIObject.TryGetComponent<IHackable>(out IHackable hack))
        {
           // transform.DORotate(selectedHackableIObject.transform.position,0.1f,RotateMode.Fast);
            hack.Hack();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, 5, 0), hackRadius);

       // if(colliders.Length == 0) return;

       // Gizmos.color = Color.red;
       // foreach (var h in colliders)
       // {
       //     Gizmos.DrawSphere(h.transform.position, 2.0f);
      //  }
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

