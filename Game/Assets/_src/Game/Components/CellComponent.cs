using TFs.Common.Entities;

namespace Game.Core.Components
{
    public struct CellComponent : IEntityBufferComponent, IEntityComponent
    {
        public int Index;
        public int Value;
        public bool IsRemoved; // true — клітинка була видалена, але ще показується в UI
    }
}