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
        if (!target)
        {
            lineRenderer.SetPosition(0,Vector3.zero);
            lineRenderer.SetPosition(1,Vector3.zero);
            return;
        }
        UpdateLineRenderer();
    }

    private void Player_hackableFound(GameObject obj)
    {
       target = obj;
    }

    private void UpdateLineRenderer()
    {
        if(!lineRenderer) return;

        lineRenderer.SetPosition(0, player.transform.position + new Vector3(0,2.5f,0));
        lineRenderer.SetPosition(1, target.transform.position);
    }
}
