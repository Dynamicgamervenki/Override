using UnityEngine;
using UnityEngine.Splines;

public class SplinePatrol
{
    private SplineContainer spline;
    private float patrolSpeed = 2f;

    public float distancePercentage = 0f;
    private Vector3 tangent;
    private float splineLength;


    public float GetCurrentDistancePercentage() => distancePercentage;
    public float GetSplineLength() => spline.CalculateLength();

    public SplinePatrol(SplineContainer splineContainer,float patrolSpeed = 2f)
    {
        this.spline = splineContainer;
        this.patrolSpeed = patrolSpeed;
    }

    public void MoveAlongSpline(Transform target)
    {
        if (spline == null) return;

        distancePercentage += patrolSpeed * Time.deltaTime / spline.CalculateLength();
        if (distancePercentage > 1f)
        {
            //  distancePercentage = 0f; // Loop back to start
            distancePercentage = 1f;    
            patrolSpeed = -patrolSpeed;               // reverse direction
        }

        if(distancePercentage <= 0f)
        {
            distancePercentage = 0f;
            patrolSpeed = -patrolSpeed;

        }
        Vector3 currentPosition = spline.EvaluatePosition(distancePercentage);
        target.position = currentPosition;

        tangent = spline.EvaluateTangent(distancePercentage);

        if(patrolSpeed < 0f)
            tangent = -tangent;

        if (tangent != Vector3.zero)
        {
            target.rotation = Quaternion.LookRotation(tangent);
        }
    }

}
