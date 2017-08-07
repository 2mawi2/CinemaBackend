using System;
using System.Collections.Concurrent;
using System.Runtime.Serialization;
using Common.Model;
using Reservation.Domain;

namespace Show.Domain
{
    [DataContract]
    [KnownType(typeof(ItemId))]
    public class ShowItem : IItem
    {
        [DataMember]
        public ItemId Id { get; set; } = new ItemId();

        /// <summary>
        /// MovieTitle of the show
        /// </summary>
        [DataMember]
        public string Movie { get; set; }

        /// <summary>
        /// Exact date of the show
        /// </summary>
        [DataMember]
        public DateTime ShowDateTime { get; set; }

        /// <summary>
        /// maximum of possible reservations for the show
        /// </summary>
        [DataMember]
        public int MaxReservations { get; set; }

        /// <summary>
        /// reservations booked on the show
        /// </summary>
        [DataMember]
        public ConcurrentDictionary<ItemId, ReservationItem> Reservations { get; set; } = new ConcurrentDictionary<ItemId, ReservationItem>();
    }
}