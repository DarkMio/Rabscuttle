define(["jquery"],
    (function($) {
        "use strict";

        var TableRenderer = function(data) {
            this.data = data;


        };

        TableRenderer.prototype.render = function() {
            var output = $("<tbody>");
            for(var i = 0; i < this.data.length; i++) {
                var dataRow = this.data[i];
                var row = $("<tr>");
                row
                    .append($("<td>").text(dataRow[0]))
                    .append($("<td>").text(dataRow[1]).addClass('mdl-data-table__cell--non-numeric'))
                    .append($("<td>").text(dataRow[2]).addClass('mdl-data-table__cell--non-numeric'));
                output.append(row);
            }
            return output;
        };

        return TableRenderer;
    })
);
