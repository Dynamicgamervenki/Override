using UnityEngine;

public class ElectricBotAnimatro : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private ElectroBot electroBot;

    #region animatorParameters
    //private static readonly int RollHash = Animator.StringToHash("Roll_Anim");
    private static readonly int RollHash = Animator.StringToHash("Roll");
    #endregion

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (electroBot.enemyState == EnemyState.hacked)
        {
            animator.SetBool(RollHash, true);   
        }
    }
}
