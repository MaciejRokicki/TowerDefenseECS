using TD.Application;
using TD.Application.Input.ActionMaps;
using UnityEngine;

namespace TD.UI
{
    public class PauseMenuListener : MonoBehaviour
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