using UnityEngine;

public class HackTargetSelector : MonoBehaviour
{
    public LineRenderer lineRenderer;
    private Player player;
   private GameObject target;

    private void Start()
    {
        player = GetComponent<Player>();
        player.hackableFound += Player_hackableFound;

        if(!lineRenderer) { Debug.LogError("Assing LineRenderer");}
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;
    }

    private void Update()
    {
        if (!target && lineRenderer.positionCount != 0) { ResetLineRenderer(); }
        UpdateLineRenderer();
    }

    private void Player_hackableFound(GameObject obj)
    {
       target = obj;
       UpdateLineRendere();
       if (obj.TryGetComponent<IHackable>(out IHackable h))
        {
            h.OnHacked += H_OnHacked;
        }
    }

    private void H_OnHacked()
    {
        ResetLineRenderer();
    }

    private void UpdateLineRendere()
    {
        lineRenderer.positionCount = 2;
    }

    private void ResetLineRenderer()
    {
        if (lineRenderer.positionCount == 0) return;

        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, Vector3.zero);
        lineRenderer.positionCount = 0;
    }

    private void UpdateLineRenderer()
    {
        if(!lineRenderer || !target || !player || lineRenderer.positionCount == 0) return;

        lineRenderer.SetPosition(0, player.transform.position + new Vector3(0,2.5f,0));
        lineRenderer.SetPosition(1, target.transform.position);
    }
}
