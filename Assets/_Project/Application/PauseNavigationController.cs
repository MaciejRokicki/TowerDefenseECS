using TD.Application.Input.ActionMaps;
using UnityEngine;

namespace TD.Application
{
    public class PauseNavigationController : MonoBehaviour
    {
        private void Start()
        {
            GameplayInputActionMap.OnPauseMenuPressed += GameplayInputActionMap_OnPauseMenuPressed;
        }

        private void OnDestroy()
        {
            GameplayInputActionMap.OnPauseMenuPressed -= GameplayInputActionMap_OnPauseMenuPressed;
        }

        private void GameplayInputActionMap_OnPauseMenuPressed()
        {
            OpenPauseMenuOverlayUseCase.Instance.Execute();
        }
    }
}