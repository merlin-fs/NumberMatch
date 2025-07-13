using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace TFs.Common.Blobs
{
    public static class BlobStorage
    {
        private static int _nextId = 0;
        private static readonly List<IBlobRef> _blobs = new();

        // Зберігає blob і повертає BlobId
        public static BlobId RegisterBlob<T>(BlobAssetReference<T> blob) where T : unmanaged
        {
            var id = _nextId++;
            _blobs[id] = new BlobRefImpl<T>(blob);
            return (BlobId)id;
        }

        public static BlobId RegisterBlob<T>(T value) where T : unmanaged
        {
            var blob = Build(ref value);
            return RegisterBlob(blob);
        }

        private static unsafe BlobAssetReference<TData> Build<TData>(ref TData data)
            where TData : unmanaged
        {
            var size = UnsafeUtility.SizeOf<TData>();
            var align = UnsafeUtility.AlignOf<TData>();

            // 1. Виділяємо Temp
            var temp = (byte*)UnsafeUtility.Malloc(size, align, Allocator.Temp);
            UnsafeUtility.MemClear(temp, size);

            // 2. Копіюємо поля TData
            UnsafeUtility.CopyStructureToPtr(ref data, temp);

            // 3. Переносимо у Persistent
            var finalPtr = (byte*)UnsafeUtility.Malloc(size, align, Allocator.Persistent);
            UnsafeUtility.MemCpy(finalPtr, temp, size);

            UnsafeUtility.Free(temp, Allocator.Temp);

            return new BlobAssetReference<TData> {Pointer = finalPtr};
        }
        
        // Дістає blob
        public static BlobAssetReference<T> GetBlob<T>(BlobId id) where T : unmanaged
        {
            if ((int)id >= _blobs.Count) 
                throw new Exception($"BlobId {id} not found");
            if (_blobs[(int)id] is not BlobRefImpl<T> typed)
                throw new Exception($"Blob with ID {id} is not of type {typeof(T)}");
            return typed.Blob;
        }

        // Для універсального зберігання
        private interface IBlobRef {}

        private class BlobRefImpl<T> : IBlobRef where T : unmanaged
        {
            public readonly BlobAssetReference<T> Blob;
            public BlobRefImpl(BlobAssetReference<T> blob) => Blob = blob;
        }
    }
}