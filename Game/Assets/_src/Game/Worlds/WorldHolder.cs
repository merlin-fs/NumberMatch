using System.Threading.Tasks;
using TFs.Common.Contexts;
using TFs.Common.Entities;
using UnityEngine;

namespace Game.Core.Worlds
{
    public class WorldHolder : MonoBehaviour, IInitializable
    {
        private World _world;

        public Task Initialize(Context context)
        {
            _world = context.Resolve<World>();
            return Task.CompletedTask;
        }

        private void Update()
        {
            _world?.Update(Time.deltaTime);
        }
    }
}