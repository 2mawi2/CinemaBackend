using Common;
using Common.Backup;
using Common.Factorys;
using Common.Model;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Reservation.Domain;
using Reservation.Service.Repositorys;
using Show.Domain;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Extensions;
using StatefulService = Microsoft.ServiceFabric.Services.Runtime.StatefulService;

namespace Reservation.Service.Service
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    public sealed class ReservationService : StatefulService, IReservationService
    {
        private readonly IServiceFactory<IShowService> _showServiceFactory;
        public static string ReservationServiceType = "ReservationServiceType";
        private readonly IReservationRepository _reservationRepository;
        private readonly IBackupManager _backupManager;

        public ReservationService(StatefulServiceContext context, IServiceFactory<IShowService> showServiceFactory)
            : this(context, new ReliableStateManager(context), showServiceFactory)
        {
        }

        public ReservationService(StatefulServiceContext serviceContext,
            IReliableStateManagerReplica reliableStateManagerReplica,
            IServiceFactory<IShowService> showServiceFactory) : base(serviceContext,
            reliableStateManagerReplica)
        {
            _reservationRepository = new ReservationRepository(StateManager);
            _showServiceFactory = showServiceFactory;
            _backupManager = new BackupManager(
                new ServiceContextInfo(Context, Partition, StateManager),
                new BackupContextInfo(BackupAsync, "reservationRequestHistory", "reservationBackupCountDict"));
        }

        protected override Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                return Task.WhenAll(_backupManager.PeriodicTakeBackupAsync(cancellationToken, Partition));
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceMessage(Context, "RunAsync failed, {0}", e);
                throw;
            }
        }

        protected override Task<bool> OnDataLossAsync(RestoreContext restoreCtx, CancellationToken cancellationToken)
        {
            return _backupManager.OnDataLossAsync(restoreCtx, cancellationToken, Partition);
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[]
            {
                new ServiceReplicaListener(this.CreateServiceRemotingListener)
            };
        }

        /// <summary>
        /// This Method Trys to Add the Reservation to the corresponding show
        /// in the show service. If success -> Add the Reservation
        /// </summary>
        public async Task<bool> AddAsync(ReservationItem item)
        {
            if (item.ShowItemId == null) // ShowItemId is invalid
                return false;

            if (!item.ReservationDateTime.HasValue) // DateTime missing
                item.ReservationDateTime = DateTime.UtcNow;

            bool isAddedOnShow =
                await ShowService(item).TryAddReservationToShowAsync(item);

            if (!isAddedOnShow) // show was not added to show
                return false;

            return await _reservationRepository.AddAsync(item);
        }

        private IShowService ShowService(ReservationItem item)
        {
            return _showServiceFactory.Get(item.ShowItemId.GetPartitionKey());
        }

        /// <summary>
        /// This method deletes a reservation from the reservationService
        /// and then sends the message TryRemoveReservationFromShowAsync
        /// over a service proxy
        /// </summary>
        public async Task<bool> RemoveAsync(ItemId reservationId)
        {
            var reservationItem = await GetByIdAsync(reservationId);
            if (reservationItem.ShowItemId == null) return false;
            // get partition key from persisted showItem
            var servicePartitionKey = reservationItem.ShowItemId.GetPartitionKey();
            // get showService reference
            var showService = _showServiceFactory.Get(servicePartitionKey);
            // send reservationItem (which shall be deleted) to show service
            var isReservationRemoved =
                await showService.TryRemoveReservationFromShowAsync(reservationItem);
            if (isReservationRemoved)
            {
                //remove reservation if all was successful
                await _reservationRepository.RemoveAsync(reservationId);
                return true;
            }
            return false;
        }

        /// <summary>
        /// This Method deletes the reservation in its own Repository
        /// and also removes the entry from the corresponding Show
        /// </summary>
        public async Task<bool> RemoveAllWithShowIdAsync(ItemId showId)
        {
            var reservationsToDelete = await _reservationRepository
                .FindAsync(i => i.ShowItemId.Equals(showId));

            var tasks = reservationsToDelete
                .Select(i => i.Id)
                .Select(async i => await _reservationRepository.RemoveAsync(i));

            return Task.WhenAll(tasks)
                       .GetAwaiter()
                       .GetResult()
                       .Any(i => i.Equals(true));
        }

        public async Task<ReservationItem> GetByIdAsync(ItemId id)
        {
            return await _reservationRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<ReservationItem>> GetAllReservationsAsync()
        {
            return await _reservationRepository.GetAllAsync();
        }

        public async Task<IEnumerable<ReservationItem>> GetByMovieAsync(string movieName)
        {
            return await _reservationRepository.GetByMovieAsync(movieName);
        }

        public async Task<IEnumerable<ReservationItem>> GetByTimeAsync(TimeRange timeRange)
        {
            return await _reservationRepository.GetByTimeAsync(timeRange);
        }
    }
}