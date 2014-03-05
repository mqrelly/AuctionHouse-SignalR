var auctionHub;

$(function () {
    var profileName = prompt('What\'s your name?', '');

    auctionHub = $.connection.auctionHub;
    auctionHub.client.errorMessage = errorMessage;
    auctionHub.client.auctionUpdated = auctionUpdated;
    auctionHub.client.auctionCanceled = auctionCanceled;
    auctionHub.client.auctionClosed = auctionClosed;

    $.connection.hub.qs = { profileName: profileName };
    $.connection.hub.start().done(init);

    function init() {
        $.connection.hub.reconnecting(function () {
            infoMessage('Connection issues... reconnecting');
        });

        $.connection.hub.reconnected(function () {
            successMessage('Connection repaired');
            auctionHub.server.resetProfileName(profileName).done(refreshAuctions());
        });

        $.connection.hub.disconnected(function () {
            errorMessage('Connection lost');
        });

        $('#new-auction-button').click(function() {
            createAuction();
            return false;
        });
        $('#auctions-list-refresh').click(function() {
            refreshAuctions();
            return false;
        });

        $('.profile-name').text(profileName);

        refreshAuctions();
    }

    function infoMessage(message, klass) {
        var msgEl = $('.flash-message');
        msgEl.removeClass('error success');
        msgEl.text(message);
        if (klass) {
            msgEl.addClass(klass);
        }

        msgEl.show();
    }

    function errorMessage(message) {
        infoMessage(message, 'error');
    }

    function successMessage(message) {
        infoMessage(message, 'success');
    }

    function refreshAuctions() {
        $('.auctions-list').children().remove();

        auctionHub.server.getAuctions().done(function(auctions) {
            for (var key in auctions) {
                auctionUpdated(auctions[key]);
            }
        });
    }

    function auctionUpdated(auction) {
        var item = $('#auction-item-' + auction.id);

        if (item.length === 0) {
            item = createItemElement(auction);
        }

        item.find('.current-price').text(auction.currentPrice);

        if (auction.lastBidderId === auctionHub.connection.id) {
            item.find('.was-bidding').addClass('true');
            item.removeClass('loosing');
            item.addClass('winning');
        } else {
            if (item.find('.was-bidding').hasClass('true')) {
                item.removeClass('winning');
                item.addClass('loosing');
            }
        }

        function createItemElement(auction) {
            var item = $('<li>', {
                id: 'auction-item-' + auction.id,
                'class': 'auction-item'
            }).append($('<span>', {
                'class': 'name',
                text: auction.name
            })).append($('<span>', {
                'class': 'current-price'
            })).append($('<span>', {
                'class': 'was-bidding'
            }));

            if (auction.ownerId === auctionHub.connection.id) {
                var cancel = $('<a>', {
                    text: 'Cancel',
                    href: '#',
                    click: function() {
                        auctionHub.server.cancelAuction(auction.id);
                        return false;
                    }
                });

                var close = $('<a>', {
                    text: 'Close',
                    href: '#',
                    click: function() {
                        auctionHub.server.closeAuction(auction.id);
                        return false;
                    }
                });

                item.append(close).append(cancel);
                item.addClass('owned');
            } else {
                var bidInput = $('<input>', { 'class': 'bid price' });
                var bidLink = $('<a>', {
                    text: 'Bid',
                    href: '#',
                    click: function() {
                        var bidPrice = +bidInput.val();
                        if (isNaN(bidPrice)) {
                            return false;
                        }

                        auctionHub.server.bid(auction.id, bidPrice);

                        bidInput.val('');

                        return false;
                    }
                });

                item.append(bidInput).append(bidLink);
            }

            $('.auctions-list').append(item);

            return item;
        }
    }

    function createAuction() {
        var nameEl = $('#new-auction-item-name');
        var priceEl = $('#new-auction-item-price');
        var name = nameEl.val();
        var price = +priceEl.val();

        if (name === '' || isNaN(price) || price <= 0) {
            errorMessage('Fill out the fields correctly, please!');
            return;
        }

        auctionHub.server.createAuction({
            name: name,
            price: price
        });

        nameEl.val('');
        priceEl.val('');
    }

    function auctionCanceled(auction) {
        var item = $('#auction-item-' + auction.id);
        if (item.length === 0) {
            return;
        }

        item.removeClass('winning loosing owned');
        item.addClass('closed');

        setTimeout(function() {
            item.remove();
        }, 500);

        infoMessage('Auction "' + action.name + '" (#' + auction.id + ') was canceled.');
    }

    function auctionClosed(auction) {
        var item = $('#auction-item-' + auction.id);
        if (item.length === 0) {
            return;
        }

        item.removeClass('winning loosing owned');

        if (auction.winnerId === auctionHub.connection.id) {
            item.addClass('winning');

            successMessage('Congratulations! ' + auction.name + ' is yours now.');
        } else {
            item.addClass('closed');

            infoMessage('Auction for ' + auction.name + ' closed.');
        }

        setTimeout(function () {
            item.remove();
        }, 500);
    }
});