using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;

namespace TFs.Common.Entities
{
    public class Archetype : IDisposable
    {
        private readonly Type[] _componentTypes;
        private NativeHashSet<int> _entities;
        private readonly Dictionary<Type, IComponentStorage> _componentStorages = new();
        private readonly Dictionary<Type, IBufferComponentStorage> _bufferStorages = new();
        private readonly Allocator _allocator;

        public Archetype(Type[] componentTypes, Allocator allocator)
        {
            _allocator = allocator;
            _componentTypes = componentTypes;
            _entities = new NativeHashSet<int>(100, _allocator);

            foreach (var type in componentTypes)
            {
                if (typeof(IEntityBufferComponent).IsAssignableFrom(type))
                {
                    var storageType = typeof(BufferComponentStorage<>).MakeGenericType(type);
                    _bufferStorages[type] = (IBufferComponentStorage)Activator.CreateInstance(storageType, Allocator.Persistent);
                }
                else
                {
                    var storageType = typeof(ComponentStorage<>).MakeGenericType(type);
                    _componentStorages[type] = (IComponentStorage)Activator.CreateInstance(storageType, Allocator.Persistent);
                }
            }
        }

        public void AddEntity(int entityId)
        {
            _entities.Add(entityId);
            foreach (var storage in _componentStorages.Values)
            {
                storage.Add(entityId);
            }

            foreach (var buffer in _bufferStorages.Values)
            {
                buffer.Add(entityId);
            }
        }

        public void RemoveEntity(int entityId)
        {
            _entities.Remove(entityId);

            foreach (var storage in _componentStorages.Values)
            {
                storage.Remove(entityId);
            }

            foreach (var buffer in _bufferStorages.Values)
            {
                buffer.Remove(entityId);
            }
        }        

        public void CopyComponents(int srcEntityId, int dstEntityId)
        {
            foreach (var storage in _componentStorages.Values)
            {
                storage.CopyComponent(srcEntityId, dstEntityId);
            }
            foreach (var buffer in _bufferStorages.Values)
            {
                buffer.CopyBuffer(srcEntityId, dstEntityId);
            }
        }
        
        public Type[] GetComponentTypesWithout(Type typeToRemove)
        {
            var newTypes = new List<Type>(_componentTypes);
            newTypes.Remove(typeToRemove);
            return newTypes.ToArray();
        }
        
        public Type[] GetComponentTypes() => _componentTypes;

        public bool HasComponent(Type componentType)
        {
            return _componentStorages.ContainsKey(componentType);
        }
        
        public NativeArray<Entity> GetEntities(Allocator allocator)
        {
            return _entities.ToNativeArray(allocator).Reinterpret<Entity>();
        }

        public Type[] GetComponentTypesWith(Type additionalType)
        {
            var newTypes = new List<Type>(_componentTypes) {additionalType};
            return newTypes.ToArray();
        }

        public bool ContainsEntity(int entityId) => _entities.Contains(entityId);

        public void AddComponent<T>(int entityId, T component) 
            where T : unmanaged, IEntityComponent
        {
            if (_componentStorages.TryGetValue(typeof(T), out var storage))
            {
                ((ComponentStorage<T>)storage).Add(entityId, component);
            }
            else
            {
                throw new InvalidOperationException($"Component type {typeof(T)} not found in this archetype.");
            }
        }

        public bool TryGetComponent<T>(int entityId, out T component) 
            where T : unmanaged, IEntityComponent
        {
            if (_componentStorages.TryGetValue(typeof(T), out var storage))
            {
                return ((ComponentStorage<T>)storage).TryGetComponent(entityId, out component);
            }
            component = default;
            return false;
        }

        public void UpdateComponent<T>(int entityId, T component) 
            where T : unmanaged, IEntityComponent
        {
            if (_componentStorages.TryGetValue(typeof(T), out var storage))
            {
                ((ComponentStorage<T>)storage).UpdateComponent(entityId, component);
            }
        }

        public void AddBuffer<T>(int entityId) 
            where T : unmanaged, IEntityBufferComponent
        {
            if (_bufferStorages.TryGetValue(typeof(T), out var storage))
            {
                ((BufferComponentStorage<T>)storage).Add(entityId);
            }
            else
            {
                throw new InvalidOperationException($"Buffer type {typeof(T)} not found in this archetype.");
            }
        }    
        
        public NativeList<T> GetBuffer<T>(int entityId) 
            where T : unmanaged, IEntityBufferComponent
        {
            if (_bufferStorages.TryGetValue(typeof(T), out var storage))
            {
                return ((BufferComponentStorage<T>)storage).GetBuffer(entityId);
            }

            throw new InvalidOperationException($"Buffer type {typeof(T)} not found in this archetype.");
        }
        
        public void Dispose()
        {
            foreach (var storage in _componentStorages.Values)
            {
                storage.Dispose();
            }

            _componentStorages.Clear();
        }
    }

    public class ArchetypeKey : IEquatable<ArchetypeKey>
    {
        private readonly Type[] _types;

        public ArchetypeKey(Type[] types)
        {
            _types = types;
            Array.Sort(_types, (a, b) => a.FullName.CompareTo(b.FullName)); // Упорядкування для коректного порівняння
        }

        public override bool Equals(object obj)
        {
            return obj is ArchetypeKey key && Equals(key);
        }

        public bool Equals(ArchetypeKey other)
        {
            return _types.Length == other._types.Length && _types.SequenceEqual(other._types);
        }

        public override int GetHashCode()
        {
            int hash = 17;
            foreach (var type in _types)
            {
                hash = hash * 31 + type.GetHashCode();
            }

            return hash;
        }
    }
}
