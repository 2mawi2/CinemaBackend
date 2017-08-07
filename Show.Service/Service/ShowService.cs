using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Common.Backup;
using Common.Model;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Reservation.Domain;
using Show.Domain;
using Show.Service.Repositorys;
using StatefulService = Microsoft.ServiceFabric.Services.Runtime.StatefulService;

namespace Show.Service.Service
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    public sealed class ShowService : StatefulService, IShowService
    {
        public static string ShowServiceType = "ShowServiceType";
        private readonly IShowRepository _showRepository;
        private readonly IBackupManager _backupManager;

        public ShowService(StatefulServiceContext context)
            : this(context, new ReliableStateManager(context))
        {
        }

        public ShowService(StatefulServiceContext serviceContext,
            IReliableStateManagerReplica reliableStateManagerReplica) : base(serviceContext,
            reliableStateManagerReplica)
        {
            _showRepository = new ShowRepository(StateManager);
            _backupManager = new BackupManager(
                new ServiceContextInfo(Context, Partition, StateManager),
                new BackupContextInfo(BackupAsync, "showRequestHistory", "showBackupCountDict"));
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

        public async Task<bool> AddAsync(ShowItem item)
        {
            return await _showRepository.AddAsync(item);
        }

        public async Task<bool> RemoveAsync(ItemId id)
        {
            return await _showRepository.RemoveAsync(id);
        }

        public async Task<ShowItem> GetByIdAsync(ItemId id)
        {
            return await _showRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<ShowItem>> GetAllShowsAsync()
        {
            return await _showRepository.GetAllAsync();
        }

        public async Task<bool> TryAddReservationToShowAsync(ReservationItem reservation)
        {
            var show = await GetByIdAsync(reservation.ShowItemId);
            if (IsInvalidShow(reservation, show))
                return false;

            show.Reservations.TryAdd(reservation.Id, reservation);
            return await _showRepository.UpdateAsync(show.Id, show);
        }

        private static bool IsInvalidShow(ReservationItem reservation, ShowItem show)
        {
            return show == null || IsReservationAfterShow(show, reservation) || IsShowBookedOut(show);
        }


        private static bool IsReservationAfterShow(ShowItem show, ReservationItem reservationItem)
        {
            return show.ShowDateTime.ToUniversalTime() < reservationItem.ReservationDateTime?.ToUniversalTime();
        }

        private static bool IsShowBookedOut(ShowItem show)
        {
            return show.MaxReservations <= show.Reservations.Count;
        }

        public async Task<bool> TryRemoveReservationFromShowAsync(ReservationItem item)
        {
            var show = await GetByIdAsync(item.ShowItemId);
            if (show.Reservations.ContainsKey(item.Id))
            {
                if (show.Reservations.TryRemove(item.Id, out _))
                {
                    return await _showRepository.UpdateAsync(show.Id, show);
                }
            }
            return false;
        }

        public async Task<IEnumerable<ShowItem>> GetShowsByMovieAsync(string movieName)
        {
            return await _showRepository.GetShowsByMovieAsync(movieName);
        }
    }
}