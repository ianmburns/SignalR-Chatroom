var userVM = function(userName) {
    this.name = userName;
};

var messageVM = function(userName, message, template) {
    this.name = userName;
    this.message = message;
    this.template = template;
};

var viewModel = {
    users: ko.observableArray(),
    messages: ko.observableArray(),

    template: function (messageVM) {
        return messageVM.template;
    },
};

$(function () {

    ko.applyBindings(viewModel);

});

