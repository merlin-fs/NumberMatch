using System;
using Game.Core.Components;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class CellView : MonoBehaviour
    {
        [SerializeField] private TMPro.TMP_Text text; // Компонент TextMeshPro для відображення значення клітинки
        [SerializeField] private Button button;
        [SerializeField] private Image image;

        private Action _onClick;
        
        private void Awake()
        {
            button.onClick.AddListener(() => _onClick?.Invoke());
        }

        public void SetValue(CellComponent value, Action onClick)
        {
            _onClick = onClick; // Зберігаємо делегат для обробки кліку
            text.text = value.Value.ToString(); // Встановлюємо текст клітинки
            SetRemoved(value.IsRemoved);
        }

        private void SetRemoved(bool isRemoved)
        {
            text.color = isRemoved ? Color.gray7 : Color.black; // Змінюємо колір тексту, якщо клітинка видалена
        }

        public void SetSelected(bool value)
        {
            image.color = value ? Color.cornflowerBlue : Color.white; // Змінюємо колір фону, якщо клітинка вибрана
        }
    }
}