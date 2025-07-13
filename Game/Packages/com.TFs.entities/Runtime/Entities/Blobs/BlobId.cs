using System;

namespace TFs.Common.Blobs
{
    [Serializable]
    public struct BlobId : IEquatable<BlobId>
    {
        private int _id;
        public override string ToString() => $"[{_id}]";
        public bool Equals(BlobId other) => _id == other._id;

        public override bool Equals(object obj)
        {
            var result = (obj is BlobId other)
                ? Equals(other) ? 1 : 0
                : 0;
            return result != 0;
        }

        public static bool operator ==(BlobId left, BlobId right) => left.Equals(right);
        public static bool operator !=(BlobId left, BlobId right) => !left.Equals(right);
        public static implicit operator BlobId(int value) => new() { _id = value };
        public static explicit operator int(BlobId value) => value._id;

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }
    }
}