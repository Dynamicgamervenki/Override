using DG.Tweening;
using UnityEngine;

public class Door : MonoBehaviour
{
    bool isDoorOpen = false;

    public void ToggleDoor()
    {
        if ((isDoorOpen = !isDoorOpen)) OpenDoor();
        else CloseDoor();
    }


    private void OpenDoor() => transform.DORotate(new Vector3(0, -90, 0), 1f, RotateMode.Fast);

    private void CloseDoor() => transform.DORotate(new Vector3(0, 0, 0), 1f, RotateMode.Fast);
}
