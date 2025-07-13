using Game.Core.Data;

namespace Game.Core
{
    /*
    public class WrapRowMergeRule : IMergeRule
    {
        public bool CanMerge(FieldData field, int y1, int x1, int y2, int x2)
        {
            if (y1 + 1 != y2) return false;             // Тільки з наступним рядком
            if (x1 != field.Size.x - 1) return false;   // Тільки остання клітинка рядка
            if (x2 != 0) return false;                  // Тільки перша клітинка наступного рядка

            // Перевіряємо, що між ними немає чисел:
            // Від x1+1 до кінця рядка (має бути порожньо)
            for (var x = x1 + 1; x < field.Size.x; x++)
                if (!field[y1, x].IsEmpty) return false;

            // Від 0 до x2-1 у наступному рядку (має бути порожньо, але x2==0, так що цикл не зайде)
            // Залишаємо для універсальності (наприклад, якщо правило розшириться)
            // for (int x = 0; x < x2; x++)
            //    if (!field[y2, x].IsEmpty) return false;

            return true;
        }
    }
    */
}