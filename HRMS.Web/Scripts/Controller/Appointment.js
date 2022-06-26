
var appointmentController = function() {

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

    var form,formLegalEntityAddress,dataTableAppointment;
    var appSettings = {
        model: {},
        status:{ IsNew:false},
        currentId:null,
        IsAdvanceSearchMode: false
    };
    var init = function (obj) {
        setTimeout(function () {
            initPrivileges();
            initLookup();  
        }, 1000);

        $(window).resize(function () {
            if ($("#table-appointment").hasClass('collapsed')) {
                $("#btnMore").removeClass("hidden");
            } else {
                $("#btnMore").addClass("hidden");
            }
        });
        $(document).ready(function () {
            if ($("#table-appointment").hasClass('collapsed')) {
                $("#btnMore").removeClass("hidden");
            } else {
                $("#btnMore").addClass("hidden");
            }
        });
    };

    var initPrivileges = function () {
        appSettings.model.AllowedToProcessedAppointment = false;
        appSettings.model.AllowedToDeclineAppointment = false;
        appSettings.model.AllowedToViewAppointmentDetails = false;
        if (app.appSettings.appState.Privileges !== undefined && app.appSettings.appState.Privileges !== null) {
            appSettings.model.AllowedToProcessedAppointment = app.appSettings.appState.Privileges.filter(p => p.SystemWebAdminPrivilegeName.includes("Allowed to processed appointment")).length > 0;
            appSettings.model.AllowedToDeclineAppointment = app.appSettings.appState.Privileges.filter(p => p.SystemWebAdminPrivilegeName.includes("Allowed to decline appointment")).length > 0;
            appSettings.model.AllowedToViewAppointmentDetails = app.appSettings.appState.Privileges.filter(p => p.SystemWebAdminPrivilegeName.includes("Allowed to view appointment details")).length > 0;
        }
    }

    var initLookup = function(){
        api.getLookup("DocReportMediaType,AppointmentStatus").done(function (data) {
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

    var initAdvanceSearchMode = function(){

        var advanceSearchModeTemplate = $.templates('#advanceSearch-template');
        var date = new Date();

        var _dateFrom = date.getFullYear() + '-' + ((date.getMonth() > 8) ? (date.getMonth() + 1) : ('0' + (date.getMonth() + 1))) + '-' + ((date.getDate() > 9) ? date.getDate() : ('0' + date.getDate()));
        var _dateTo = date.getFullYear() + '-' + ((date.getMonth() > 8) ? (date.getMonth() + 1) : ('0' + (date.getMonth() + 1))) + '-' + ((date.getDate() > 9) ? date.getDate() : ('0' + date.getDate()));

        appSettings.AdvanceSearchModel = {
            AppointmentId: "",
            Patient: "",
            AppointmentDateFrom: moment(_dateFrom).format("MM/DD/YYYY"),
            AppointmentDateTo: moment(_dateTo).format("MM/DD/YYYY"),
            ProcessedBy: ""
        };
        advanceSearchModeTemplate.link("#advanceSearchView", appSettings.AdvanceSearchModel);
        initGrid();
    }

    var initEvent = function () {
        $("#btnToggleAdvanceSearch").on("click", function(){
            if(appSettings.IsAdvanceSearchMode){
                appSettings.IsAdvanceSearchMode = false;
                $("#form-advanceSearch").addClass('hidden');
                $("#table-appointment_filter").removeClass('hidden');
            }
            else{
                appSettings.IsAdvanceSearchMode = true;
                $("#form-advanceSearch").removeClass('hidden');
                $("#table-appointment_filter").addClass('hidden');
            }
            try {
                
                dataTableAppointment.ajax.reload();
            }
            catch(err) {
                Swal.fire("Error", err, 'error');
            }
        });
        $("#btnAdvanceSearch").on("click", function(){
            try {
                dataTableAppointment.ajax.reload();
            }
            catch(err) {
                Swal.fire("Error", err, 'error');
            }
        });

        $("#AppointmentStatusId").on("change", function(){

            try {
                
                appSettings.FilterSearchModel.AppointmentStatusId = $(this).val();
                dataTableAppointment.ajax.reload();
            }
            catch(err) {
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
        $('#table-appointment tbody').on('click', 'tr', function () {
            if (dataTableAppointment.row(this).data()) {
                appSettings.currentId = dataTableAppointment.row(this).data().AppointmentId;
                var isSelected = !$(this).hasClass('selected');
                if (isSelected && appSettings.currentId !== null && appSettings.currentId !== undefined && appSettings.currentId !== "") {
                    appSettings.ChangeStatusModel = {
                        AppointmentStatusId: dataTableAppointment.row(this).data().AppointmentStatus.AppointmentStatusId,
                        AppointmentId: dataTableAppointment.row(this).data().AppointmentId
                    }
                    appSettings.ChangeStatusControl = {
                        CanOpen: false,
                        CanProcess: false,
                        CanCancelProcess: false,
                        CanApprove: false,
                        CanComplete: false,
                        CanDecline: false,
                    }
                    if (appSettings.ChangeStatusModel.AppointmentStatusId === 1) {
                        appSettings.ChangeStatusControl.CanOpen = true;
                        appSettings.ChangeStatusControl.CanProcess = true;
                    } else if (appSettings.ChangeStatusModel.AppointmentStatusId === 2) {
                        appSettings.ChangeStatusControl.CanOpen = true;
                        appSettings.ChangeStatusControl.CanCancelProcess = true;
                        appSettings.ChangeStatusControl.CanApprove = true;
                        appSettings.ChangeStatusControl.CanDecline = true;
                    } else if (appSettings.ChangeStatusModel.AppointmentStatusId === 3) {
                        appSettings.ChangeStatusControl.CanOpen = true;
                        appSettings.ChangeStatusControl.CanCancelProcess = true;
                        appSettings.ChangeStatusControl.CanComplete = true;
                        appSettings.ChangeStatusControl.CanDecline = true;
                    } else if (appSettings.ChangeStatusModel.AppointmentStatusId === 4) {
                        appSettings.ChangeStatusControl.CanOpen = true;
                    } else {
                        appSettings.ChangeStatusControl.CanOpen = true;
                    }

                    appSettings.model.AllowedToViewAppointmentDetails && appSettings.ChangeStatusControl.CanOpen ?
                        $("#btnOpen").removeClass("hidden") : $("#btnOpen").addClass("hidden");

                    appSettings.model.AllowedToProcessedAppointment && appSettings.ChangeStatusControl.CanProcess ?
                        $("#btnProcess").removeClass("hidden") : $("#btnProcess").addClass("hidden");

                    appSettings.model.AllowedToProcessedAppointment && appSettings.ChangeStatusControl.CanCancelProcess ?
                        $("#btnCancelProcess").removeClass("hidden") : $("#btnCancelProcess").addClass("hidden");

                    appSettings.model.AllowedToProcessedAppointment && appSettings.ChangeStatusControl.CanApprove ?
                        $("#btnApprove").removeClass("hidden") : $("#btnApprove").addClass("hidden");

                    appSettings.model.AllowedToProcessedAppointment && appSettings.ChangeStatusControl.CanComplete ?
                        $("#btnComplete").removeClass("hidden") : $("#btnComplete").addClass("hidden");

                    appSettings.model.AllowedToDeclineAppointment && appSettings.ChangeStatusControl.CanDecline ?
                        $("#btnDecline").removeClass("hidden") : $("#btnDecline").addClass("hidden");


                    $("#btnOpen").on("click", function () {
                        window.location.href = "/Admin/Appointments/" + appSettings.ChangeStatusModel.AppointmentId;
                    });
                    $("#btnProcess").on("click", function () {
                        appSettings.ChangeStatusModel.AppointmentStatusId = 2;
                        appSettings.ChangeStatusModel.Description = "Process appointment";
                        UpdateAppointmentStatus();
                    });
                    $("#btnCancelProcess").on("click", function () {
                        appSettings.ChangeStatusModel.AppointmentStatusId = 1;
                        appSettings.ChangeStatusModel.Description = "Cancel process";
                        UpdateAppointmentStatus();
                    });
                    $("#btnApprove").on("click", function () {
                        appSettings.ChangeStatusModel.AppointmentStatusId = 3;
                        appSettings.ChangeStatusModel.Description = "Approve appointment";
                        UpdateAppointmentStatus();
                    });
                    $("#btnComplete").on("click", function () {
                        appSettings.ChangeStatusModel.AppointmentStatusId = 4;
                        appSettings.ChangeStatusModel.Description = "Mark appointment as complete";
                        UpdateAppointmentStatus();
                    });
                    $("#btnDecline").on("click", function () {
                        appSettings.ChangeStatusModel.AppointmentStatusId = 6;
                        appSettings.ChangeStatusModel.Description = "Decline appointment";
                        UpdateAppointmentStatus();
                    });
                } else {
                    $("#btnOpen").addClass("hidden");
                    $("#btnProcess").addClass("hidden");
                    $("#btnCancelProcess").addClass("hidden");
                    $("#btnApprove").addClass("hidden");
                    $("#btnComplete").addClass("hidden");
                    $("#btnDecline").addClass("hidden");
                }
            }
        });
    }

    var initGrid = function() {
        try {

            dataTableAppointment = $("#table-appointment").DataTable({
                processing: true,
                responsive: {
                    details: {
                        type: 'column',
                        target: 'tr'
                    }
                },
                columnDefs: [
                    {
                        targets: [0,1,2], width:1
                    },
                    {
                        className: 'control',
                        orderable: false,
                        targets:   0
                    }
                ],
                "columns": [
                    { "data": "","sortable":false, "orderable": false, "searchable": false,
                        render: function (data, type, full, meta) {
                            return '';
                        }
                    },
                    { 
                        "data": "AppointmentId",
                        render: function (data, type, full, meta)
                        {
                            return appSettings.model.AllowedToViewAppointmentDetails ?
                                '<a href="/Admin/Appointments/' + full.AppointmentId + '" title="View details">' + full.AppointmentId + '</a>' : '<a>' + full.AppointmentId + '</>';
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
                    { "data": "Patient.LegalEntity.FullName" },
                    { "data": "Patient.LegalEntity.CompleteAddress" },
                    { 
                        "data": "AppointmentStatus.AppointmentStatusName",
                        render: function(data, type, full, meta){
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
                            return '<span class="badge '+ badgeStatus +'" style="padding: 10px">' + data + '</span>';
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
                    "url": app.appSettings.HRMSAPIURI + "Appointment/getPage",
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
                            AppointmentId: appSettings.AdvanceSearchModel.AppointmentId,
                            AppointmentStatusId: appSettings.FilterSearchModel.AppointmentStatusId,
                            Patient: appSettings.AdvanceSearchModel.Patient,
                            AppointmentDateFrom: appSettings.AdvanceSearchModel.AppointmentDateFrom,
                            AppointmentDateTo: appSettings.AdvanceSearchModel.AppointmentDateTo,
                            ProcessedBy: appSettings.AdvanceSearchModel.ProcessedBy
                        }
                        return dataFilter;
                    }
                },

                "paging": true,
                "searching": true,
                "language": {
                    "info": " _START_ - _END_ of _TOTAL_ ",
                    "sLengthMenu": "<div class='appointment-lookup-table-length-menu form-group pmd-textfield pmd-textfield-floating-label'><label>Rows per page:</label>_MENU_</div>",
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
                    $(".appointment-lookup-table-length-menu select").select2({
                        theme: "bootstrap",
                        minimumResultsForSearch: Infinity,
                    });
                    circleProgress.close();
                }
            });
            $(".custom-select-action").html('<button class="btn btn-sm pmd-btn-fab pmd-btn-flat pmd-ripple-effect btn-primary" type="button"><i class="material-icons pmd-sm">delete</i></button><button class="btn btn-sm pmd-btn-fab pmd-btn-flat pmd-ripple-effect btn-primary" type="button"><i class="material-icons pmd-sm">more_vert</i></button>');
            dataTableAppointment.columns.adjust();
        }
        catch(err) {
            Swal.fire("Error", err, 'error');
        }
    };

    var UpdateAppointmentStatus = function () {
        if (appSettings.currentId !== null || appSettings.currentId !== undefined || appSettings.currentId !== "") {
            Swal.fire({
                title: appSettings.ChangeStatusModel.Description,
                text: "Do you want to continue!",
                type: 'question',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Yes',
                allowOutsideClick: false
            })
                .then((result) => {
                    if (result.value) {
                        $(".content").find("input,button,a").prop("disabled", true).addClass("disabled");
                        circleProgress.show(true);
                        $.ajax({
                            url: app.appSettings.HRMSAPIURI + "/Appointment/UpdateStatus/",
                            type: "PUT",
                            contentType: 'application/json;charset=utf-8',
                            dataType: "json",
                            headers: {
                                Authorization: 'Bearer ' + app.appSettings.apiToken
                            },
                            data: JSON.stringify(appSettings.ChangeStatusModel),
                            success: function (result) {
                                if (result.IsSuccess) {
                                    circleProgress.close();
                                    appSettings.currentId = null;
                                    $("#btnOpen").removeClass("hidden");
                                    $("#btnProcess").addClass("hidden");
                                    $("#btnCancelProcess").addClass("hidden");
                                    $("#btnApprove").addClass("hidden");
                                    $("#btnComplete").addClass("hidden");
                                    $("#btnDecline").addClass("hidden");
                                    Swal.fire("Success!", result.Message, "success").then((prompt) => {
                                        circleProgress.show(true);
                                        $(".content").find("input,button,a").prop("disabled", false).removeClass("disabled");
                                        dataTableAppointment.ajax.reload();
                                        circleProgress.close();
                                        if (appSettings.ChangeStatusModel.AppointmentStatusId === 1 || appSettings.ChangeStatusModel.AppointmentStatusId === 2)
                                            window.location.href = "/Admin/Appointments/" + selectedRow.AppointmentId;
                                    });
                                } else {
                                    Swal.fire("Error!", result.Message, "error").then((result) => {
                                        $(".content").find("input,button,a").prop("disabled", false).removeClass("disabled");
                                    });
                                }
                            },
                            error: function (result) {
                                $(".content").find("input,button,a").prop("disabled", false).removeClass("disabled");
                                Swal.fire("Error!", result.responseJSON.DeveloperMessage, 'error');
                                circleProgress.close();
                            }
                        });
                    }
                });
        }
    };

    //Function for clearing the textboxes
    return  {
        init: init
    };
}
var appointment = new appointmentController;
