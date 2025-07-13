using TFs.Common.Entities;

namespace Game.Core.Components
{
    public struct MergeRequestComponent : IEntityComponent
    {
        public int IndexA; // індекс першої клітинки
        public int IndexB; // індекс другої клітинки
    }
}