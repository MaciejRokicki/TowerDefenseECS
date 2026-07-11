using TD.Common;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    private async void Start()
    {
        await SceneManager.LoadSceneAsync(Scenes.COMMON_SCENE_ID, LoadSceneMode.Additive);
        await SceneManager.LoadSceneAsync(Scenes.LOGIC_SCENE_ID, LoadSceneMode.Additive);
        await SceneManager.UnloadSceneAsync(Scenes.BOOTSTRAP_SCENE_ID);
    }
}
