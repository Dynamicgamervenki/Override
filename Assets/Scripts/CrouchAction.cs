using UnityEngine;
using UnityEngine.AI;

public class CrouchAction : BaseAction
{
    #region privateVariables
    private NavMeshAgent player;
    #endregion

    public override ActionStates GetActionState()
    {
        return ActionStates.Crouching;
    }


    public override void PerformAction()
    {
        Debug.LogWarning("Crouch Action Performed !");
    }
}
