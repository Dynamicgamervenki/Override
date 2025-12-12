using UnityEngine;
using UnityEngine.Splines;

public class SplinePatrol
{
    private SplineContainer _splineContainer;
    private float _patrolSpeed = 2f;
    public float DistancePercentage = 0f;
    private Vector3 _tangent;
    private float _splineLength;
    private bool _hasReachedSpline = false;

    private float _startDistanceThreshold = 1.0f;

    public float GetDirection()
    {
        return Mathf.Sign(_patrolSpeed);
    }


    public float GetCurrentDistancePercentage() => DistancePercentage;
    public float GetSplineLength() => _splineContainer.CalculateLength();

    public SplinePatrol(SplineContainer splineContainer,float patrolSpeed = 2f)
    {
        _splineContainer = splineContainer;
        _patrolSpeed = patrolSpeed;
    }

    public void MoveAlongSpline(Transform target,int splineIndex = 0)
    {
        if (_splineContainer == null || target == null) return;
        if (splineIndex < 0 || splineIndex >= _splineContainer.Splines.Count) return;
        
        
        if (!_hasReachedSpline)
        {
            Vector3 startPointOnSpline;
            startPointOnSpline = ChooseStartPosBasedOnDistance(target, splineIndex);

            //= _splineContainer.EvaluatePosition(splineIndex,DistancePercentage);
            float dist = Vector3.Distance(target.position, startPointOnSpline);
            
            if (dist > _startDistanceThreshold)
            {
                target.position = Vector3.MoveTowards(
                    target.position,
                    startPointOnSpline,
                    _patrolSpeed * Time.deltaTime
                );
                
                var dir = startPointOnSpline - target.position;
                if (dir.sqrMagnitude > 0.0001f)
                    target.rotation = Quaternion.LookRotation(dir);
                
                return;
            }
            
            target.position = startPointOnSpline;
            _hasReachedSpline = true;
        }
        

        if (_splineLength <= 0f)
            _splineLength = _splineContainer.CalculateLength();

        
        DistancePercentage += _patrolSpeed * Time.deltaTime / _splineLength;
        
        if (DistancePercentage > 1f)
        {
            DistancePercentage = 1f;
            _patrolSpeed = -_patrolSpeed;   // reverse direction
        }

        if (DistancePercentage < 0f)
        {
            DistancePercentage = 0f;
            _patrolSpeed = -_patrolSpeed;  
        }

        Vector3 currentPosition = _splineContainer.EvaluatePosition(splineIndex,DistancePercentage);
        
        target.position = currentPosition;

        _tangent = _splineContainer.EvaluateTangent(splineIndex,DistancePercentage);

        if (_patrolSpeed < 0f)
            _tangent = -_tangent;

        if (_tangent != Vector3.zero)
        {
            target.rotation = Quaternion.LookRotation(_tangent);
        }
    }

    private Vector3 ChooseStartPosBasedOnDistance(Transform target, int splineIndex)
    {
        Vector3 startPointOnSpline;
        var start  = _splineContainer.EvaluatePosition(splineIndex, 0f);
        var middle = _splineContainer.EvaluatePosition(splineIndex, 0.5f);
        var end    = _splineContainer.EvaluatePosition(splineIndex, 1f);

        var dStart  = Vector3.Distance(target.position, start);
        var dMiddle = Vector3.Distance(target.position, middle);
        var dEnd    = Vector3.Distance(target.position, end);
            
        if (dStart < dMiddle && dStart < dEnd)
        {
            startPointOnSpline = start;
            DistancePercentage = 0f;
        }
        else if (dEnd < dStart && dEnd < dMiddle)
        {
            startPointOnSpline = end;
            DistancePercentage = 1f;
        }
        else
        {
            startPointOnSpline = middle;
            DistancePercentage = 0.5f;
        }

        return startPointOnSpline;
    }

    public void AddSplines(Spline spline)
    {
        _splineContainer.AddSpline(spline);
    }

    public void MoveInReverse()
    {
       // if (_patrolSpeed > 0f)
            _patrolSpeed = -_patrolSpeed;
    }

    public void MoveForward()
    {
        if (_patrolSpeed < 0f)
            _patrolSpeed = -_patrolSpeed;
    }


    public void CreateNewKnot(Vector3 target)
    {
        //Vector3 target = new Vector3(46.8400002f, 6.30000019f, -6.9000001f);
        Vector3 localPos = _splineContainer.transform.InverseTransformPoint(target);
        
        BezierKnot newKnot = new BezierKnot(localPos);
        _splineContainer.Spline.Add(newKnot);
        RefreshSplineLength();
    }

    public void ModifyExistingKnot(int knotIndex,Vector3 target)
    {
       // Vector3 target = new Vector3(46.8400002f, 6.30000019f, -6.9000001f);
        Vector3 localPos = _splineContainer.transform.InverseTransformPoint(target);

        var knot = _splineContainer.Spline[knotIndex];
        knot.Position = localPos;
        _splineContainer.Spline[knotIndex] = knot;
        RefreshSplineLength();
        
    }

    public void RemoveKnot(int knotIndex)
    {
        var knot = _splineContainer.Spline[knotIndex];
        _splineContainer.Spline.Remove(knot);
        RefreshSplineLength();
    }
    
    public int GetNextKnotIndex()
    {
        var spline = this._splineContainer.Spline;

        float knotParam = spline.ConvertIndexUnit(DistancePercentage, PathIndexUnit.Knot);
        
        int currentKnot = Mathf.FloorToInt(knotParam);
        
        int nextKnot = SplineUtility.NextIndex(spline, currentKnot);

        return nextKnot;
    }

    public void CreateNewSpline()
    {
        Spline newSpline = new Spline();
        _splineContainer.AddSpline(newSpline);
    }
    
    private void RefreshSplineLength()
    {
        _splineLength = _splineContainer.CalculateLength();
    }


    public bool HasReachedInitialPointOnSpline() => _hasReachedSpline;



}

