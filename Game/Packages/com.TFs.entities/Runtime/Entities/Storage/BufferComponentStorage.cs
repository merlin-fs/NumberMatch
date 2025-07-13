using System;
using System.Collections.Generic;
using Unity.Collections;

namespace TFs.Common.Entities 
{
    public interface IBufferComponentStorage : IDisposable
    {
        void Add(int entityId);
        void Remove(int entityId);
        void CopyBuffer(int srcEntityId, int dstEntityId);
    }
    
    public class BufferComponentStorage<T> : IBufferComponentStorage
        where T : unmanaged, IEntityBufferComponent
    {
        private readonly Dictionary<int, NativeList<T>> _entityBuffers;

        public BufferComponentStorage(Allocator allocator)
        {
            _entityBuffers = new Dictionary<int, NativeList<T>>();
        }

        public void Add(int entityId)
        {
            if (!_entityBuffers.TryGetValue(entityId, out var buffer))
            {
                buffer = new NativeList<T>(Allocator.Persistent);
                _entityBuffers[entityId] = buffer;
            }
        }

        public void Remove(int entityId)
        {
            if (!_entityBuffers.TryGetValue(entityId, out var buffer)) return;
            
            buffer.Dispose();
            _entityBuffers.Remove(entityId);
        }

        public void CopyBuffer(int srcEntityId, int dstEntityId)
        {
            if (!_entityBuffers.TryGetValue(srcEntityId, out var buffer)) return; // Entity not in storage
            Add(dstEntityId);
            _entityBuffers[dstEntityId].CopyFrom(buffer);
        }

        public NativeList<T> GetBuffer(int entityId)
        {
            return _entityBuffers.GetValueOrDefault(entityId);
        }

        public void Dispose()
        {
            foreach (var buffer in _entityBuffers.Values)
            {
                buffer.Dispose();
            }
            _entityBuffers.Clear();
        }
    }}
