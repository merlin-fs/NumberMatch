using System;
using System.Collections.Generic;
using System.Linq;

namespace TFs.Common.Entities
{
    public class SystemQuery
    {
        private readonly HashSet<Type> _componentTypes = new();

        public void AddComponentType<T>() where T : unmanaged, IEntityComponent
        {
            _componentTypes.Add(typeof(T));
        }
        
        public void SetIgnore(bool value) => IsIgnore = value; 

        public bool IsIgnore { get; private set; }

        public bool Matches(EntityManager manager, Entity entity)
        {
            return _componentTypes.All(type => manager.HasComponent(entity, type));
        }

        public IEnumerable<Type> GetComponentTypes() => _componentTypes;
    }
}