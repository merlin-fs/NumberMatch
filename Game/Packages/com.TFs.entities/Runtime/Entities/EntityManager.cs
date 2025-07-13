using System;
using System.Collections.Generic;
using Unity.Collections;

namespace TFs.Common.Entities
{
    public partial class EntityManager : IDisposable
    {
        private int _nextEntityId = 1;
        private readonly Allocator _allocator;

        // Архетипи: ключ - набір компонентів
        private readonly Dictionary<ArchetypeKey, Archetype> _archetypes = new();

        public EntityManager(Allocator allocator)
        {
            _allocator = allocator;
        }
        
        // Створює одну сутність
        public Entity CreateEntity<TComponent>() where TComponent : unmanaged, IEntityComponent
        {
            return CreateEntity(typeof(TComponent));
        }

        // Створює одну сутність
        public Entity CreateEntityBuffer<TBuffer>() where TBuffer : unmanaged, IEntityBufferComponent
        {
            return CreateEntity(typeof(TBuffer));
        }
        
        // Створює одну сутність
        public Entity CreateEntity(params Type[] componentTypes)
        {
            var archetype = GetOrCreateArchetype(componentTypes);
            var entity = new Entity{ID = _nextEntityId++};
            archetype.AddEntity(entity.ID);
            return entity;
        }

        // Створює декілька сутностей
        public void CreateEntities(NativeArray<Entity> entities, params Type[] componentTypes)
        {
            var archetype = GetOrCreateArchetype(componentTypes);
            var intEntities = entities.Reinterpret<int>();

            for (var i = 0; i < entities.Length; i++)
            {
                var entityId = _nextEntityId++;
                archetype.AddEntity(entityId);
                intEntities[i] = entityId;
            }
        }

        public Entity CloneEntity(Entity src)
        {
            var archetype = FindEntityArchetype(src.ID);
            if (archetype == null)
            {
                return Entity.Null;
            }

            var newEntity = CreateEntity(archetype.GetComponentTypes());
            archetype.CopyComponents(src.ID, newEntity.ID);
            return newEntity;
        }
        
        public void RemoveEntity(Entity entity)
        {
            var archetype = FindEntityArchetype(entity.ID);
            if (archetype == null)
            {
                throw new InvalidOperationException($"Entity {entity} not found in any archetype.");
            }
            archetype.RemoveEntity(entity.ID);
        }
        
        // Додає компонент до групи сутностей
        public void AddComponent<T>(NativeArray<Entity> entities, T component) where T : unmanaged, IEntityComponent
        {
            foreach (var entity in entities)
            {
                AddComponent(entity, component);
            }
        }

        // Видаляє компонент з групи сутностей
        public void RemoveComponent<T>(NativeArray<Entity> entities) where T : unmanaged, IEntityComponent
        {
            foreach (var entity in entities)
            {
                RemoveComponent<T>(entity);
            }
        }

        // Додає компонент до однієї сутності
        public void AddComponent<T>(Entity entity, T component) where T : unmanaged, IEntityComponent
        {
            var archetype = FindEntityArchetype(entity.ID);
            if (archetype == null)
            {
                throw new InvalidOperationException($"Entity {entity} not found in any archetype.");
            }

            var newTypes = archetype.GetComponentTypesWith(typeof(T));
            var newArchetype = GetOrCreateArchetype(newTypes);

            archetype.RemoveEntity(entity.ID);
            newArchetype.AddEntity(entity.ID);
            newArchetype.UpdateComponent(entity.ID, component);
        }

        // Видаляє компонент із однієї сутності
        public void RemoveComponent<T>(Entity entity) where T : unmanaged, IEntityComponent
        {
            var archetype = FindEntityArchetype(entity.ID);
            if (archetype == null)
            {
                throw new InvalidOperationException($"Entity {entity} not found in any archetype.");
            }

            var newTypes = archetype.GetComponentTypesWithout(typeof(T));
            var newArchetype = GetOrCreateArchetype(newTypes);

            archetype.RemoveEntity(entity.ID);
            newArchetype.AddEntity(entity.ID);
        }

        
        public void AddBuffer<T>(Entity entity) 
            where T : unmanaged, IEntityBufferComponent
        {
            var archetype = FindEntityArchetype(entity.ID);
            if (archetype == null)
            {
                throw new InvalidOperationException($"Entity {entity.ID} not found in any archetype.");
            }
            archetype.AddBuffer<T>(entity.ID);
        }

        public NativeList<T> GetBuffer<T>(Entity entity) 
            where T : unmanaged, IEntityBufferComponent
        {
            var archetype = FindEntityArchetype(entity.ID);
            if (archetype == null)
            {
                throw new InvalidOperationException($"Entity {entity.ID} not found in any archetype.");
            }
            return archetype.GetBuffer<T>(entity.ID);
        }        
        
        // Отримує компонент для сутності
        public T GetComponent<T>(Entity entity) 
            where T : unmanaged, IEntityComponent
        {
            var archetype = FindEntityArchetype(entity.ID);
            if (!archetype.TryGetComponent<T>(entity.ID, out var component))
                throw new InvalidOperationException($"Entity {entity} not found in any archetype.");
            return component;
        }


        // Отримує компонент для сутності
        public bool TryGetComponent<T>(Entity entity, out T component) where T : unmanaged, IEntityComponent
        {
            var archetype = FindEntityArchetype(entity.ID);
            return archetype?.TryGetComponent(entity.ID, out component) ??
                   throw new InvalidOperationException($"Entity {entity} not found in any archetype.");
        }

        public void UpdateComponent<T>(Entity entity, T component)
            where T : unmanaged, IEntityComponent
        {
            var archetype = FindEntityArchetype(entity.ID);
            archetype.UpdateComponent(entity.ID, component);
        }

        public bool HasComponent<T>(Entity entity)
            where T : unmanaged, IEntityComponent
        {
            return HasComponent(entity, typeof(T));
        }

        public bool HasComponent(Entity entity, Type componentType)
        {
            var archetype = FindEntityArchetype(entity.ID);
            return archetype?.HasComponent(componentType) ?? false;
        }
       
        // Створює або отримує архетип за списком компонентів
        private Archetype GetOrCreateArchetype(Type[] componentTypes)
        {
            var key = new ArchetypeKey(componentTypes);
            if (!_archetypes.TryGetValue(key, out var archetype))
            {
                archetype = new Archetype(componentTypes, _allocator);
                _archetypes[key] = archetype;
            }
            return archetype;
        }

        // Знаходить архетип, що містить сутність
        private Archetype FindEntityArchetype(int entityId)
        {
            foreach (var archetype in _archetypes.Values)
            {
                if (archetype.ContainsEntity(entityId))
                    return archetype;
            }
            return null;
        }

        public IEnumerable<Archetype> GetArchetypes() => _archetypes.Values;

        public void Dispose()
        {
            foreach (var archetype in _archetypes.Values)
            {
                archetype.Dispose();
            }
            _archetypes.Clear();
        }
    }
}
