using UnityEngine;


public class Lasers : MonoBehaviour
{
    private void Start()
    {
        TurnOnLasers();
    }
    
    public void TurnOfLasersAfterDelay(float delay)
    {
        Invoke(nameof(TurnOffLasers),delay);
    }

    public void TurnOnLasersAfterDelay(float delay)
    {
        Invoke(nameof(TurnOnLasers),delay);
    }
    
    private void TurnOffLasers() => gameObject.SetActive(false);
    private void TurnOnLasers() =>gameObject.SetActive(true);

    
}
