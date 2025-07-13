using System.Collections.Generic;
using System.Threading.Tasks;
using Game.Core;
using Game.Core.Components;
using TFs.Common.Contexts;
using UnityEngine;
using TFs.Common.Entities;
using Unity.Mathematics; 

namespace Game.UI
{
    public class GameFieldUiDebugView : MonoBehaviour, IInitializable
    {
        [SerializeField] private RectTransform root; // Контейнер під Canvas (наприклад, пустий GameObject з VerticalLayoutGroup)
        [SerializeField] private GameObject cellPrefab; // Prefab з TMP_Text/Text (наприклад, просто TextMeshProUGUI)
        
        private GameManager _gameManager;
        private World _world;

        private readonly Dictionary<int, CellView> _cellTexts = new();
        private int2 _size;

        public Task Initialize(Context context)
        {
            _gameManager = context.Resolve<GameManager>();
            _world = context.Resolve<World>();
            
            this.Invoke(nameof(RenderField), 2f); // Викликати через 2 секунди після ініціалізації
            return Task.CompletedTask;
        }
        
        // Виклич вручну або через систему, наприклад після змін у полі
        public void RenderField()
        {
            EntityManager manager = _world.EntityManager;
            var field = manager.GetComponent<FieldComponent>(_gameManager.Field);
            var cells = manager.GetBuffer<CellComponent>(_gameManager.Field);
            var total = field.Size.x * field.Size.y;

            while (root.childCount > total)
            {
                DestroyImmediate(root.GetChild(root.childCount - 1).gameObject);
            }

            // Знайти сутність поля і дізнатися розмір
            if (cells.Length == 0) return;

            // Підготувати сітку (destroy/create cells, якщо змінився розмір)
            while (root.childCount < total)
            {
                var cellGo = Instantiate(cellPrefab, root);
                cellGo.name = $"CellText_{root.childCount}";
                var cellView = cellGo.GetComponent<CellView>();
                _cellTexts[root.childCount-1] = cellView;
            }

            // Відмалювати кожну клітинку
            for (var i = 0; i < total; i++)
            {
                var pos = (float2)field.FromIndex(i);
                
                pos.x *= cellPrefab.GetComponent<RectTransform>().rect.width; 
                pos.y *= -cellPrefab.GetComponent<RectTransform>().rect.height;
                var offset = - new float2(root.GetComponent<RectTransform>().rect.width, 
                                    -root.GetComponent<RectTransform>().rect.height) * 0.5f;
                    
                var cellView = _cellTexts[i];
                
                cellView.GetComponent<RectTransform>()
                    .anchoredPosition = math.float2(root.anchoredPosition.x, root.anchoredPosition.y) + pos + offset;
                cellView.SetValue(i < cells.Length ? cells[i] : new CellComponent{IsRemoved = true, Value = 0});
            }
        }
    }
}