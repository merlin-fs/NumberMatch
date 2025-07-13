using Game.Core.Components;
using Unity.Collections;

namespace Game.Core
{
    public class WrapRowMergeRule : IMergeRule
    {
        public bool CanMerge(FieldComponent field, NativeArray<CellComponent> cells, int idxA, int idxB)
        {
            var a = cells[idxA];
            var b = cells[idxB];
            if (a.IsRemoved || b.IsRemoved) return false;
            if (!(a.Value == b.Value || a.Value + b.Value == 10)) return false;

            var posA = field.FromIndex(a.Index);
            var posB = field.FromIndex(b.Index);

            // Перевіряємо wrap: a має бути в кінці рядка, b — на початку наступного
            if (posA.y + 1 != posB.y) return false;
            if (posA.x != field.Size.x - 1) return false;
            if (posB.x != 0) return false;

            // Перевіряємо, що всі клітинки після a до кінця рядка порожні
            for (int x = posA.x + 1; x < field.Size.x; x++)
            {
                int idx = field.At(x, posA.y);
                if (!cells[idx].IsRemoved) return false;
            }

            // Перевіряємо, що всі клітинки до b в наступному рядку порожні (але b.x == 0, тож цей цикл порожній)
            // Якщо колись дозволиш merge не тільки з перших клітинок наступного рядка, залиш цикл:
            // for (int x = 0; x < posB.x; x++)
            //    if (!cells[field.At(x, posB.y)].IsRemoved) return false;

            return true;
        }
    }
}