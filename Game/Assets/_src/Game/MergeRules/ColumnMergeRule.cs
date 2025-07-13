using Game.Core.Data;
using Unity.Mathematics;

namespace Game.Core
{
    /*
    public class ColumnMergeRule : IMergeRule
    {
        public bool CanMerge(FieldData field, int y1, int x1, int y2, int x2)
        {
            if (x1 != x2 || y1 == y2) return false;
            
            var min = math.min(y1, y2) + 1;
            var max = math.max(y1, y2);
            for (var y = min; y < max; y++)
                if (!field[y, x1].IsEmpty) return false;
            
            return true;
        }
    }
    */
}