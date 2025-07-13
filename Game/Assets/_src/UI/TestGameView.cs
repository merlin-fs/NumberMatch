using System.Threading.Tasks;
using Game.Core;
using TFs.Common.Contexts;
using TFs.Common.Entities;
using TFs.Common.Observables;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class TestGameView : MonoBehaviour, IInitializable
    {
        [SerializeField] private Button buttonStartNewGame;
        [SerializeField] private Button buttonAddPack;
        
        private GameManager _gameManager;
        private GameFieldUiDebugView _gameFieldUiDebugView;
        private World _world;
        
        private CompositeDisposable _disposables;
        
        public Task Initialize(Context context)
        {
            _gameManager = context.Resolve<GameManager>();
            _gameFieldUiDebugView = context.Resolve<GameFieldUiDebugView>();
            _gameFieldUiDebugView.OnUpdate += OnUpdate;
            _world = context.Resolve<World>();
            return Task.CompletedTask;
        }

        private void OnUpdate()
        {
            Invoke(nameof(RenderField), 0.5f); // Викликати через 2 секунди після ініціалізації
        }
        
        private void OnAddPack()
        {
            _gameManager.AddPack();
            Invoke(nameof(RenderField), 0.5f); // Викликати через 2 секунди після ініціалізації
        }

        private void OnStartNewGame()
        {
            _gameManager.StartNewGame();
            Invoke(nameof(RenderField), 0.5f); // Викликати через 2 секунди після ініціалізації
        }

        private void RenderField()
        {
            _gameFieldUiDebugView.RenderField(_world.EntityManager);
        }
        
        private void OnEnable()
        {
            _disposables = new CompositeDisposable();
            buttonStartNewGame.onClick.FromUnityEvent()
                .Subscribe(OnStartNewGame).AddTo(_disposables);
            
            buttonAddPack.onClick.FromUnityEvent()
                .Subscribe(OnAddPack).AddTo(_disposables);
        }
        
        private void OnDisable()
        {
            _disposables?.Dispose();
            _disposables = null;
        }
    }
    
}

