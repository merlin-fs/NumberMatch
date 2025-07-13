using TFs.Common.Entities;

namespace Game.Core.Components
{
    public struct AddNumbersRequestComponent : IEntityComponent
    {
        public int NumbersCount; // Скільки чисел додати (не рядків!)
        public int Index; // Індекс клітинки, куди додавати числа
    }
}