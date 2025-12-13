using UnityEngine;

public class ElectricBotAnimator : MonoBehaviour
{
    private Animator _animator;
    [SerializeField] private ElectroBot electroBot;

    #region animatorParameters
    //private static readonly int RollHash = Animator.StringToHash("Roll_Anim");
    private static readonly int RollHash = Animator.StringToHash("Roll");
    #endregion

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (electroBot.GetEnemyState() == EnemyState.Hacked)
        {
            _animator.SetBool(RollHash, true);   
        }
    }
}
