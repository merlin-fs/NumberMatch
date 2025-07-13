using System;
using Unity.Collections;

namespace TFs.Common.Entities
{
    public class World : IDisposable
    {
        public string Name { get; }
        public EntityManager EntityManager { get; }
        public SystemManager SystemManager { get; }

        public Allocator Allocator { get; }

        public World(string name, Allocator allocator = Allocator.Persistent)
        {
            Name = name;
            Allocator = allocator;
            EntityManager = new EntityManager(allocator);
            SystemManager = new SystemManager();
        }

        /// <summary>
        /// Додає систему до системного менеджера.
        /// </summary>
        public void AddSystem<T>(T system, UpdateType? updateType = null) where T : class, ISystem
        {
            SystemManager.AddSystem(system, EntityManager, updateType);
        }

        /// <summary>
        /// Видаляє систему із системного менеджера.
        /// </summary>
        public void RemoveSystem<T>() where T : class, ISystem
        {
            SystemManager.RemoveSystem<T>(EntityManager);
        }

        /// <summary>
        /// Викликає Update для всіх систем, зареєстрованих в World.
        /// </summary>
        public void Update(float deltaTime)
        {
            SystemManager.UpdateAll(EntityManager, deltaTime);
        }

        /// <summary>
        /// Викликає FixedUpdate для всіх систем, зареєстрованих в World.
        /// </summary>
        public void FixedUpdate(float deltaTime)
        {
            SystemManager.FixedUpdateAll(EntityManager, deltaTime);
        }

        /// <summary>
        /// Викликає LateUpdate для всіх систем, зареєстрованих в World.
        /// </summary>
        public void LateUpdate(float deltaTime)
        {
            SystemManager.LateUpdateAll(EntityManager, deltaTime);
        }

        public void Dispose()
        {
            SystemManager.Dispose(EntityManager);
            EntityManager.Dispose();
        }
    }   
}