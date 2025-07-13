using Game.Configs;
using TFs.Common.Contexts;
using TFs.Common.Entities;

namespace Game.Core
{
    public class GameManager
    {
        public World World => _world;
        
        private World _world;
        private Entity _field;
        
        public GameManager(Context context) 
        {
            _world = new World("GameWorld");
            var config = new FieldGenerationConfig
            {
                PairCountPerType = 3, // Кількість пар для кожного типу
                Seed = 43,
            };
            _field = _world.EntityManager.CreateEntity(typeof(Components.FieldComponent), typeof(Components.CellComponent), typeof(Components.DeckComponent));
            _world.EntityManager.UpdateComponent(_field, new Components.FieldComponent
            {
                Size = new Unity.Mathematics.int2(9, 0), // Розмір поля
            });
            _world.SystemManager.AddSystem(new Systems.FieldGenerationSystem(config, _field), _world.EntityManager);
            
            StartNewGame();
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
    }
}