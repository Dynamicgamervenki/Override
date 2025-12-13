using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;

public class AerialEnemyBase : MonoBehaviour, IPatrol
{


    [Header("Patrolling : ")]
    [SerializeField] private float patrolSpeed = 10.0f;
    [SerializeField] private SplineContainer splineContainer;

    [Header("Collision Detection : ")]
    [SerializeField] private Vector3 collisionOffset = Vector3.zero;
    [SerializeField] private float collisionRadius = 1f;
    [SerializeField] private float maxDiastance = 2.0f;
    
    [Header("Spline Junction Detection : ")]
    private bool junctionFound;
    [SerializeField] private  float junctionRadius = 10;
    [SerializeField] private  Vector3 junctionScanOffset = Vector3.zero;
    [SerializeField] private  LayerMask junctionLayer;
    [SerializeField] private  Color junctionColor = Color.grey;
    
    
    protected  SplineContainer NewsplineContainer;
    protected  float Direction;
    protected SplineJunction Junction;
    
    private float junctionPatrolActiveTime = 5.0f;
    private EnemyState _enemyState;
    private bool _isPatrolling = false;
    private SplinePatrol _splinePatrol;
    private bool _canPatrol;
    private bool _needToSwitchPatrolRoute;
    private Ray _ray;
    private  float currentDistance;
    private void Start()
    {
        OnStart();
    }

    protected virtual void OnStart()
    {
        _splinePatrol = new SplinePatrol(splineContainer, patrolSpeed);
    }

    private void Update()
    {
        OnUpdate();
    }
    protected virtual void OnUpdate()
    {
        if(_enemyState == EnemyState.Stunned || _enemyState == EnemyState.Hacked) return;
        
        if(_enemyState == EnemyState.Patrolling)
        {
            Patrol(splineIndex);
            Direction = _splinePatrol.GetDirection();
        }
        
        if(!NewsplineContainer) return;
        if ((!(currentDistance > 0.95f) || Direction != 1) && (!(currentDistance < 0.5f) || Direction != -1)) return;
        ChangeSplineContainer(NewsplineContainer);
        NewsplineContainer = null;
    }

    [SerializeField] private  int splineIndex = 0;
    
    
    private void Patrol(int splineIndex = 0)
    {
        _ray = new Ray(transform.position + collisionOffset, transform.forward);
        // _canPatrol = !Physics.SphereCast(ray, collisionRadius, maxDiastance);
        //     _splinePatrol.MoveAlongSpline(transform,splineIndex);
        //     currentDistance = _splinePatrol.GetCurrentDistancePercentage();
        RaycastHit hit;
        _canPatrol = !(Physics.SphereCast(_ray, collisionRadius, out hit, maxDiastance) &&
                       hit.collider.GetComponent<SplineJunction>() == null);

        if (_canPatrol)
        {
            _splinePatrol.MoveAlongSpline(transform,splineIndex);
        }
        else
        {
            if (!_splinePatrol.HasReachedInitialPointOnSpline())
            {
                _enemyState = EnemyState.Idle;
            }
            _splinePatrol.MoveInReverse();
            _splinePatrol.MoveAlongSpline(transform,splineIndex);
        }
    }

    public void Investigate(Vector3 target)
    {
        _enemyState = EnemyState.Investigaion;
        transform.DOMove(target, 2.0f, false);
        transform.LookAt(target);
        //transform.DORotate(target, .5f);
    }

    protected void SetEnemyState(EnemyState enemyState) => this._enemyState = enemyState;


    #region interface
    public bool IsPatrolling { get => _isPatrolling; set => IsPatrolling = IsPatrolling; }
    public float GetPatrolSpeed() => patrolSpeed;
    public SplineContainer GetSplineContainer() => splineContainer;
    #endregion
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Vector3 start = transform.position + collisionOffset;
        Vector3 end = start + transform.forward * maxDiastance;
        
        Gizmos.DrawWireSphere(start, collisionRadius);
        Gizmos.DrawWireSphere(end, collisionRadius);
        Gizmos.DrawLine(start, end);
    }

    public void ChangeSplineContainer(SplineContainer newsplineContainer)
    {
        splineContainer = newsplineContainer;
        _splinePatrol = new SplinePatrol(splineContainer, patrolSpeed);
        _splinePatrol.MoveAlongSpline(transform, splineIndex);
    }
    
    public void CheckForSplineJunction()
    {
        junctionFound = false;
        Junction = null;
        
        var hits = Physics.OverlapSphere(transform.position + junctionScanOffset, junctionRadius, junctionLayer);
        foreach (var hit in hits)
        {
            junctionFound = hit.TryGetComponent<SplineJunction>(out var junction) && junction.direction == Direction;
            if (!junctionFound) continue;
            this.Junction = junction;
            HackingInputRouter.Instance.TogglePatrolSelectionUi(true);
            StartCoroutine(HidePatrolSelectionUIAfterDelay(junctionPatrolActiveTime));
           // newsplineContainer = junction.GetRandomSplineContainer();
            break;
        }
        
        if (!junctionFound)
        {
            HackingInputRouter.Instance.TogglePatrolSelectionUi(false);
        }

    }
    
    
    private IEnumerator HidePatrolSelectionUIAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (HackingInputRouter.Instance != null)
        {
            HackingInputRouter.Instance.TogglePatrolSelectionUi(false);
        }
        junctionFound = false;
    }
    
    public EnemyState GetEnemyState() => _enemyState;

    private void OnDrawGizmos()
    {
        Gizmos.color = junctionColor;
        Gizmos.DrawWireSphere(transform.position + junctionScanOffset, junctionRadius);
    }
}



public enum EnemyState
{
    Idle,
    Patrolling,
    Combat,
    Hacked,
    Investigaion,
    Stunned
}

