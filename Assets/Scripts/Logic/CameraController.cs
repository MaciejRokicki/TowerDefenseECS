using TD.Common.InputManager.InputActionMaps;
using UnityEngine;

namespace TD.Logic
{
    public class CameraController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private Camera camera;

        [Header("Settings")]
        [SerializeField]
        private float movementSpeed;
        [SerializeField]
        private float zoomSpeed;
        [SerializeField]
        private float minZoom;
        [SerializeField]
        private float maxZoom;

        private Vector3 targetPosition;
        private float targetCameraSize;

        private void Update()
        {
            targetPosition += GameplayInputActionMap.Movement * movementSpeed * Time.deltaTime;
            targetPosition.z = -10.0f;

            targetCameraSize += GameplayInputActionMap.Zoom;
            targetCameraSize = Mathf.Clamp(targetCameraSize, minZoom, maxZoom);

            camera.transform.position = Vector3.Lerp(camera.transform.position, targetPosition, movementSpeed * Time.deltaTime);
            camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, targetCameraSize, zoomSpeed * Time.deltaTime);
        }
    }
}