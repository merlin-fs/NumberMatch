using UnityEngine;

namespace Game.Core.Data
{
    public struct MergeCandidate
    {
        
        public int y1, x1, y2, x2;

        public MergeCandidate(int y1, int x1, int y2, int x2)
        {
            this.y1 = y1;
            this.x1 = x1;
            this.y2 = y2;
            this.x2 = x2;
        }
    }
}