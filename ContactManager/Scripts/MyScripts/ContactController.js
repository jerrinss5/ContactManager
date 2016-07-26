app.controller('contactcontroller', function ($scope, contactservice) {
    $scope.Contacts = [];

    $scope.Message = "";
    $scope.userName = sessionStorage.getItem('userName');


    loadContacts();

    function loadContacts() {


        var promise = contactservice.get();
        promise.then(function (resp) {
            $scope.Contacts = resp.data;
            $scope.Message = "Call Completed Successfully";
        }, function (err) {
            $scope.Message = "Error!!! " + err.status
        });
    };
    $scope.logout = function () {

        sessionStorage.removeItem('accessToken');
        window.location.href = '/Login/SecurityInfo';
    };
});