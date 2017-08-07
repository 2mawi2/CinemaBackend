app.controller("indexController",
    [
        '$scope', '$window', 'reservationService',
        ($scope, $window, reservationService) => {
            $scope.data = {}
            $scope.data.reservations = []
            $scope.updateReservations = () => reservationService.getReservations().then(i => {
                $scope.data.reservations = i
            }, i => alert(i))

            $scope.getReservations = () => reservationService.getReservations()

            //Initialization
            $scope._init_ = () => {
                const toResultObject = (promise) => {
                    return promise
                        .then(result => ({ success: true, result }))
                        .catch(error => ({ success: false, error }));
                };
                let servicePromises = [reservationService.getReservations()];
                Promise.all(servicePromises.map(toResultObject))
                    .then(values => {
                        var reservations = values[0].result
                        $scope.data.reservations = reservations
                        return $scope.data.reservations
                    })
                    .then(reservations => {

                    })
            }
        }
    ]);