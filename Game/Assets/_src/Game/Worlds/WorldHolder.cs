using TFs.Common.Entities;
using UnityEngine;

namespace Game.Core.Worlds
{
    public class WorldHolder : MonoBehaviour
    {
        private World _world;
        
        void Start()
        {
            _world = new World("test");
        }

        void Update()
        {

        }
    }
}