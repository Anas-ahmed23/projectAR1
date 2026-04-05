using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// Places a prefab on detected AR surfaces when the user taps the screen.
/// Attach to an empty GameObject named "AR Placement Manager".
/// Assign the prefab to place and the AR Input Handler reference in the Inspector.
/// </summary>
public class ARPlacementManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject prefabToPlace;

    [Header("References")]
    [SerializeField] private ARInputHandler inputHandler;

    private ARRaycastManager _raycastManager;

    // A reusable list to store the results of each raycast hit.
    private static readonly List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    private void Awake()
    {
        _raycastManager = FindFirstObjectByType<ARRaycastManager>();

        if (_raycastManager == null)
        {
            Debug.LogWarning("ARPlacementManager: No ARRaycastManager found in the scene.");
        }
    }

    private void OnEnable()
    {
        if (inputHandler == null)
        {
            Debug.LogWarning("ARPlacementManager: inputHandler is not assigned in the Inspector.");
            return;
        }

        // 1. Start listening to the Input Handler.
        inputHandler.OnPerformTap += PlaceObject;
    }

    private void OnDisable()
    {
        if (inputHandler == null) return;

        // 2. Stop listening.
        inputHandler.OnPerformTap -= PlaceObject;
    }

    private void PlaceObject(Vector2 screenPos)
    {
        // 3. Shoot a ray from the screen position into the AR environment.
        //    TrackableType.PlaneWithinPolygon ensures we only hit actual
        //    detected floor/table geometry, not empty space.
        if (_raycastManager != null &&
            _raycastManager.Raycast(screenPos, s_Hits, TrackableType.PlaneWithinPolygon))
        {
            // 4. Raycast hits are sorted by distance; [0] is the closest surface.
            Pose hitPose = s_Hits[0].pose;

            // 5. Spawn the prefab at the real-world location and rotation.
            Instantiate(prefabToPlace, hitPose.position, hitPose.rotation);
        }
    }
}