function getParameterByName(name, url) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(url);
    return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}

function lgClearAllUrl() {
    $('#baseUrlTextArea, #baseLinkGeneratorUrl').val('').text('');
    $('#encodedLinkGeneratorUrl, #encodedUrlTextArea').val('').text('');
}

function lgEncodeFromTextArea() {
    var baseUrl = $('#baseUrlTextArea').val();
    if (baseUrl != '') {
        var email = $('#email').val();

        if (!email) {
            alert('Enter email!');
            return;
        }

        var url = baseUrl.split('?')[0];
        var params = baseUrl.split('?')[1];
        if (params && url) {
            $.ajax({
                type: "POST",
                contentType: "application/json",
                url: encodeUrl,
                data: JSON.stringify({ link: params, email: email }),
                success: function (data) {
                    var result = url + '?hash=' + data;
                    $('#encodedUrlTextArea').text(result).val(result);
                },
                error: function () {
                    alert('Sorry, an error has eccoured');
                }
            });
        } else {
            alert('Enter valid url!');
        }
    } else {
        alert('Enter url for encoding');
    }
}
function lgDecodeFromTextArea() {
    var encodedUrl = $('#encodedUrlTextArea').val();
    if (encodedUrl != '') {
        var email = $('#email').val();

        if (!email) {
            alert('Enter email!');
            return;
        }
        var url = encodedUrl.split('?')[0];
        var hash = getParameterByName('hash', encodedUrl);
        if (hash && url) {
            $.ajax({
                type: "POST",
                contentType: "application/json",
                url: decodeUrl,
                data: JSON.stringify({ hash: hash, email: email }),
                success: function (data) {
                    var result = url + data.data;
                    $('#baseUrlTextArea').text(result).val(result);
                },
                error: function () {
                    alert('Sorry, an error has eccoured');
                }
            });
        } else {
            alert('Enter valid url!');
        }
    } else {
        alert('Enter url for decoding');
    }
}