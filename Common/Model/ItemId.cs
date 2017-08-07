using System;
using System.Runtime.Serialization;
using Common.Utils;
using Microsoft.ServiceFabric.Services.Client;

namespace Common.Model
{
    [DataContract]
    [KnownType(typeof(Guid))]
    public class ItemId : IEquatable<ItemId>, IComparable<ItemId>, IFormattable, IComparable
    {
        [DataMember] private readonly Guid _id;

        public ItemId()
        {
            _id = Guid.NewGuid();
        }

        public ItemId(Guid id)
        {
            _id = id;
        }

        public override string ToString()
        {
            return _id.ToString();
        }

        public int CompareTo(ItemId other)
        {
            if (ReferenceEquals(this, other)) return 0;
            return ReferenceEquals(null, other) ? 1 : _id.CompareTo(other._id);
        }

        public bool Equals(ItemId other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) || _id.Equals(other._id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType() && Equals((ItemId) obj);
        }

        public ServicePartitionKey GetPartitionKey()
        {
            return new ServicePartitionKey(HashUtil.GetLongHashCode(_id.ToString()));
        }

        public override int GetHashCode() => _id.GetHashCode();

        public int CompareTo(object obj)
        {
            return _id.CompareTo(((ItemId) obj)._id);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return _id.ToString(format, formatProvider);
        }

        public static bool operator ==(ItemId left, ItemId right) => Equals(left, right);

        public static bool operator !=(ItemId left, ItemId right) => !Equals(left, right);
    }
}