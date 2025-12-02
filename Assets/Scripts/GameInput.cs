using UnityEngine;
using System;

public class GameInput : MonoBehaviour
{
    InputSystem_Actions inputActions;

    #region Inputevents
    public event EventHandler OnMoveAction;
    #endregion 

    private void Start()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += Move_performed;
    }

    private void Move_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnMoveAction?.Invoke(obj, EventArgs.Empty);
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }
}
