using Game.Core.Components;
using Unity.Collections;
using Unity.Mathematics;

namespace Game.Core
{
    public class DiagonalMergeRule : IMergeRule
    {
        public bool CanMerge(FieldComponent field, NativeArray<CellComponent> cells, int idxA, int idxB)
        {
            var a = cells[idxA];
            var b = cells[idxB];
            if (a.IsRemoved || b.IsRemoved) return false;
            if (!(a.Value == b.Value || a.Value + b.Value == 10)) return false;

            var posA = field.FromIndex(a.Index);
            var posB = field.FromIndex(b.Index);

            // Тільки на діагоналі (|dx| == |dy|)
            int dx = posA.x - posB.x;
            int dy = posA.y - posB.y;
            if (math.abs(dx) != math.abs(dy)) return false;

            int stepX = dx > 0 ? -1 : 1;
            int stepY = dy > 0 ? -1 : 1;
            int steps = math.abs(dx);

            int x = posA.x + stepX;
            int y = posA.y + stepY;
            for (var i = 1; i < steps; i++)
            {
                var idx = field.At(x, y);
                if (idx == a.Index || idx == b.Index)
                {
                    x += stepX; y += stepY; 
                    continue;
                }
                if (!cells[idx].IsRemoved) return false;
                x += stepX;
                y += stepY;
            }
            return true;
        }
    }
}