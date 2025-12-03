using UnityEngine;

public abstract class BaseAction : MonoBehaviour
{
    [SerializeField] private float stoppingDistance = 0f;
    public abstract void PerformAction();
    public abstract ActionStates GetActionState();


    #region Getters
    public float GetStoppingDistance() => stoppingDistance;
    #endregion
}
