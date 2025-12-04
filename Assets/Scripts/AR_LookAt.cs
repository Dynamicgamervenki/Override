using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class AR_LookAt : MonoBehaviour
{
    private MultiAimConstraint multiAimConstraint;
    [SerializeField] private Player player;
    [SerializeField] private RigBuilder rigBuilder;
    [SerializeField] private Rig rig;

    private void Start()
    {
        player.hackableFound += Player_hackableFound;
        multiAimConstraint = GetComponent<MultiAimConstraint>();
    }

    private void Player_hackableFound(GameObject obj)
    {
        if(obj.TryGetComponent<IHackable>(out IHackable H))
        {
            H.OnHacked += H_OnHacked;
        }

        var data = multiAimConstraint.data;

        var sources = new WeightedTransformArray(0);
        sources.Add(new WeightedTransform(obj.transform, 1f));

        data.sourceObjects = sources;
        multiAimConstraint.data = data;

        multiAimConstraint.weight = 1f;
        if (!rigBuilder) return;
        DOTween.To(() => rig.weight, r => rig.weight = r, 1f, 0.3f);
        rigBuilder.Build();
    }

    private void H_OnHacked()
    {
        DOTween.To(() => rig.weight, r => rig.weight = r, 0f, 0.3f);

        var data = multiAimConstraint.data;
        data.sourceObjects = new WeightedTransformArray(0);
        multiAimConstraint.data = data;
    }

    private void OnDestroy()
    {
        if (!player) return;
        player.hackableFound -= Player_hackableFound;
    }


}
