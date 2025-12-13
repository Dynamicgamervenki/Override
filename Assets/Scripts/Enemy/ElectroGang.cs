
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

public class ElectroGang : MonoBehaviour
{
    [SerializeField] private SplineContainer splineContainer;
    private Spline _spline;

    [SerializeField] private GameObject electroBot;
    [SerializeField] private float patrolSpeed = 2f;

    private void Start()
    {
        // allow assigning in inspector or fallback to nearby components
        if (splineContainer == null)
            splineContainer = GetComponent<SplineContainer>() 
                             ?? GetComponentInParent<SplineContainer>()
                             ?? GetComponentInChildren<SplineContainer>();

        if (splineContainer == null)
        {
            Debug.LogWarning("[ElectroGang] No SplineContainer found. Assign one in the inspector or attach to the same GameObject.");
            return;
        }

        _spline = splineContainer.Spline;
        if (_spline == null)
        {
            Debug.LogWarning("[ElectroGang] SplineContainer has no Spline.");
            return;
        }

        if (electroBot == null)
        {
            Debug.LogWarning("[ElectroGang] electroBot prefab not assigned.");
            return;
        }

        SpawnBots();
    }

    private void SpawnBots()
    {
        var knots = _spline.Knots.ToList();
        int n = knots.Count;
        if (n <= 0) return;

        for (int i = 0; i < n; i++)
        {
            float startPercent = (float)i / n; // evenly spaced around closed spline
            // evaluate world position on spline for exact placement
            Vector3 worldPos = splineContainer.transform.TransformPoint(knots[i].Position);

            var botGO = Instantiate(electroBot, worldPos, Quaternion.identity);
            if (botGO == null)
            {
                Debug.LogWarning($"[ElectroGang] Failed to instantiate bot prefab at index {i}.");
                continue;
            }

            var bot = botGO.GetComponent<ElectroBot>();
            if (bot == null)
            {
                Debug.LogWarning($"[ElectroGang] Instantiated prefab missing ElectrobBot component at index {i}.");
                continue;
            }

            // ensure bot is exactly on the spline and initialized with the correct container/speed
            Vector3 precisePos = splineContainer.EvaluatePosition(0, startPercent);
            bot.transform.position = precisePos;

            bot.Initialize(splineContainer, startPercent, patrolSpeed, 0);
        }
    }
}
