// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Runtime.Serialization;
using Microsoft.ServiceFabric.Actors;

namespace Common.Model
{
    [DataContract]
    public class CustomerOrderActorMessageId : IEquatable<CustomerOrderActorMessageId>,
        IComparable<CustomerOrderActorMessageId>, IFormattable, IComparable
    {
        public CustomerOrderActorMessageId(ActorId sendingActorId, long messageId)
        {
            this.sendingActorId = sendingActorId;
            this.messageId = messageId;
        }

        [DataMember]
        public ActorId sendingActorId { get; private set; }

        [DataMember]
        public long messageId { get; private set; }

        int IComparable.CompareTo(object obj)
        {
            return CompareTo((CustomerOrderActorMessageId) obj);
        }

        public int CompareTo(CustomerOrderActorMessageId other)
        {
            if (sendingActorId.ToString().CompareTo(other.sendingActorId.ToString()) > 1)
            {
                return 1;
            }
            if (sendingActorId.ToString().CompareTo(other.sendingActorId.ToString()) < 1)
            {
                return -1;
            }
            if (messageId > other.messageId)
            {
                return 1;
            }
            if (messageId < other.messageId)
            {
                return -1;
            }

            return 0;
        }

        public bool Equals(CustomerOrderActorMessageId other)
        {
            return (sendingActorId.Equals(other.sendingActorId) && messageId == other.messageId);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return $"{sendingActorId}|{messageId}";
        }

        public static CustomerOrderActorMessageId GetRandom()
        {
            ActorId id = new ActorId(Guid.NewGuid());
            Random r = new Random();
            return new CustomerOrderActorMessageId(id, r.Next());
        }

        public static bool operator ==(CustomerOrderActorMessageId item1, CustomerOrderActorMessageId item2)
        {
            return item1.Equals(item2);
        }

        public static bool operator !=(CustomerOrderActorMessageId item1, CustomerOrderActorMessageId item2)
        {
            return !item1.Equals(item2);
        }

        public static bool operator >(CustomerOrderActorMessageId item1, CustomerOrderActorMessageId item2)
        {
            var result = item1.CompareTo(item2);
            return (result == 0 | result == -1);
        }

        public static bool operator <(CustomerOrderActorMessageId item1, CustomerOrderActorMessageId item2)
        {
            int result = item1.CompareTo(item2);
            return (result == 0 | result == 1);
        }

        public override bool Equals(object obj)
        {
            return (CompareTo(obj as CustomerOrderActorMessageId) == 0);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}