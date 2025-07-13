using Game.Configs;
using Game.Core.Components;
using TFs.Common.Entities;
using Unity.Collections;

namespace Game.Core.Systems
{
    public class FieldGenerationSystem : ISystem
    {
        private FieldGenerationConfig _config;
        
        public FieldGenerationSystem(FieldGenerationConfig config)
        {
            _config = config;
        }
        
        public void OnCreate(EntityManager manager, SystemQuery query)
        {
            // Створюємо сутність поля
            var fieldEntity = manager.CreateEntity(typeof(GameFieldComponent), typeof(CellComponent));
            var field = new GameFieldComponent { Size = _config.Size };
            manager.UpdateComponent(fieldEntity, field);
            var cells = manager.GetBuffer<CellComponent>(fieldEntity.ID);

            // Генеруємо стартові рядки
            for (var y = 0; y < _config.StartRows; y++)
            {
                GenerateRow(field, cells, y);
            }            
        }

        private void GenerateRow(GameFieldComponent field, NativeList<CellComponent> cells, int row)
        {
            var width = _config.Size.x;
            var deckPos = _config.DeckPosition;
            cells.Capacity = width * row;  

            for (var x = 0; x < width; x++)
            {
                if (deckPos >= _config.Deck.Length) break; // deck закінчився — handle як треба
                var value = _config.Deck[deckPos++];
                
                cells.Add(new CellComponent
                {
                    Index = field.At(x, row),
                    Value = value,
                    IsRemoved = false
                });
            }
            _config.DeckPosition = deckPos; // Зберігаємо нову позицію
        }
        
        public void OnUpdate(EntityManager manager, NativeArray<Entity> entities, float deltaTime) {}
    }
}