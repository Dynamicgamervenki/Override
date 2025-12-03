using UnityEngine;
using UnityEngine.Splines;

public interface IPatrol
{
    public bool IsPatroling { get; set; }
    public SplineContainer GetSplineContainer();
    public float GetPatrolSpeed();
}
