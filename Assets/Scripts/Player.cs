using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

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
    #endregion

    private void Start()
    {
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
        if (Physics.Raycast(Camera.main.ScreenPointToRay(clickPosition), out hit, 100, clickableMask))
        {
            navMeshAgent.destination = hit.point;
        }
    }

    private void FaceTarget()
    {
        if(navMeshAgent.velocity == Vector3.zero) return;

        Vector3 direction = (navMeshAgent.destination - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed);
    }

    #region Getteres
    public Vector3 GetVelocity() => navMeshAgent.velocity;
    #endregion
}
