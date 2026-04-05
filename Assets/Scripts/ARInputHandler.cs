using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles AR touch/tap input using Unity's New Input System.
/// Attach to an empty GameObject named "AR Input Handler" and assign
/// the Player/Tap action to the tapAction field in the Inspector.
/// </summary>
public class ARInputHandler : MonoBehaviour
{
    // 1. The event that other scripts can subscribe to for tap notifications.
    public event Action<Vector2> OnPerformTap;

    // 2. Drag the 'Player/Tap' Input Action reference here in the Inspector.
    [SerializeField] private InputActionReference tapAction;

    private void OnEnable()
    {
        if (tapAction == null || tapAction.action == null)
        {
            Debug.LogWarning("ARInputHandler: tapAction is not assigned in the Inspector.");
            return;
        }

        // 3. Subscribe to the 'started' phase (the moment the finger hits the screen).
        tapAction.action.started += OnTapTriggered;
        tapAction.action.Enable();
    }

    private void OnDisable()
    {
        if (tapAction == null || tapAction.action == null) return;

        // 4. Clean up to prevent memory leaks.
        tapAction.action.started -= OnTapTriggered;
        tapAction.action.Disable();
    }

    private void OnTapTriggered(InputAction.CallbackContext context)
    {
        // 5. Grab the coordinates of the pointer (Mouse or Touch).
        if (Pointer.current != null)
        {
            Vector2 screenPosition = Pointer.current.position.ReadValue();

            // 6. Broadcast the coordinates to any listening scripts.
            OnPerformTap?.Invoke(screenPosition);
        }
    }
}