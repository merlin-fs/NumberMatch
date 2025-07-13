using Game.Core;
using TFs.Common.Contexts;
using TFs.Common.Entities;

namespace Game.Common.Contexts
{
    public class GameContextInitializer : SceneContextInitializer
    {
        public override void InstallBindings(IContextBinding contextBinding)
        {
            var gameManager = new GameManager(contextBinding.Context);
            contextBinding.Bind<GameManager>(gameManager);
            contextBinding.Bind<World>(gameManager.World);
        }
    }
}
