using Game.Core.Components;
using UnityEngine;

namespace Game.UI
{
    public class CellView : MonoBehaviour
    {
        [SerializeField] private TMPro.TMP_Text text; // Компонент TextMeshPro для відображення значення клітинки

        public void SetValue(CellComponent value)
        {
            text.text = value.Value.ToString(); // Встановлюємо текст клітинки
            SetRemoved(value.IsRemoved);
        }

        private void SetRemoved(bool isRemoved)
        {
            text.color = isRemoved ? Color.gray7 : Color.black; // Змінюємо колір тексту, якщо клітинка видалена
        }
    }
}