
var myAppointmentController = function () {

    var apiService = function (apiURI) {
        var getById = function (Id) {
            return $.ajax({
                url: apiURI + "Appointment/" + Id + "/detail",
                type: "GET",
                contentType: 'application/json;charset=utf-8',
                dataType: "json",
                headers: {
                    Authorization: 'Bearer ' + app.appSettings.apiToken
                }
            });
        }

        var getLookup = function (tableNames) {
            return $.ajax({
                url: apiURI + "SystemLookup/GetAllByTableNames?TableNames=" + tableNames,
                type: "GET",
                contentType: 'application/json;charset=utf-8',
                dataType: "json",
                headers: {
                    Authorization: 'Bearer ' + app.appSettings.apiToken
                }
            });
        }

        return {
            getById: getById,
            getLookup: getLookup
        };
    }
    var api = new apiService(app.appSettings.HRMSAPIURI);

    var form, dataTableAppointment;
    var appSettings = {
        model: {},
        status: { IsNew: false },
        currentId: null,
        IsAdvanceSearchMode: false
    };
    var init = function (obj) {
        setTimeout(function () {
            initLookup();
        }, 1000);

        $(window).resize(function () {
            if ($("#table-myAppointment").hasClass('collapsed')) {
                $("#btnMore").removeClass("hidden");
            } else {
                $("#btnMore").addClass("hidden");
            }
        });
        $(document).ready(function () {
            if ($("#table-myAppointment").hasClass('collapsed')) {
                $("#btnMore").removeClass("hidden");
            } else {
                $("#btnMore").addClass("hidden");
            }
        });
    };

    var initLookup = function () {
        api.getLookup("AppointmentStatus").done(function (data) {
            appSettings.lookup = [];
            appSettings.lookup = $.extend(appSettings.lookup, data.Data);
            appSettings.FilterSearchModel = {
                lookup: appSettings.lookup,
                AppointmentStatusId: ""
            };
            var filterAppointmentStatusTemplate = $.templates('#filterAppointmentStatus-template');
            filterAppointmentStatusTemplate.link("#filterAppointmentStatusView", appSettings.FilterSearchModel);
            
            initAdvanceSearchMode();
            initEvent();

        });
    }

    var initAdvanceSearchMode = function () {

        var advanceSearchModeTemplate = $.templates('#advanceSearch-template');
        var date = new Date();

        var _dateFrom = date.getFullYear() + '-' + ((date.getMonth() > 8) ? (date.getMonth() + 1) : ('0' + (date.getMonth() + 1))) + '-' + ((date.getDate() > 9) ? date.getDate() : ('0' + date.getDate()));
        var _dateTo = date.getFullYear() + '-' + ((date.getMonth() > 8) ? (date.getMonth() + 1) : ('0' + (date.getMonth() + 1))) + '-' + ((date.getDate() > 9) ? date.getDate() : ('0' + date.getDate()));

        appSettings.AdvanceSearchModel = {
            AppointmentId: "",
            AppointmentDateFrom: moment(_dateFrom).format("MM/DD/YYYY"),
            AppointmentDateTo: moment(_dateTo).format("MM/DD/YYYY"),
        };
        advanceSearchModeTemplate.link("#advanceSearchView", appSettings.AdvanceSearchModel);
        initGrid();
    }

    var initEvent = function () {
        $("#btnToggleAdvanceSearch").on("click", function () {
            if (appSettings.IsAdvanceSearchMode) {
                appSettings.IsAdvanceSearchMode = false;
                $("#form-advanceSearch").addClass('hidden');
                $("#table-myAppointment_filter").removeClass('hidden');
            }
            else {
                appSettings.IsAdvanceSearchMode = true;
                $("#form-advanceSearch").removeClass('hidden');
                $("#table-myAppointment_filter").addClass('hidden');
            }
            try {

                dataTableAppointment.ajax.reload();
            }
            catch (err) {
                Swal.fire("Error", err, 'error');
            }
        });
        $("#btnAdvanceSearch").on("click", function () {
            try {
                dataTableAppointment.ajax.reload();
            }
            catch (err) {
                Swal.fire("Error", err, 'error');
            }
        });

        $("#AppointmentStatusId").on("change", function () {

            try {

                appSettings.FilterSearchModel.AppointmentStatusId = $(this).val();
                dataTableAppointment.ajax.reload();
            }
            catch (err) {
                Swal.fire("Error", err, 'error');
            }
        });

        $('#AppointmentDateFrom').datetimepicker({
            format: 'MM/DD/YYYY'
        });
        $('#AppointmentDateTo').datetimepicker({
            format: 'MM/DD/YYYY'
        });

        $('#AppointmentDateFrom').parent().addClass('pmd-textfield-floating-label-completed');
        $('#AppointmentDateTo').parent().addClass('pmd-textfield-floating-label-completed');


        $("#AppointmentDateFrom").on("focusout", function () {
            appSettings.AdvanceSearchModel.AppointmentDateFrom = $(this).val();
        });
        $("#AppointmentDateTo").on("focusout", function () {
            appSettings.AdvanceSearchModel.AppointmentDateTo = $(this).val();
        });

        $(".select-simple").select2({
            theme: "bootstrap",
            minimumResultsForSearch: Infinity,
        });
        $('.select-simple').parent().addClass('pmd-textfield-floating-label-completed');
    }

    var initGrid = function () {
        try {

            dataTableAppointment = $("#table-myAppointment").DataTable({
                processing: true,
                responsive: {
                    details: {
                        type: 'column',
                        target: 'tr'
                    }
                },
                columnDefs: [
                    {
                        targets: [0, 3], width: 1
                    },
                    {
                        className: 'control',
                        orderable: false,
                        targets: 0
                    }
                ],
                "columns": [
                    {
                        "data": "", "sortable": false, "orderable": false, "searchable": false,
                        render: function (data, type, full, meta) {
                            return '';
                        }
                    },
                    {
                        "data": "AppointmentId",
                        render: function (data, type, full, meta) {
                            return '<a href="/MyAppointments/' + full.AppointmentId + '" title="View details">' + full.AppointmentId + '</a>';
                        }
                    },
                    {
                        "data": "AppointmentDate",
                        render: function (data, type, full, meta) {
                            var date = new Date(data);
                            var newFormat = date.getFullYear() + '-' + ((date.getMonth() > 8) ? (date.getMonth() + 1) : ('0' + (date.getMonth() + 1))) + '-' + ((date.getDate() > 9) ? date.getDate() : ('0' + date.getDate()))
                            return newFormat + ' ' + full.AppointmentTime;
                        }
                    },
                    {
                        "data": "AppointmentStatus.AppointmentStatusName",
                        render: function (data, type, full, meta) {
                            var badgeStatus = 'badge-warning';
                            if (full.AppointmentStatus.AppointmentStatusId === 2) {
                                badgeStatus = 'badge-warning';
                            } else if (full.AppointmentStatus.AppointmentStatusId === 3) {
                                badgeStatus = 'badge-info';
                            } else if (full.AppointmentStatus.AppointmentStatusId === 4) {
                                badgeStatus = 'badge-success';
                            } else if (full.AppointmentStatus.AppointmentStatusId === 5) {
                                badgeStatus = 'badge-error';
                            } else if (full.AppointmentStatus.AppointmentStatusId === 6) {
                                badgeStatus = 'badge-error';
                            } else {
                                badgeStatus = 'badge-warning';
                            }
                            return '<span class="badge ' + badgeStatus + '" style="padding: 10px">' + data + '</span>';
                        }
                    },
                ],
                "order": [[1, "asc"]],
                select: {
                    style: 'single'
                },
                bFilter: true,
                bLengthChange: true,
                "serverSide": true,
                "ajax": {
                    "url": app.appSettings.HRMSAPIURI + "Appointment/GetPageBySystemUserId",
                    "type": "GET",
                    "datatype": "json",
                    contentType: 'application/json;charset=utf-8',
                    headers: {
                        Authorization: 'Bearer ' + app.appSettings.apiToken
                    },
                    data: function (data) {
                        var dataFilter = {
                            Draw: data.draw,
                            Search: data.search.value,
                            PageNo: data.start <= 0 ? data.start + 1 : (data.start / data.length) + 1,//must be added to 1
                            PageSize: data.length,
                            OrderColumn: data.columns[data.order[0].column].data,
                            OrderDir: data.order[0].dir,
                            IsAdvanceSearchMode: appSettings.IsAdvanceSearchMode,
                            CreatedBy: app.appSettings.appState.User.UserId,
                            AppointmentId: appSettings.AdvanceSearchModel.AppointmentId,
                            AppointmentStatusId: appSettings.FilterSearchModel.AppointmentStatusId,
                            AppointmentDateFrom: appSettings.AdvanceSearchModel.AppointmentDateFrom,
                            AppointmentDateTo: appSettings.AdvanceSearchModel.AppointmentDateTo
                        }
                        return dataFilter;
                    }
                },

                "paging": true,
                "searching": true,
                "language": {
                    "info": " _START_ - _END_ of _TOTAL_ ",
                    "sLengthMenu": "<div class='myAppointment-lookup-table-length-menu form-group pmd-textfield pmd-textfield-floating-label'><label>Rows per page:</label>_MENU_</div>",
                    "sSearch": "",
                    "sSearchPlaceholder": "Search",
                    "paginate": {
                        "sNext": " ",
                        "sPrevious": " "
                    },
                },
                dom: "<'pmd-card-title'<'data-table-responsive pull-left'><'search-paper pmd-textfield'f>>" +
                    "<'row'<'col-sm-12'tr>>" +
                    "<'pmd-card-footer' <'pmd-datatable-pagination' l i p>>",
                "initComplete": function (settings, json) {
                    $(".myAppointment-lookup-table-length-menu select").select2({
                        theme: "bootstrap",
                        minimumResultsForSearch: Infinity,
                    });
                    circleProgress.close();
                }
            });
            $(".custom-select-action").html('<button class="btn btn-sm pmd-btn-fab pmd-btn-flat pmd-ripple-effect btn-primary" type="button"><i class="material-icons pmd-sm">delete</i></button><button class="btn btn-sm pmd-btn-fab pmd-btn-flat pmd-ripple-effect btn-primary" type="button"><i class="material-icons pmd-sm">more_vert</i></button>');
            dataTableAppointment.columns.adjust();
        }
        catch (err) {
            Swal.fire("Error", err, 'error');
        }
    };

    //Function for clearing the textboxes
    return {
        init: init
    };
}
var myAppointment = new myAppointmentController;
