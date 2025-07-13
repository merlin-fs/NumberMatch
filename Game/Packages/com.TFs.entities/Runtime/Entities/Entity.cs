using System;

namespace TFs.Common.Entities
{
    public struct Entity : IEquatable<Entity>, IComparable<Entity>
    {
        public static Entity Null { get; } = default;

        public int ID;
        
        public override string ToString()
        {
            return $"Entity: {ID}";
        }

        public bool Equals(Entity other)
        {
            return ID == other.ID;
        }

        public override bool Equals(object obj)
        {
            return obj is Entity other && Equals(other);
        }

        public override int GetHashCode()
        {
            return ID;
        }

        public int CompareTo(Entity other)
        {
            return ID.CompareTo(other.ID);
        }
    }
}