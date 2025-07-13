using System.Collections.Generic;
using System.Linq;
using Game.Configs;
using Game.Core.Components;
using TFs.Common.Entities;
using Unity.Collections;
using Unity.Mathematics;

namespace Game.Core.Systems
{
    public class FieldGenerationSystem : ISystem
    {
        private readonly FieldGenerationConfig _config;
        private readonly Entity _fieldEntity; // Сутність поля, створюється один раз
        
        public FieldGenerationSystem(FieldGenerationConfig config, Entity fieldEntity)
        {
            _config = config;
            _fieldEntity = fieldEntity;
        }
        
        public void OnCreate(EntityManager manager, SystemQuery query)
        {
            query.AddComponentType<AddNumbersRequestComponent>();
        }

        public void OnUpdate(EntityManager manager, NativeArray<Entity> entities, float deltaTime)
        {
            if (entities.Length == 0) return;
            
            var field = manager.GetComponent<FieldComponent>(_fieldEntity);
            var cells = manager.GetBuffer<CellComponent>(_fieldEntity);
            var deck = manager.GetBuffer<DeckComponent>(_fieldEntity);
            var max = field.At(field.Size.x, field.Size.y);
            
            foreach (var entity in entities)
            {
                var request = manager.GetComponent<AddNumbersRequestComponent>(entity);
                AddNumbers(deck, cells, request.NumbersCount, request.Index);
                
                var lastIndex = request.Index + request.NumbersCount - 1;
                var neededRows = (lastIndex / field.Size.x) + 1;
                field.Size.y = math.max(field.Size.y, neededRows);
                manager.UpdateComponent(_fieldEntity, field);
                manager.RemoveEntity(entity);
            }
        }
        
        private void AddNumbers(NativeList<DeckComponent> desk, NativeList<CellComponent> cells, int numbersCount, int startIndex)
        {
            cells.Capacity += numbersCount; // Збільшуємо ємність буфера
            
            for (var i = 0; i < numbersCount; i++)
            {
                if (desk.Length == 0)
                {
                    var deck = BuildDeck(_config.PairCountPerType, _config.Seed).Reinterpret<DeckComponent>();
                    desk.AddRange(deck);
                    deck.Dispose();
                }
                
                var value = desk[0].Value;
                desk.RemoveAt(0);
                cells.Add(new CellComponent
                {
                    Index = startIndex + i,
                    Value = value,
                    IsRemoved = false,
                });
            }
        }
        
        private NativeArray<int> BuildDeck(int pairCountPerType, int seed)
        {
            // Будь-яка твоя deck-логіка (див. попередні приклади)
            var pairs = new List<(int, int)>
            {
                (1, 9), (2, 8), (3, 7), (4, 6), (5, 5),
                (1, 1), (2, 2), (3, 3), (4, 4), (5, 5), (6, 6), (7, 7), (8, 8), (9, 9)
            };

            var deck = new List<int>();
            for (var i = 0; i < pairCountPerType; i++)
            {
                foreach (var pair in pairs)
                {
                    deck.Add(pair.Item1);
                    deck.Add(pair.Item2);
                }
            }

            // Shuffle
            var rng = new System.Random(seed);
            return new NativeArray<int>(deck.OrderBy(x => rng.Next()).ToArray(), Allocator.Temp);
        }        
    }
}