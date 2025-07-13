using System;
using Unity.Collections;

namespace TFs.Common.Entities
{
    public interface IComponentStorage : IDisposable
    {
        void Add(int entityId);
        void Remove(int entityId);
        void CopyComponent(int srcEntityId, int dstEntityId);
    }
    
    
    public class ComponentStorage<T> : IComponentStorage where T : unmanaged
    {
        private NativeList<T> _components; // Масив компонентів
        private NativeList<int> _entityIds; // Відповідні ID сутностей
        private NativeHashMap<int, int> _indexMap; // Відображення EntityID -> Index

        public ComponentStorage(Allocator allocator)
        {
            _components = new NativeList<T>(allocator);
            _entityIds = new NativeList<int>(allocator);
            _indexMap = new NativeHashMap<int, int>(100, allocator);
        }

        void IComponentStorage.Add(int entityId) => Add(entityId);

        public void Add(int entityId, T component = default)
        {
            if (_indexMap.ContainsKey(entityId))
            {
                throw new InvalidOperationException($"Entity.ID {entityId} already exists in the storage.");
            }
            _entityIds.Add(entityId);
            _components.Add(component);
            _indexMap[entityId] = _components.Length - 1; // Map entityId to last index
        }

        public bool TryGetComponent(int entityId, out T component)
        {
            component = default;
            if (!_indexMap.TryGetValue(entityId, out var index))
                return false;
            component = _components[index];
            return true;
        }

        public void UpdateComponent(int entityId, T component)
        {
            if (!_indexMap.TryGetValue(entityId, out var index))
                throw new InvalidOperationException($"Entity {entityId} not found in storage.");

            _components[index] = component;
        }
        
        public void Remove(int entityId)
        {
            if (!_indexMap.TryGetValue(entityId, out var index)) return; // Entity not in storage

            // Remove entity and component
            var lastIndex = _components.Length - 1;
            if (index != lastIndex)
            {
                // Переміщуємо останній компонент на місце видаленого
                _components[index] = _components[lastIndex];
                _entityIds[index] = _entityIds[lastIndex];
                _indexMap[_entityIds[index]] = index;
            }

            _components.RemoveAt(lastIndex);
            _entityIds.RemoveAt(lastIndex);
            _indexMap.Remove(entityId);
        }

        public void CopyComponent(int srcEntityId, int dstEntityId)
        {
            if (!_indexMap.TryGetValue(srcEntityId, out var index)) return; // Entity not in storage
            Add(dstEntityId, _components[index]);
        }

        public void Dispose()
        {
            _components.Dispose();
            _entityIds.Dispose();
            _indexMap.Dispose();
        }
    }
}