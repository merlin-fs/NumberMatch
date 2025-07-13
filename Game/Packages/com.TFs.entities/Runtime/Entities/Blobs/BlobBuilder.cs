using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace TFs.Common.Blobs
{
    public unsafe class BlobBuilder: IDisposable
    {
        private byte* _tempBuffer;
        private int _offset;
        private int _capacity;

        public BlobBuilder(int capacity)
        {
            _capacity = capacity;
            // Виділяємо тимчасову пам’ять
            _tempBuffer = (byte*)UnsafeUtility.Malloc(capacity, 16, Allocator.Temp);
            UnsafeUtility.MemClear(_tempBuffer, capacity);
            _offset = 0;
        }

        // Створюємо root-структуру (T)
        public ref T ConstructRoot<T>() where T : unmanaged
        {
            var size = UnsafeUtility.SizeOf<T>();
            var align = UnsafeUtility.AlignOf<T>();
            _offset = Align(_offset, align);

            if (_offset + size > _capacity)
                throw new Exception("BlobBuilder overflow");

            byte* ptr = _tempBuffer + _offset;
            _offset += size;
            return ref UnsafeUtility.AsRef<T>(ptr);
        }

        // Виділяємо масив (unmanaged)
        public byte* Allocate(int count, int elementSize, int elementAlign)
        {
            _offset = Align(_offset, elementAlign);
            var size = count * elementSize;

            if (_offset + size > _capacity)
                throw new Exception("BlobBuilder overflow");

            byte* ptr = _tempBuffer + _offset;
            _offset += size;

            return ptr;
        }

        // Переводимо з тимчасового буфера в persistent
        public BlobAssetReference<T> CreateBlobAssetReference<T>() where T : unmanaged
        {
            var finalPtr = (byte*)UnsafeUtility.Malloc(_offset, 16, Allocator.Persistent);
            UnsafeUtility.MemCpy(finalPtr, _tempBuffer, _offset);
            var blobRef = new BlobAssetReference<T>{Pointer = finalPtr};
            return blobRef;
        }

        public void Dispose()
        {
            if (_tempBuffer == null) return;

            UnsafeUtility.Free(_tempBuffer, Allocator.Temp);
            _tempBuffer = null;
        }

        private int Align(int offset, int align)
        {
            return (offset + (align - 1)) & ~(align - 1);
        }    
    }
}
