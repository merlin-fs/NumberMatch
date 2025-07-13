using Game.Core.Components;
using Unity.Collections;
using Unity.Mathematics;

namespace Game.Core
{
    public class ColumnMergeRule : IMergeRule
    {
        public bool CanMerge(FieldComponent field, NativeArray<CellComponent> cells, int idxA, int idxB)
        {
            var a = cells[idxA];
            var b = cells[idxB];
            if (a.IsRemoved || b.IsRemoved) return false;
            if (!(a.Value == b.Value || a.Value + b.Value == 10)) return false;

            var posA = field.FromIndex(a.Index);
            var posB = field.FromIndex(b.Index);

            // В одному стовпці
            if (posA.x != posB.x) return false;

            // Перевіряємо "видимість" у колонці
            var minY = math.min(posA.y, posB.y) + 1;
            var maxY = math.max(posA.y, posB.y);
            for (var y = minY; y < maxY; y++)
            {
                var idx = field.At(posA.x, y);
                if (idx == a.Index || idx == b.Index) continue;
                if (!cells[idx].IsRemoved) return false;
            }
            return true;
        }
    }
   
}