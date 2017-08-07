using System;
using System.Runtime.Serialization;
using Common.Model;

namespace Reservation.Domain
{
    [DataContract]
    public class ReservationItem : IItem
    {
        [DataMember]
        public ItemId Id { get; set; } = new ItemId();

        /// <summary>
        /// Moviename of the reservation
        /// </summary>
        [DataMember]
        public string Movie { get; set; }

        /// <summary>
        /// Reference to ShowId
        /// </summary>
        [DataMember]
        public ItemId ShowItemId { get; set; }

        /// <summary>
        /// Reservation DateTime
        /// If not given by the client it will be set from the service to 
        /// DateTime.UtcNow()
        /// </summary>
        [DataMember]
        public DateTime? ReservationDateTime { get; set; }
    }
}