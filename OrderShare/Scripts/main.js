$(document).ready(function () {

    /*var _showLoading = function () {
        $('.loading').removeClass('display-none');
    };
    var _hideLoading = function () {
        $('.loading').addClass('display-none');
    };

    var getData = function () {
        $('.message').empty();
        $('.message').html('System will automatic read data every 6 minutes');
        $.ajax({
            url: baseUri + 'Home/ReadData',
            type: 'GET',
            dataType: 'json',
            beforeSend: function () {
                _showLoading();
            },
            complete: function () {
                _hideLoading();
            },
            success: function (data) {
                if (data) {
                    setTimeout(function () {
                        $('.message').html(data.Message);
                        $('.message').hide();
                    }, 100);
                }
            }
        });
    };

    getData();

    ////auto reload after every 6 minutes
    setInterval(function () {
        getData();
    }, 2000);
    */
});