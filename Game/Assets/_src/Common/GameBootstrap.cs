using TFs.Common.Contexts;
using UnityEngine;

namespace Game.Common
{
    public class GameBootstrap : MonoBehaviour
    {
        private void Awake()
        {
            QualitySettings.vSyncCount = 0; // вимкнути VSync
            Application.targetFrameRate = 120; // намагатися досягти 120 fps

            Context.CreateProjectContext("GameScene");//SceneType.Loading.GetSceneName()
        }
    }
}

