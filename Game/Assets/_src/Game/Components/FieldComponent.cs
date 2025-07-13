using System.Runtime.CompilerServices;
using TFs.Common.Entities;
using Unity.Mathematics;

namespace Game.Core.Components
{
    public struct FieldComponent : IEntityComponent
    {
        public int2 Size; // x=Width, y=Height

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int2 FromIndex(int index)
        {
            return new int2(index % Size.x, index / Size.x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int At(int x, int y) => (y * Size.x) + x;
    }
}