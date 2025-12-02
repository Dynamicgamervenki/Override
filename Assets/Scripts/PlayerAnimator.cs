using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private const string speed = "Speed";
    [SerializeField] private Player player;
    private Animator animator;
    private Vector3 velocity;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        velocity = player.GetVelocity();
        if (velocity == Vector3.zero)
        {
            animator.SetFloat(speed, 0);
        }
        else
        {
            velocity = player.GetVelocity();
            float targetSpeed = velocity.magnitude;
            float maxSpeed = 6f;
            targetSpeed = Mathf.Clamp(targetSpeed, 0f, maxSpeed);
            float smoothSpeed = Mathf.Lerp(animator.GetFloat(speed), targetSpeed, Time.deltaTime * 5f);
            animator.SetFloat("Speed", smoothSpeed);
  

            float motionMultiplier = 1.2f; 
            animator.SetFloat("MotionSpeed", motionMultiplier);

        }
    }
}
