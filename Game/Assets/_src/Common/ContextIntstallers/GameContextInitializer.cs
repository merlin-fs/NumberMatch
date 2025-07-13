using Game.Core;
using Game.UI;
using TFs.Common.Contexts;
using TFs.Common.Entities;
using UnityEngine;

namespace Game.Common.Contexts
{
    public class GameContextInitializer : SceneContextInitializer
    {
        [SerializeField] private GameFieldUiDebugView gameFieldUiDebugView;
        
        public override void InstallBindings(IContextBinding contextBinding)
        {
            contextBinding.Bind<GameFieldUiDebugView>(gameFieldUiDebugView);
            var gameManager = new GameManager(contextBinding.Context);
            contextBinding.Bind<GameManager>(gameManager);
            contextBinding.Bind<World>(gameManager.World);
        }
    }
}
