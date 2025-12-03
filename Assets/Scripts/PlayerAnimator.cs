using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Player player;

    #region privateVariables
    private Animator animator;
    private Vector3 velocity;
    #endregion

    #region animatorParameters
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int MotionSpeedHash = Animator.StringToHash("MotionSpeed");
    private static readonly int CrouchHash = Animator.StringToHash("isCrouching");
    #endregion

    #region values
    private const float MaxSpeed = 6f;
    private const float SpeedSmoothTime = 5f;
    private const float MotionSpeedMultiplier = 1.2f;
    #endregion

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        UpdateIdleState();
        UpdateCrouchState();
    }

    private void UpdateIdleState()
    {
        Vector3 velocity = player.GetVelocity();
        float currentSpeed = animator.GetFloat(SpeedHash);

        if (velocity.sqrMagnitude < 0.0001f)
        {
            animator.SetFloat(SpeedHash, 0f);
            return;
        }
        float targetSpeed = Mathf.Clamp(velocity.magnitude, 0f, MaxSpeed);
        float newSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * SpeedSmoothTime);

        animator.SetFloat(SpeedHash, newSpeed);
        animator.SetFloat(MotionSpeedHash, MotionSpeedMultiplier);
    }

    private void UpdateCrouchState()
    {
        bool isCrouching = player.GetPlayerActionState() == ActionStates.Crouching;
        animator.SetBool(CrouchHash, isCrouching);
    }

}
