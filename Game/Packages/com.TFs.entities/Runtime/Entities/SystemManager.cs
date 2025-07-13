using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;

namespace TFs.Common.Entities
{
    public class SystemManager : IDisposable
    {
        private readonly Dictionary<Type, SystemInfo> _systems = new();

        private class SystemInfo
        {
            public ISystem System;
            public UpdateType UpdateType;
            public SystemQuery Query;
        }
        
        public bool HasSystem<T>()
        {
            return _systems.ContainsKey(typeof(T));
        }

        public T GetSystem<T>()
        {
            return _systems[typeof(T)].System is T system ? system : default;
        }
        
        public void AddSystem<T>(T system, EntityManager manager, UpdateType? overrideUpdateType = null)
            where T : ISystem
        {
            var type = typeof(T);

            if (_systems.ContainsKey(type))
            {
                throw new InvalidOperationException($"System of type {type} is already registered.");
            }

            // Якщо UpdateType не вказаний, отримуємо його з атрибуту
            var updateType = overrideUpdateType ?? GetUpdateType(type);

            SystemInfo systemInfo;
            _systems[type] = systemInfo = new SystemInfo
            {
                UpdateType = updateType,
                System = system,
                Query = new SystemQuery()
            };
            system.OnCreate(manager, systemInfo.Query);
        }

        public void RemoveSystem<T>(EntityManager manager) where T : ISystem
        {
            var type = typeof(T);
            if (!_systems.TryGetValue(type, out var systemInfo)) return;

            DestroySystem(systemInfo.System, manager);
            _systems.Remove(type);
        }

        [BurstCompile]
        private NativeList<Entity> BuildMatchingEntities(Allocator allocator, EntityManager manager, SystemInfo systemInfo)
        {
            var matchingEntities = new NativeList<Entity>(allocator);
            foreach (var archetype in manager.GetArchetypes())
            {
                using var entities = archetype.GetEntities(Allocator.Temp);
                foreach (var entity in entities)
                {
                    if (systemInfo.Query.Matches(manager, entity))
                    {
                        matchingEntities.Add(entity);
                    }
                }
            }                   
            return matchingEntities;
        }
        
        public void UpdateAll(EntityManager manager, float deltaTime)
        {
            foreach (var systemInfo in _systems.Values)
            {
                if (systemInfo.UpdateType != UpdateType.Update)
                    continue;
                var matchingEntities = systemInfo.Query.IsIgnore
                    ? default
                    : BuildMatchingEntities(Allocator.Temp, manager, systemInfo).AsArray();
                systemInfo.System.OnUpdate(manager, matchingEntities, deltaTime);
                if (matchingEntities.IsCreated) matchingEntities.Dispose();
            }
        }

        public void FixedUpdateAll(EntityManager manager, float deltaTime)
        {
            foreach (var systemInfo in _systems.Values)
            {
                if (systemInfo.UpdateType != UpdateType.FixedUpdate)
                    continue;
                var matchingEntities = systemInfo.Query.IsIgnore
                    ? default
                    : BuildMatchingEntities(Allocator.Temp, manager, systemInfo).AsArray();
                systemInfo.System.OnUpdate(manager, matchingEntities, deltaTime);
                if (matchingEntities.IsCreated) matchingEntities.Dispose();
            }
        }

        public void LateUpdateAll(EntityManager manager, float deltaTime)
        {
            foreach (var systemInfo in _systems.Values)
            {
                if (systemInfo.UpdateType != UpdateType.LateUpdate)
                    continue;
                var matchingEntities = systemInfo.Query.IsIgnore
                    ? default
                    : BuildMatchingEntities(Allocator.Temp, manager, systemInfo).AsArray();
                systemInfo.System.OnUpdate(manager, matchingEntities, deltaTime);
                if (matchingEntities.IsCreated) matchingEntities.Dispose();
            }
        }

        private static void DestroySystem(ISystem system, EntityManager manager)
        {
            system.OnDestroy(manager);
            if (system is IDisposable disposable)
                disposable.Dispose();
        }
        
        private static UpdateType GetUpdateType(Type type)
        {
            var attribute = Attribute.GetCustomAttribute(type, typeof(UpdateMethodAttribute)) as UpdateMethodAttribute;
            return attribute?.UpdateType ?? UpdateType.Update; // За замовчуванням Update
        }

        public void Dispose(EntityManager manager)
        {
            foreach (var systemInfo in _systems.Values)
                DestroySystem(systemInfo.System, manager);
            _systems.Clear();
        }
        
        public void Dispose()
        {
            _systems.Clear();
        }        
    }
}