using System;
using System.Collections.Generic;
using System.Linq;

namespace TFs.Common.Entities.Prefabs
{
    public class PrefabEntity
    {
        public struct PrefabTag: IEntityComponent {}
        public Entity Entity { get; }
        
        private readonly Dictionary<Type, object> _components = new();

        public PrefabEntity(Entity entity)
        {
            Entity = entity;
        }

        public void AddComponent<T>(T component) where T : struct
        {
            _components[typeof(T)] = component;
        }

        public bool TryGetComponent<T>(out T component) where T : struct
        {
            if (_components.TryGetValue(typeof(T), out var value))
            {
                component = (T)value;
                return true;
            }
            component = default;
            return false;
        }

        public Type[] GetComponentTypes()
        {
            return _components.Keys.ToArray();
        }
    }
}