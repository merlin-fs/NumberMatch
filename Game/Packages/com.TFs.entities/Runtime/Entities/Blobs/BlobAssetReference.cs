using System;
using Unity.Collections.LowLevel.Unsafe;

namespace TFs.Common.Blobs
{
    public unsafe struct BlobAssetReference<T> where T : unmanaged
    {
        public byte* Pointer;

        public bool IsCreated => Pointer != null;
        // Доступ до root-структури
        public ref T Value => ref UnsafeUtility.AsRef<T>(Pointer);
        
        // Звільняти треба окремо
        public void Dispose()
        {
            if (Pointer == null) return;
            UnsafeUtility.Free(Pointer, Unity.Collections.Allocator.Persistent);
            Pointer = null;
        }
    }
}