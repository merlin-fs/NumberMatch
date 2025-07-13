using System.Collections.Generic;
using Game.Configs;
using Game.UI;
using TFs.Common.Contexts;
using TFs.Common.Entities;

namespace Game.Core
{
    public class GameManager
    {
        public World World => _world;
        public Entity Field => _field;
        
        private World _world;
        private Entity _field;
        
        private GameFieldUiDebugView _gameFieldUiDebugView;
        
        public GameManager(Context context) 
        {
            _gameFieldUiDebugView = context.Resolve<GameFieldUiDebugView>();
            
            _world = new World("GameWorld");
            var config = new FieldGenerationConfig
            {
                Sum10PairsPerType = 18, // Кількість пар для кожного типу, які в сумі дають 10
                EqualPairsPerType = 2, // Кількість пар для кожного типу, які рівні між собою
                Seed = 0,
            };
            _field = _world.EntityManager.CreateEntity(typeof(Components.FieldComponent), typeof(Components.CellComponent), typeof(Components.DeckComponent));
            _world.EntityManager.UpdateComponent(_field, new Components.FieldComponent
            {
                Size = new Unity.Mathematics.int2(9, 0), // Розмір поля
            });

            var rules = new List<IMergeRule>();
            rules.Add(new ColumnMergeRule());
            rules.Add(new RowMergeRule());
            rules.Add(new DiagonalMergeRule());
            rules.Add(new WrapRowMergeRule());
            
            _world.SystemManager.AddSystem(new Systems.FieldGenerationSystem(config, _field), _world.EntityManager);
            _world.SystemManager.AddSystem(new Systems.CellMergeSystem(_field, rules), _world.EntityManager);
            _world.SystemManager.AddSystem(new Systems.ClearRowsSystem(_field), _world.EntityManager);
        }
        
        public void StartNewGame()
        {
            _world.EntityManager.GetBuffer<Components.CellComponent>(_field).Clear();
            _world.EntityManager.GetBuffer<Components.DeckComponent>(_field).Clear();
                
            var request = _world.EntityManager.CreateEntity<Components.AddNumbersRequestComponent>();
            _world.EntityManager.UpdateComponent(request, new Components.AddNumbersRequestComponent
            {
                NumbersCount = 35,
                Index = 0,
            });
        }

        public void AddPack()
        {
            var cells = _world.EntityManager.GetBuffer<Components.CellComponent>(_field);
            var request = _world.EntityManager.CreateEntity<Components.AddNumbersRequestComponent>();
            _world.EntityManager.UpdateComponent(request, new Components.AddNumbersRequestComponent
            {
                NumbersCount = cells.Length,
                Index = cells.Length,
            });
        }
    }
}