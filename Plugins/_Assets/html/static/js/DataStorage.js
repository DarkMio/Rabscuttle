define(["jquery"],
    (function($) {
        "use strict";

        var DataStorage = function() {
            var scope = this;

            this.data = [];
            this.perMonthData = [];
            this.dataPromise = this.loadData();
        };

        DataStorage.prototype.loadData = function() {
            var scope = this;
            return $.getJSON("static/data/quotes.json", function(data) {
                scope.data = data;
            })
        };

        DataStorage.prototype.aggregate = function() {


            for(var i = 0; i < this.data.length; i++) {
                this.data[i][0] = this.data[i][0] + 1;
                this.data[i][1] = this.data[i][1].replace("<", "&lt").replace(">", "&gt").replace("||", "<br>");
            }


            this.monthlyStats();
        };

        DataStorage.prototype.monthlyStats = function() {
            if(!this.data) {
                return;
            }

            var compare = function(a, b) {
                if(a[3] < b[3]) {
                    return -1;
                }
                if(a[3] > b[3]) {
                    return 1;
                }
                return 0;
            };

            var data = this.data = this.data.sort(compare);
            var secondsPerDay = 24 * 60 * 60 * 30.5;
            var object = {};
            for(var i = 0; i < data.length; i++) {
                var key = Math.floor(data[i][3] / secondsPerDay);
                var val = object[key] || 0;
                object[key] = val + 1;
            }
            this.perMonthData = object;
        };

        DataStorage.prototype.generateDailyDataset = function() {
            var array = [];
            var min = Infinity;
            for (var k in this.perMonthData) {
                var key = parseInt(k);
                min = Math.min(min, key);
                array.push({x: key, y: this.perMonthData[k]});
            }


            for(var i = 0; i < array.length; i++) {
                array[i]['x'] = array[i]['x'] - min;
            }

            var compare = function(a, b) {
                if(a['x'] < b['x']) {
                    return -1;
                }
                if(a['x'] > b['x']) {
                    return 1;
                }
                return 0;
            };

            array = array.sort(compare);
            return array;
        };

        return DataStorage;
    })
);
