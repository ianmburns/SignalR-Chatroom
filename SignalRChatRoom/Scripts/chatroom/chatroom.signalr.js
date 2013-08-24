$("#txtMessage").keypress(function (e) {
    if (e.which == 13) {
        $('#btnSubmit').click();
    }
});

$(function () {

    messageClearFocus();

    // Declare a proxy to reference the hub. 
    var chatHub = $.connection.ChatHub;

    registerClientMethods(chatHub);

    // Start Hub
    $.connection.hub.start().done(function () {

        registerEvents(chatHub);

    });

});

function registerClientMethods(hub) {

    hub.client.addMessage = function (name, message) {
        // Add the message to the page. 
        viewModel.messages.push(new messageVM(name, message, 'normalMessage'));
    };

    hub.client.refreshUserList = function (userList) {

        viewModel.users.removeAll();

        for (var i = 0; i < userList.length; i++) {
            viewModel.users.push(new userVM(userList[i].Name));
        }
    };

    hub.client.systemAlert = function (message) {
        viewModel.messages.push(new messageVM(name, message, 'systemMessage'));
    };
}

function registerEvents(hub) {

    $('#btnSubmit').click(function (e) {
        e.preventDefault();

        if (join(hub)) {

            messageClearFocus();
            $('#txtMessage').prop('placeholder', '');

            $('#btnSubmit').html('Send');

            $(this).off('click');
            $(this).on('click', function (e) { send(e, hub); });
        }
    });

    var join = function (hub) {

        var name = $('#txtMessage').val();

        if (name.length > 0) {

            var name = $('#txtMessage').val();

            hub.server.connect(name);

            $('#displayname').val(name);
            return true;
        }
        return false;
    }

    var send = function (e, hub) {
        e.preventDefault();
        // Call the Send method on the hub.
        var message = $('#txtMessage').val();
        if (message.length > 0)
            hub.server.sendMessage(message);
        // Clear text box and reset focus for next comment. 
        messageClearFocus();
    };

    $('#rooms ul li').not('.nav-header').click(function (e) {
        e.preventDefault();

        $('#rooms ul li').not('.nav-header').removeClass('active');

        $(this).addClass('active');
        var roomKey = $(this).data('value');
        hub.server.joinGroup(roomKey);
    });
}

function messageClearFocus() {
    $('#txtMessage').val('').focus();
}

function htmlEncode(value) {
    var encodedValue = $('<div />').text(value).html();
    return encodedValue;
}