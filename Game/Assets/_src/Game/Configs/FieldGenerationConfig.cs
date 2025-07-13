using Unity.Collections;
using Unity.Mathematics;

namespace Game.Configs
{
    public struct FieldGenerationConfig
    {
        public int2 Size;                // Розмір поля (width, height)
        public int StartRows;            // Кількість стартових рядків
        public NativeArray<int> Deck;    // Весь deck, з якого беруться числа
        public int DeckPosition;         // Поточний індекс у deck (для генерації рядків)
        public uint Seed;                // Для random (опційно)
    }
}