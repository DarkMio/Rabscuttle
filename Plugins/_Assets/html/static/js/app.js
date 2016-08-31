requirejs.config({
    paths: {
        'jquery': [
            'https://ajax.googleapis.com/ajax/libs/jquery/3.1.0/jquery.min',
            'static/js/lib/jquery-3.1.0.min'
        ],
        'jquery-dataTable': 'lib/jquery.dataTables.min',
        'material': [
            'https://cdnjs.cloudflare.com/ajax/libs/material-design-lite/1.2.0/material',
            'static/js/lib/material.min'
        ],
        'DataStorage': 'DataStorage',
        'TableRenderer': 'TableRenderer',
        'chartjs': [
            'https://cdnjs.cloudflare.com/ajax/libs/Chart.js/2.2.2/Chart.bundle.min',
            'lib/chart.min'
        ],
    },
    shim: {
        'material': {
            deps: ['jquery']
        },
        'jquery-dataTable': {
            deps: ['jquery']
        }
    }
});

define(['jquery', 'material', 'DataStorage', 'TableRenderer', 'jquery-dataTable', 'chartjs'],
    (function($, mat, DataStorage, TableRenderer, DataTable, Chart){
        "use strict";

        $(document).ready(function() {

            var storage = new DataStorage();
            storage.dataPromise.then(function() {
                storage.aggregate();

                $('#table').dataTable({
                    'data': storage.data,
                    'columns': [
                        {
                            title: 'Index',
                            searchable: false
                        },
                        {
                            title: 'Quote <input class="mdl-button mdl-button--accent search" placeholder="SEARCH">',
                            className: 'mdl-data-table__cell--non-numeric'
                        },
                        {
                            title: 'Name',
                            className: 'mdl-data-table__cell--non-numeric'
                        }
                    ],
                    "lengthChange": false,
                    "searching": false,
                    "info": false,
                    "pagingType": "full",
                    "ordering": false,
                    });

                    var height = $("#chartspace").height();
                    var ctx = $("#chartspace")[0].getContext('2d');
                    ctx.canvas.height = 176;
                    ctx.canvas.width = $("#chartspace").width();
                    Chart.defaults.global.responsive = false;
                    var chart = new Chart(ctx, {
                        type: 'line',
                        maintainAspectRatio: false,
                        data: {
                            datasets: [{
                                data: storage.generateDailyDataset()
                            }]
                        },
                        options: {
                            legend: {
                                display: false
                            },
                            scales: {
                                xAxes: [{
                                    display: false,
                                    type: 'linear',
                                    position: 'bottom'
                                }],
                                yAxes: [{
                                    display: false
                                }]
                            }
                        }
                    });
            });
            /*
            console.log("Hello world.");
            var storage = new DataStorage();
            storage.dataPromise.done(function() {
                var renderer = new TableRenderer(storage.dataPromise.responseJSON);
                $("#table").append(renderer.render()).DataTable({
                    "lengthChange": false,
                    "searching": false,
                    "info": false,
                    "pagingType": "full"
                });

            })
            */
        })
    })
);
