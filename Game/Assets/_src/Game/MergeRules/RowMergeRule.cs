using Game.Core.Components;
using Unity.Collections;
using Unity.Mathematics;

namespace Game.Core
{
    public class RowMergeRule : IMergeRule
    {
        public bool CanMerge(FieldComponent field, NativeArray<CellComponent> cells, int idxA, int idxB)
        {
            var a = cells[idxA];
            var b = cells[idxB];
            if (a.IsRemoved || b.IsRemoved) return false;
            if (!(a.Value == b.Value || a.Value + b.Value == 10)) return false;

            var posA = field.FromIndex(a.Index);
            var posB = field.FromIndex(b.Index);

            // Перевіряємо, чи в одному рядку
            if (posA.y != posB.y) return false;

            // Перевіряємо "видимість" (між ними не має бути інших активних)
            var min = math.min(posA.x, posB.x) + 1;
            var max = math.max(posA.x, posB.x);

            for (var x = min; x < max; x++)
            {
                var idx = field.At(x, posA.y);
                if (idx == a.Index || idx == b.Index) continue;
                var cell = cells[idx];
                if (!cell.IsRemoved) //cell.HasValue && 
                    return false;
            }

            return true;
        }
    }
}