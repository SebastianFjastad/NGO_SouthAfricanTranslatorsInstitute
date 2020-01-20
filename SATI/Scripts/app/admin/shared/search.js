function Search(memberIdToRemove) {
    var recentlyViewedMembersStorageKey = "recently_viewed_members";

    var viewModel = function () {
        var self = this;
        self.recentlyViewedMembers = ko.observableArray(getRecentlyViewedMembers());
        self.getMember = function () {
            window.location = appRoutes.admin.member.index + "?memberId=" + this.Id;
        };
        self.clearHistory = function () {
            localStorage.setItem(recentlyViewedMembersStorageKey, 'null');
            self.recentlyViewedMembers([]);
        }
        self.searchHistoryVisible = ko.pureComputed(function () {
            return self.recentlyViewedMembers()[0] != null;
        });
    }

    var vm = new viewModel();

    $("#memberSearch").autocomplete({
        source: appRoutes.admin.member.search,
        minLength: 1,
        select: function (event, ui) {
            saveLastViewedMember(ui.item);
            vm.recentlyViewedMembers.removeAll();
            vm.recentlyViewedMembers(getRecentlyViewedMembers());
            window.location = appRoutes.admin.member.index + "?memberId=" + ui.item.Id;
        }
    })
        .autocomplete("instance")
        ._renderItem = function (ul, item) {
            return $("<li></li>").data("item.autocomplete", item)
                .append('<span>' + item.MembershipNo + ' - ' + item.FirstName + ' ' + item.LastName + '</span>')
                .appendTo(ul);
        };

    function getRecentlyViewedMembers() {
        var members = JSON.parse(localStorage.getItem(recentlyViewedMembersStorageKey));

        if (memberIdToRemove) {
            var newMembersList = _.filter(members, function (x) { return x.Id != memberIdToRemove });
            localStorage.setItem(recentlyViewedMembersStorageKey, JSON.stringify(newMembersList));
            return newMembersList;
        }
        else {
            return members;
        }
    }

    function saveLastViewedMember(member) {
        var members = getRecentlyViewedMembers();

        var newMemberList = _.filter(members, function (x, i) {
            return x.Id !== member.Id && i <= 9;
        });

        newMemberList.unshift(member);

        localStorage.setItem(recentlyViewedMembersStorageKey, JSON.stringify(newMemberList));
    }

    ko.applyBindings(vm, $("#sideMenu")[0]);
}