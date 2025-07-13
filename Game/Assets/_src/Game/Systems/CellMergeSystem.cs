using System;
using System.Collections.Generic;
using System.Linq;
using Game.Core.Components;
using TFs.Common.Entities;
using Unity.Collections;

namespace Game.Core.Systems
{
    public class CellMergeSystem : ISystem
    {
        private Entity _fieldEntity;
        private readonly List<IMergeRule> _rules = new();
        private readonly Action _onUpdate; // Додатковий колбек для оновлення UI або інших систем

        public CellMergeSystem(Entity fieldEntity, IEnumerable<IMergeRule> rules, Action onUpdate)
        {
            _fieldEntity = fieldEntity;
            _rules.AddRange(rules);
            _onUpdate = onUpdate;
        }

        public void OnCreate(EntityManager manager, SystemQuery query)
        {
            query.AddComponentType<MergeRequestComponent>();
        }
        
        public void OnUpdate(EntityManager manager, NativeArray<Entity> entities, float deltaTime)
        {
            if (entities.Length == 0)
                return;
            
            var field = manager.GetComponent<FieldComponent>(_fieldEntity);
            var cells = manager.GetBuffer<CellComponent>(_fieldEntity);

            // Знаходимо всі запити на merge
            foreach (var reqEntity in entities)
            {
                var request = manager.GetComponent<MergeRequestComponent>(reqEntity);

                // Тепер просто використовуємо request.IndexA / IndexB як індекси у буфері:
                if (CanMerge(field, cells.AsArray(), request.IndexA, request.IndexB))
                {
                    ref var a = ref cells.ElementAt(request.IndexA);
                    ref var b = ref cells.ElementAt(request.IndexB);
                    a.IsRemoved = true;
                    b.IsRemoved = true;
                    // додаткова логіка (очки, анімація тощо)
                }
                
                manager.RemoveEntity(reqEntity); // Запит виконано
            }
            _onUpdate?.Invoke();
        }

        private bool CanMerge(FieldComponent field, NativeArray<CellComponent> cells, int idxA, int idxB)
        {
            return _rules.Any(rule => rule.CanMerge(field, cells, idxA, idxB));
        }
    }
}