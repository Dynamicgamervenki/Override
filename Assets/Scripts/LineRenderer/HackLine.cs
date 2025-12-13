using UnityEngine;

public class HackLine : MonoBehaviour
{
    private LineRenderer _lr;
    private Transform _start;
    private Transform _end;

    public void Initialize(Transform start, Transform end)
    {
        _start = start;
        _end = end;

        _lr = GetComponent<LineRenderer>();
        _lr.positionCount = 2;
        _lr.useWorldSpace = true;
    }

    public void ChangePoints(Transform start, Transform end)
    {
        if (_lr.positionCount == 0) { _lr.positionCount = 2; }

        gameObject.transform.SetParent(start);
        this._start = start;
        this._end = end;
    }

    public void ResetLine()
    {
        if (!_lr ||_lr.positionCount == 0) return;

        _lr.SetPosition(0, Vector3.zero);
        _lr.SetPosition(1, Vector3.zero);
        _lr.positionCount = 0;
    }
    
    public float GetDistance()
    {
        if(_lr.positionCount == 0) return 0;
        
        Vector3 a = _lr.GetPosition(0);
        Vector3 b = _lr.GetPosition(1);

        return Vector3.Distance(a, b);
    }


    void Update()
    {
        if (!_start || !_end || _lr.positionCount == 0) return;

        _lr.SetPosition(0, _start.position);
        _lr.SetPosition(1, _end.position);
    }

    public void DestroyLine()
    {
        ResetLine();
        Destroy(gameObject);
    }
    
}
