
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

    var iniValidation = function () {
        form.validate({
            ignore: [],
            rules: {
                AppointmentDate: {
                    required: true
                },
                AppointmentTime: {
                    required: true
                },
                PrimaryReason: {
                    required: true
                },
                DateSymtomsFirstNoted: {
                    required: true
                },
                DescOfCharOfSymtoms: {
                    required: true
                }
            },
            messages: {
                AppointmentDate: "Please select Occupation",
                AppointmentTime: "Please select Occupation",
                PrimaryReason: "Please select Occupation",
                DateSymtomsFirstNoted: "Please select Occupation",
            },
            errorElement: 'span',
            errorPlacement: function (error, element) {
                error.addClass('help-block');
                element.closest('.form-group').append(error);
            },
            highlight: function (element, errorClass, validClass) {
                $(element).closest('.form-group').addClass('has-error');
            },
            unhighlight: function (element, errorClass, validClass) {
                $(element).closest('.form-group').removeClass('has-error');
            },
        });
        $.validator.addMethod("emailCheck", function (value) {
            return /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/.test(value) // consists of only these
        });
    };

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
        $("#btnAdd").on("click", Add);
        $("#btnSave").on("click", Save);
        $("#btnEdit").on("click", Edit);
        //$("#btnDelete").on("click", Delete);

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
                    {
                        "data": null, "searchable": false, "orderable": false,
                        render: function (data, type, full, meta) {
                            var tableControl = '';
                            if (full.AppointmentStatus.AppointmentStatusId === 1 || full.AppointmentStatus.AppointmentStatusId === 2) {
                                tableControl = '<span class="dropdown pmd-dropdown dropup clearfix">'
                                    + '<button class="btn btn-sm pmd-btn-fab pmd-btn-flat pmd-ripple-effect btn-primary" type="button" id="drop-role-' + full.AppointmentId + '" data-toggle="dropdown" aria-expanded="true">'
                                    + '<i class="material-icons pmd-sm">more_vert</i>'
                                    + '</button>'
                                    + '<ul aria-labelledby="drop-role-' + full.AppointmentId + '" role="menu" class="dropdown-menu pmd-dropdown-menu-top-right">'
                                    + '<li role="presentation"><a class="edit" style="color:#000" href="javascript:void(0);" tabindex="-1" data-value="' + full.AppointmentId + '" role="menuitem">Edit</a></li>'
                                    + '<li role="presentation"><a class="cancel" style="color:#000" href="javascript:void(0);" tabindex="-1" data-value="' + full.AppointmentId + '" role="menuitem">Cancel</a></li>'
                                    + '</ul>'
                                    + '</span>';
                            }
                            if (full.AppointmentStatus.AppointmentStatusId === 3) {
                                tableControl = '<span class="dropdown pmd-dropdown dropup clearfix">'
                                    + '<button class="btn btn-sm pmd-btn-fab pmd-btn-flat pmd-ripple-effect btn-primary" type="button" id="drop-role-' + full.AppointmentId + '" data-toggle="dropdown" aria-expanded="true">'
                                    + '<i class="material-icons pmd-sm">more_vert</i>'
                                    + '</button>'
                                    + '<ul aria-labelledby="drop-role-' + full.AppointmentId + '" role="menu" class="dropdown-menu pmd-dropdown-menu-top-right">'
                                    + '<li role="presentation"><a class="edit" style="color:#000" href="javascript:void(0);" tabindex="-1" data-value="' + full.AppointmentId + '" role="menuitem">Edit</a></li>'
                                    + '<li role="presentation"><a class="cancel" style="color:#000" href="javascript:void(0);" tabindex="-1" data-value="' + full.AppointmentId + '" role="menuitem">Cancel</a></li>'
                                    + '</ul>'
                                    + '</span>';
                            }
                            else if (full.AppointmentStatus.AppointmentStatusId === 5) {
                                tableControl = '<span class="dropdown pmd-dropdown dropup clearfix">'
                                    + '<button class="btn btn-sm pmd-btn-fab pmd-btn-flat pmd-ripple-effect btn-primary" type="button" id="drop-role-' + full.AppointmentId + '" data-toggle="dropdown" aria-expanded="true">'
                                    + '<i class="material-icons pmd-sm">more_vert</i>'
                                    + '</button>'
                                    + '<ul aria-labelledby="drop-role-' + full.AppointmentId + '" role="menu" class="dropdown-menu pmd-dropdown-menu-top-right">'
                                    + '<li role="presentation"><a class="edit" style="color:#000" href="javascript:void(0);" tabindex="-1" data-value="' + full.AppointmentId + '" role="menuitem">Edit</a></li>'
                                    + '<li role="presentation"><a class="remove" style="color:#000" href="javascript:void(0);" tabindex="-1" data-value="' + full.AppointmentId + '" role="menuitem">Remove</a></li>'
                                    + '</ul>'
                                    + '</span>';
                            }
                            else {
                                tableControl = '<span class="dropdown pmd-dropdown dropup clearfix">'
                                    + '<button class="btn btn-sm pmd-btn-fab pmd-btn-flat pmd-ripple-effect btn-primary" type="button" id="drop-role-' + full.AppointmentId + '" data-toggle="dropdown" aria-expanded="true">'
                                    + '<i class="material-icons pmd-sm">more_vert</i>'
                                    + '</button>'
                                    + '<ul aria-labelledby="drop-role-' + full.AppointmentId + '" role="menu" class="dropdown-menu pmd-dropdown-menu-top-right">'
                                    + '<li role="presentation"><a class="edit" style="color:#000" href="javascript:void(0);" tabindex="-1" data-value="' + full.AppointmentId + '" role="menuitem">Edit</a></li>'
                                    + '<li role="presentation"><a class="remove" style="color:#000" href="javascript:void(0);" tabindex="-1" data-value="' + full.AppointmentId + '" role="menuitem">Remove</a></li>'
                                    + '</ul>'
                                    + '</span>';
                            }
                            return tableControl;
                        }
                    }
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


    var Add = function () {
        appSettings.status.IsNew = true;
        var appointmentTemplate = $.templates('#appointment-template');
        $("#modal-dialog").find('.modal-title').html('New Appointment');
        $("#modal-dialog").find('.modal-footer #btnSave').html('Save');
        $("#modal-dialog").find('.modal-footer #btnSave').attr("data-name", "Save");

        //reset model 
        appSettings.model.IsNew = true;
        appSettings.model.IsPatientInRecord = false;
        appSettings.model.IsUser = true;
        appSettings.model.SystemUserId = app.appSettings.appState.User.UserId;
        var dateNow = new Date();
        var dateTomorrow = new Date();
        dateTomorrow.setDate(dateNow.getDate() + 1);
        appSettings.model.AppointmentDate = dateTomorrow.getFullYear() + '-' + ((dateTomorrow.getMonth() > 8) ? (dateTomorrow.getMonth() + 1) : ('0' + (dateTomorrow.getMonth() + 1))) + '-' + ((dateTomorrow.getDate() > 9) ? dateTomorrow.getDate() : ('0' + dateTomorrow.getDate()));
        appSettings.model.AppointmentTime = dateNow.getHours() + ":" + dateNow.getMinutes() + ":" + dateNow.getSeconds();
        appSettings.model.DateSymtomsFirstNoted = dateNow.getFullYear() + '-' + ((dateNow.getMonth() > 8) ? (dateNow.getMonth() + 1) : ('0' + (dateNow.getMonth() + 1))) + '-' + ((dateNow.getDate() > 9) ? dateNow.getDate() : ('0' + dateNow.getDate()));
        //end reset model
        //render template
        appointmentTemplate.link("#modal-dialog .modal-body", appSettings.model);

        //init form validation
        form = $('#form-appointment');
        iniValidation();
        //end init form

        $('#AppointmentDate').datetimepicker({
            format: 'MM/DD/YYYY',
            minDate: moment(dateTomorrow),
            date: dateTomorrow,
        });
        $('#DateSymtomsFirstNoted').datetimepicker({
            format: 'MM/DD/YYYY',
            maxDate: moment(dateNow),
            date: dateNow,
        });
        $('#AppointmentTime').datetimepicker({
            format: 'LT',
            date: dateNow
        });
        $("#AppointmentDate").on("focusout", function () {
            appSettings.AdvanceSearchModel.AppointmentDate = $(this).val();
        });
        $("#DateSymtomsFirstNoted").on("focusout", function () {
            appSettings.AdvanceSearchModel.DateSymtomsFirstNoted = $(this).val();
        });
        $("#AppointmentTime").on("focusout", function () {
            appSettings.AdvanceSearchModel.AppointmentTime = $(this).val();
        });
        //show modal
        $("#modal-dialog").modal('show');
        $("body").addClass("modal-open");
        //end show modal

    }
    var Edit = function () {
        if (appSettings.currentId !== null || appSettings.currentId !== undefined || appSettings.currentId !== "") {
            appSettings.status.IsNew = false;
            var appointmentTemplate = $.templates('#appointment-template');
            $("#modal-dialog").find('.modal-title').html('Update Appointment');
            $("#modal-dialog").find('.modal-footer #btnSave').html('Update');
            $("#modal-dialog").find('.modal-footer #btnSave').attr("data-name", "Update");
            circleProgress.show(true);
            api.getById(appSettings.currentId).done(function (data) {
                appSettings.model = $.extend(appSettings.model, data.Data);
                appSettings.detailsModel.AppointmentTime = moment(appSettings.model.AppointmentTime, ["HH:mm"]).format("h: mm A");

                var dateNow = new Date();
                var dateTomorrow = new Date();
                dateTomorrow.setDate(dateNow.getDate() + 1);
                //render template
                appointmentTemplate.link("#modal-dialog .modal-body", appSettings.model);
                //end render template

                $(".select-tags").select2({
                    tags: false,
                    theme: "bootstrap",
                });

                //init form validation
                form = $('#form-appointment');
                iniValidation();
                //end init form
                circleProgress.close();

                $('#AppointmentDate').datetimepicker({
                    format: 'MM/DD/YYYY',
                    minDate: moment(dateTomorrow)
                });
                $('#DateSymtomsFirstNoted').datetimepicker({
                    format: 'MM/DD/YYYY',
                    maxDate: moment(dateNow),
                    date: dateNow
                });
                $('#AppointmentTime').datetimepicker({
                    format: 'LT'
                });
                $("#AppointmentDate").on("focusout", function () {
                    appSettings.AdvanceSearchModel.AppointmentDate = $(this).val();
                });
                $("#DateSymtomsFirstNoted").on("focusout", function () {
                    appSettings.AdvanceSearchModel.DateSymtomsFirstNoted = $(this).val();
                });
                $("#AppointmentTime").on("focusout", function () {
                    appSettings.AdvanceSearchModel.AppointmentTime = $(this).val();
                });
                $("#modal-dialog").modal('show');
                $("body").addClass("modal-open");
                setTimeout(1000, function () {
                    $("body").addClass("modal-open");

                })
            });
        }
    }
    var Save = function (e) {
        if (!form.valid()) {
            return;
        }
        if (appSettings.status.IsNew) {
            Swal.fire({
                title: 'Save appointment',
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
                        var target = $(this);
                        var targetName = target.attr("data-name");
                        target.html(targetName + '&nbsp;<span class="spinner-border spinner-border-sm"></span>');
                        circleProgress.show(true);
                        $.ajax({
                            url: app.appSettings.HRMSAPIURI + "/Appointment/",
                            type: 'POST',
                            dataType: "json",
                            contentType: 'application/json;charset=utf-8',
                            headers: {
                                Authorization: 'Bearer ' + app.appSettings.apiToken
                            },
                            data: JSON.stringify(appSettings.model),
                            success: function (result) {
                                if (result.IsSuccess) {
                                    circleProgress.close();
                                    Swal.fire("Success!", result.Message, "success").then((prompt) => {
                                        circleProgress.show(true);
                                        $(".content").find("input,button,a").prop("disabled", false).removeClass("disabled");
                                        target.empty();
                                        target.html(targetName);
                                        dataTableAppointment.ajax.reload();
                                        circleProgress.close();
                                        $("#modal-dialog").modal('hide');
                                    });
                                } else {
                                    Swal.fire("Error!", result.Message, "error").then((result) => {
                                        $(".content").find("input,button,a").prop("disabled", false).removeClass("disabled");
                                        target.empty();
                                        target.html(targetName);
                                    });
                                }
                            },
                            failure: function (response) {
                                alert(response.responseText);
                            },
                            error: function (data) {
                                var errormessage = "";
                                var errorTitle = "";
                                if (data.responseJSON.Message != null) {
                                    erroTitle = "Error!";
                                    errormessage = data.responseJSON.Message;
                                }
                                if (data.responseJSON.DeveloperMessage != null && data.responseJSON.DeveloperMessage.includes("Cannot insert duplicate")) {
                                    erroTitle = "Not Allowed!";
                                    errormessage = "Already exist!";
                                }
                                $(".content").find("input,button,a").prop("disabled", false).removeClass("disabled");
                                target.empty();
                                target.html(targetName);
                                Swal.fire(erroTitle, errormessage, 'error');
                                circleProgress.close();
                            }
                        });
                    }
                });
        }
        else {
            Swal.fire({
                title: 'Update appointment',
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
                        var target = $(this);
                        var targetName = target.attr("data-name");
                        target.html(targetName + '&nbsp;<span class="spinner-border spinner-border-sm"></span>');
                        circleProgress.show(true);
                        $.ajax({
                            url: app.appSettings.HRMSAPIURI + "/Appointment/",
                            type: "PUT",
                            dataType: "json",
                            contentType: 'application/json;charset=utf-8',
                            data: JSON.stringify(appSettings.model),
                            headers: {
                                Authorization: 'Bearer ' + app.appSettings.apiToken
                            },
                            success: function (result) {
                                if (result.IsSuccess) {
                                    circleProgress.close();
                                    Swal.fire("Success!", result.Message, "success").then((prompt) => {
                                        circleProgress.show(true);
                                        $(".content").find("input,button,a").prop("disabled", false).removeClass("disabled");
                                        target.empty();
                                        target.html(targetName);
                                        dataTableAppointment.ajax.reload();
                                        circleProgress.close();
                                        $("#modal-dialog").modal('hide');
                                    });
                                } else {
                                    Swal.fire("Error!", result.Message, "error").then((result) => {
                                        $(".content").find("input,button,a").prop("disabled", false).removeClass("disabled");
                                        target.empty();
                                        target.html(targetName);
                                    });
                                }
                            },
                            error: function (data) {
                                var errormessage = "";
                                var errorTitle = "";
                                if (data.responseJSON.Message != null) {
                                    erroTitle = "Error!";
                                    errormessage = data.responseJSON.Message;
                                }
                                if (data.responseJSON.DeveloperMessage != null && data.responseJSON.DeveloperMessage.includes("Cannot insert duplicate")) {
                                    erroTitle = "Not Allowed!";
                                    errormessage = "Already exist!";
                                }
                                $(".content").find("input,button,a").prop("disabled", false).removeClass("disabled");
                                target.empty();
                                target.html(targetName);
                                Swal.fire(erroTitle, errormessage, 'error');
                                circleProgress.close();
                            }
                        });
                    }
                });
        }
    }
    
    return {
        init: init
    };
}
var myAppointment = new myAppointmentController;
