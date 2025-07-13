using Game.Core.Components;
using Unity.Collections;

namespace Game.Core
{
    public interface IMergeRule
    {
        bool CanMerge(FieldComponent field, NativeArray<CellComponent> cells, int idxA, int idxB);
    }
}