var app = angular.module('CinemaApp', []);

app.factory('reservationService',
    [
        "$http", function($http) {
            const self = {};

            self.create = (showId) => $http.post(`api/reservations/create/${showId}`).then(i => i.data, error => {
                if(error.status === 400) {
                    throw "The show is either booked out or in the past."
                }
            })

            self.getAll = () => $http.get("api/reservations/all").then(i => i.data, error => {
                if (error.status === 404) {
                    throw "No reservations found"
                }
            })

            self.delete = (reservationId) => $http.post(`api/reservations/delete/${reservationId}`);

            return self;
        }
    ]);

app.factory('showService',
    [
        "$http", function($http) {
            const self = {};

            self.make = () => $http.get("api/shows/make").then(i => i.data)

            self.getAll = () => $http.get("api/shows/all").then(i => i.data, error => {
                if (error.status === 404) {
                    throw "No shows found"
                }
            })

            self.delete = (showId) => $http.post(`api/shows/delete/${showId}`);

            self.create = (movie, showDateTime, maxReservation) => {
                var show = {
                    Movie: movie,
                    ShowDateTime: showDateTime,
                    MaxReservations: maxReservation
                }
                console.log(show)
                return $http.post("api/shows/post", show).then(i => i.data)
            }

            return self;
        }
    ]);


app.controller("indexController",
    [
        '$scope', '$window', 'reservationService', 'showService',
        ($scope, $window, reservationService, showService) => {
            $scope.data = {}
            $scope.data.reservations = []
            $scope.data.shows = []
            $scope.data.showIdEntry = null
            $scope.data.showInput = {}

            $scope.updateAll = () => {
                $scope.updateShows()
                $scope.updateReservations()
            }

            $scope.updateReservations = () => reservationService.getAll().then(i => {
                console.log(`got reservations: ${i.toString()}`)
                $scope.data.reservations = i
            }, (reason) => {
                $scope.data.reservations = []
            })

            $scope.updateShows = () => showService.getAll().then(i => {
                console.log(`got shows: ${i.toString()}`)
                $scope.data.shows = i
            }, (reason) => {
                console.log(`error while getting shows: ${reason}`)
                $scope.data.shows = []
            })

            $scope.createShow = () => showService.create(
                $scope.data.showInput.movie,
                new Date($scope.data.showInput.year, $scope.data.showInput.month-1, $scope.data.showInput.day, 0, 0, 0, 0),
                $scope.data.showInput.maxReservations).then(i => $scope.updateAll())

            $scope.createReservation = () => reservationService.create($scope.data.showIdEntry).then(i => {
                $scope.updateAll()
            }, (reason) => {
                alert(reason)
                $scope.updateAll()
            })

            $scope.deleteReservation = (reservationId) => reservationService.delete(reservationId).then(i => {
                $scope.updateAll()
            }, (reason) => {
                $scope.updateAll()
            })

            $scope.deleteShow = (showId) => showService.delete(showId).then(i => {
                console.log("show deleted" + i)
                $scope.updateAll()
            }, (reason) => {
                $scope.updateAll()
            })

            $scope.countReservations = (reservations) => {
                var size = 0, key;
                for (key in reservations) {
                    if (reservations.hasOwnProperty(key)) size++;
                }
                return size;
            }

            $scope.makeShow = () => showService.make().then(i => {
                $scope.updateAll()
            })

       
        }
    ]);