using Game.Core.Data;
using Unity.Mathematics;

namespace Game.Core
{
    /*
    public class RowMergeRule : IMergeRule
    {
        public bool CanMerge(FieldData field, int y1, int x1, int y2, int x2)
        {
            if (y1 != y2 || x1 == x2) return false;
            var min = math.min(x1, x2) + 1;
            var max = math.max(x1, x2);
            
            for (var x = min; x < max; x++)
                if (!field[y1, x].IsEmpty) return false;
            
            return true;
        }    
    }
    */
}