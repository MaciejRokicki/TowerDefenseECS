using TD.Core.Input.ActionMaps;
using UnityEngine;
using VContainer;

namespace TD.Features.CameraController
{
    public class CameraController : MonoBehaviour
    {
        private GameplayInputActionMap gameplayInputActionMap;

        [Header("References")]
        [SerializeField]
        private Camera camera;

        [Header("Settings - Movement")]
        [SerializeField]
        private AnimationCurve movementSpeedCurve;
        [SerializeField]
        private AnimationCurve swipeMovementSpeedCurve;
        [SerializeField]
        private Vector2 blCorner;
        [SerializeField]
        private Vector2 trCorner;
        [Header("Settings - Zoom")]
        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float startZoom;
        [SerializeField]
        private float zoomSpeed;
        [SerializeField]
        private float minZoom;
        [SerializeField]
        private float maxZoom;

        private Vector3 targetPosition;
        private float targetZoom;
        private float zoomPercentage;

        private Vector2 blViewport;
        private Vector2 trViewport;

        [Inject]
        private void Construct(GameplayInputActionMap gameplayInputActionMap)
        {
            this.gameplayInputActionMap = gameplayInputActionMap;
        }

        private void Awake()
        {
            camera.orthographicSize = targetZoom = startZoom * (maxZoom - minZoom) + minZoom;
        }

        private void Update()
        {
            if (gameplayInputActionMap.IsSwiping)
            {
                targetPosition -= gameplayInputActionMap.SwipeMovement * swipeMovementSpeedCurve.Evaluate(zoomPercentage) * Time.deltaTime;
            }
            else
            {
                targetPosition += gameplayInputActionMap.Movement * movementSpeedCurve.Evaluate(zoomPercentage) * Time.deltaTime;
            }

            targetPosition.z = -100.0f;

            targetZoom += gameplayInputActionMap.Zoom;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);

            targetPosition = ClampCamera(targetPosition);

            camera.transform.position = Vector3.Lerp(camera.transform.position, targetPosition, movementSpeedCurve.Evaluate(zoomPercentage) * Time.deltaTime);
            camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, targetZoom, zoomSpeed * Time.deltaTime);
            zoomPercentage = (targetZoom - minZoom) / (maxZoom - minZoom);
        }

        private Vector3 ClampCamera(Vector3 targetPosition)
        {
            float width = camera.orthographicSize * camera.aspect;
            blViewport = trViewport = camera.transform.position;
            blViewport.x -= width;
            blViewport.y -= camera.orthographicSize;
            trViewport.x += width;
            trViewport.y += camera.orthographicSize;

            targetPosition.x = Mathf.Clamp(targetPosition.x, blCorner.x + width, trCorner.x - width);
            targetPosition.y = Mathf.Clamp(targetPosition.y, blCorner.y + camera.orthographicSize, trCorner.y - camera.orthographicSize);

            return targetPosition;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawCube(blCorner, Vector3.one);
            Gizmos.DrawCube(trCorner, Vector3.one);
        }
    }
}