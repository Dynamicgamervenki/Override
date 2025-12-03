using UnityEngine;
using UnityEngine.Splines;

public class AerialEnemyBase : MonoBehaviour, IPatrol
{
    public EnemyState enemyState;

    private bool isPatroling = false;
    private SplinePatrol splinePatrol;

    [SerializeField] private float patrolSpeed = 10.0f;
    [SerializeField] private SplineContainer splineContainer;

    private void Start()
    {
        OnStart();
    }

    protected virtual void OnStart()
    {
        splinePatrol = new SplinePatrol(splineContainer, patrolSpeed);
    }

    private void Update()
    {
        if(enemyState == EnemyState.patroling)
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        splinePatrol.MoveAlongSpline(transform);
    }

    public void SetEnemyState(EnemyState enemyState) => this.enemyState = enemyState;


    #region interface
    public bool IsPatroling { get => isPatroling; set => IsPatroling = IsPatroling; }
    public float GetPatrolSpeed() => patrolSpeed;
    public SplineContainer GetSplineContainer() => splineContainer;
    #endregion
}

public enum EnemyState
{
    idle,
    patroling,
    combat,
    hacked
}

