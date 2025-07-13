using Unity.Collections;

namespace TFs.Common.Entities
{
    public interface ISystem
    {
        public void OnCreate(EntityManager manager, SystemQuery query){}
        void OnUpdate(EntityManager manager, NativeArray<Entity> entities, float deltaTime);
        public void OnDestroy(EntityManager manager){}
    }
}