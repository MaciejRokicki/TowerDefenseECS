using TD.Common.InputManager.InputActionMaps;
using UnityEngine;

namespace TD.Logic
{
    public class CameraController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private Camera camera;
        [SerializeField]
        private Transform targetTransform;

        [Header("Settings")]
        [SerializeField]
        private float movementSpeed;
        [SerializeField]
        private float zoomSpeed;
        [SerializeField]
        private float minZoom;
        [SerializeField]
        private float maxZoom;

        private float targetCameraSize;

        private void Update()
        {
            targetTransform.position += GameplayInputActionMap.Movement * movementSpeed * Time.deltaTime;

            targetCameraSize += GameplayInputActionMap.Zoom;
            targetCameraSize = Mathf.Clamp(targetCameraSize, minZoom, maxZoom);

            camera.transform.position = Vector3.Lerp(camera.transform.position, targetTransform.position, movementSpeed * Time.deltaTime);
            camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, targetCameraSize, zoomSpeed * Time.deltaTime);
        }
    }
}