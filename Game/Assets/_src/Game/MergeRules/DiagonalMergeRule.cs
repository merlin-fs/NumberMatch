using Game.Core.Data;
using Unity.Mathematics;

namespace Game.Core
{
    /*
    public class DiagonalMergeRule : IMergeRule
    {
        public bool CanMerge(FieldData field, int y1, int x1, int y2, int x2)
        {
            if (math.abs(y1 - y2) != math.abs(x1 - x2) || (y1 == y2 && x1 == x2)) return false;
            
            var stepY = y1 < y2 ? 1 : -1;
            var stepX = x1 < x2 ? 1 : -1;
            int y = y1 + stepY, x = x1 + stepX;
            
            while (y != y2 && x != x2)
            {
                if (!field[y, x].IsEmpty) return false;
                y += stepY;
                x += stepX;
            }
            return true;
        }
    }
    */
}