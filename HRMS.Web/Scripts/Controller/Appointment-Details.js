
var appointmentDetailsController = function() {

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

        var getDiagnosisByAppointmentId = function (appointmentId) {
            return $.ajax({
                url: apiURI + "Diagnosis/GetByAppointmentId?AppointmentId=" + appointmentId,
                type: "GET",
                contentType: 'application/json;charset=utf-8',
                dataType: "json",
                headers: {
                    Authorization: 'Bearer ' + app.appSettings.apiToken
                }
            });
        }

        var getDiagnosisById = function (Id) {
            return $.ajax({
                url: apiURI + "Diagnosis/" + Id + "/detail",
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
            getLookup: getLookup,
            getDiagnosisByAppointmentId: getDiagnosisByAppointmentId,
            getDiagnosisById: getDiagnosisById,
        };
    }
    var api = new apiService(app.appSettings.HRMSAPIURI);

    var form, formDiagnosis, dataTableDiagnosis;
    var appSettings = {
        model: {},
        detailsModel: {},
        patientModel: {},
        statusModel: {},
        diagnosisModel: {},
        status:{ IsNew:false},
        currentId:null,
    };
    var init = function (obj) {
        circleProgress.show(true);
        try {
            appSettings = $.extend(appSettings, obj);
            setTimeout(function () {
                initLookup();
            }, 1000);
        }
        catch (err) {
            Swal.fire("Error", err, 'error');
        }
    };

    var initLookup = function () {
        api.getLookup("AppointmentStatus").done(function (data) {
            appSettings.lookup = $.extend(appSettings.lookup, data.Data);
            initDetails();
        });
    }

    var initDiagnosisValidation = function () {
        formDiagnosis.validate({
            ignore: [],
            rules: {
                DiagnosisDate: {
                    required: true
                },
                DescOfDiagnosis: {
                    required: true
                },
                DescOfTreatment: {
                    required: true
                },
            },
            messages: {
                DiagnosisDate: "Please enter Date Diagnosed",
                DescOfDiagnosis: "Please enter Description of the Diagnosis",
                DescOfTreatment: "Please enter Description of the Treatment",
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
    };

    var initEvent = function () {

        $("#btnProcess").on("click", function () {
            appSettings.ChangeStatusModel = {
                AppointmentStatusId: 2,
                AppointmentId: appSettings.AppointmentId,
                Description: "Process appointment"
            }
            UpdateAppointmentStatus();
        });

        $("#btnCancelProcess").on("click", function () {
            appSettings.ChangeStatusModel = {
                AppointmentStatusId: 1,
                AppointmentId: appSettings.AppointmentId,
                Description: "Cancel process"
            }
            UpdateAppointmentStatus();
        });

        $("#btnApprove").on("click", function () {
            appSettings.ChangeStatusModel = {
                AppointmentStatusId: 3,
                AppointmentId: appSettings.AppointmentId,
                Description: "Approve appointment"
            }
            UpdateAppointmentStatus();
        });

        $("#btnComplete").on("click", function () {
            appSettings.ChangeStatusModel = {
                AppointmentStatusId: 4,
                AppointmentId: appSettings.AppointmentId,
                Description: "Mark as Completed"
            }
            UpdateAppointmentStatus();
        });

        $("#btnDecline").on("click", function () {
            appSettings.ChangeStatusModel = {
                AppointmentStatusId: 6,
                AppointmentId: appSettings.AppointmentId,
                Description: "Decline this appointment"
            }
            UpdateAppointmentStatus();
        });


        $("#btnNewDiagnosis").on("click", function () {
            appSettings.diagnosisModel.IsNew = true;
            var diagnosisTemplate = $.templates('#diagnosis-template');
            $("#modal-dialog").find('.modal-title').html('New Diagnosis');
            $("#modal-dialog").find('.modal-footer #btnSave').html('Save');
            $("#modal-dialog").find('.modal-footer #btnSave').attr("data-name", "Save");
            $("#modal-dialog").find('.modal-footer #btnSave').removeClass('hidden');

            //reset model 
            appSettings.diagnosisModel = {
                DiagnosisDate: moment(new Date()).format("MM/DD/YYYY"),
                DescOfDiagnosis: null,
                DescOfTreatment: null,
                AppointmentId: appSettings.AppointmentId,
                CanEditDiagnosis: true
            };
            appSettings.diagnosisModel.IsNew = true;
            //end reset model
            //render template
            diagnosisTemplate.link("#modal-dialog .modal-body", appSettings.diagnosisModel);

            $('#DiagnosisDate').datetimepicker({
                format: 'MM/DD/YYYY'
            });


            //init form validation
            formDiagnosis = $('#form-diagnosis');
            initDiagnosisValidation();
            //end init form

            //custom init for ui
            //form.find(".group-fields").first().addClass("hidden");
            //end custom init
            //show modal
            $("#modal-dialog").modal('show');
            $("body").addClass("modal-open");
            $("#DiagnosisDate").on("focusout", function () {
                appSettings.diagnosisModel.DiagnosisDate = $(this).val();
            });
        });
        $("#modal-dialog").find('.modal-footer #btnSave').addClass('hidden');
        $(".modal-footer #btnSave").on("click", SaveDiagnosis);
        $("#table-diagnosis tbody").on("click", "tr .dropdown-menu a.edit", function () {
            appSettings.diagnosisModel.DiagnosisId = $(this).attr("data-value");
            if (appSettings.diagnosisModel.DiagnosisId !== null || appSettings.diagnosisModel.DiagnosisId !== undefined || appSettings.diagnosisModel.DiagnosisId !== "") {
                appSettings.diagnosisModel.IsNew = false;
                var diagnosisTemplate = $.templates('#diagnosis-template');
                appSettings.statusModel.CanEditDiagnosis ? $("#modal-dialog").find('.modal-title').html('Update Diagnosis') : $("#modal-dialog").find('.modal-title').html('Diagnosis');
                $("#modal-dialog").find('.modal-footer #btnSave').html('Update');
                $("#modal-dialog").find('.modal-footer #btnSave').attr("data-name", "Update");
                appSettings.statusModel.CanEditDiagnosis ? $("#modal-dialog").find('.modal-footer #btnSave').removeClass('hidden') : $("#modal-dialog").find('.modal-footer #btnSave').addClass('hidden');
                circleProgress.show(true);
                api.getDiagnosisById(appSettings.diagnosisModel.DiagnosisId).done(function (data) {
                    appSettings.diagnosisModel = $.extend(appSettings.diagnosisModel, data.Data);
                    appSettings.diagnosisModel.DiagnosisDate = moment(data.Data.DiagnosisDate).format("MM/DD/YYYY");
                    appSettings.diagnosisModel.CanEditDiagnosis = appSettings.statusModel.CanEditDiagnosis;
                    //render template
                    diagnosisTemplate.link("#modal-dialog .modal-body", appSettings.diagnosisModel);
                    //end render template

                    $('#DiagnosisDate').datetimepicker({
                        format: 'MM/DD/YYYY'
                    });
                    formDiagnosis = $('#form-diagnosis');
                    initDiagnosisValidation();
                    //end init form
                    circleProgress.close();

                    $("#modal-dialog").modal('show');
                    $("body").addClass("modal-open");
                    setTimeout(1000, function () {
                        $("body").addClass("modal-open");

                    })
                    $("#DiagnosisDate").on("focusout", function () {
                        appSettings.diagnosisModel.DiagnosisDate = $(this).val();
                    });

                    if (appSettings.diagnosisModel.CanEditDiagnosis) {
                        $('#form-diagnosis input').attr('readonly', false);
                    }
                    else {
                        $('#form-diagnosis input').attr('readonly', true);
                    }
                });
            }
        });
        $("#table-diagnosis tbody").on("click", "tr .dropdown-menu a.remove", function () {
            appSettings.diagnosisModel.DiagnosisId = $(this).attr("data-value");
            DeleteDiagnosis();

        });

        $("#table-diagnosis tbody").on("click", "tr .dropdown-menu a.status", function () {
            appSettings.diagnosisModel.DiagnosisId = $(this).attr("data-key");
            let isActive = $(this).attr("data-value");
            appSettings.diagnosisModel.IsActive = (isActive==="true") ? false : true;
            UpdateDiagnosisStatus();

        });
    }

    var initDetails = function () {
        var appointmentStatusTemplate = $.templates('#appointment-status-template');
        var appointmentDetailsTemplate = $.templates('#appointment-details-template');
        var appointmentPatientTemplate = $.templates('#appointment-patient-template');
        appSettings.model.AllowedToProcessedAppointment = false;
        appSettings.model.AllowedToDeclineAppointment = false;
        appSettings.model.AllowedToViewAppointmentDetails = false;
        if (app.appSettings.appState.Privileges !== undefined && app.appSettings.appState.Privileges !== null) {
            appSettings.model.AllowedToProcessedAppointment = app.appSettings.appState.Privileges.filter(p => p.SystemWebAdminPrivilegeName.includes("Allowed to processed appointment")).length > 0;
            appSettings.model.AllowedToDeclineAppointment = app.appSettings.appState.Privileges.filter(p => p.SystemWebAdminPrivilegeName.includes("Allowed to decline appointment")).length > 0;
            appSettings.model.AllowedToViewAppointmentDetails = app.appSettings.appState.Privileges.filter(p => p.SystemWebAdminPrivilegeName.includes("Allowed to view appointment details")).length > 0;
        }

        if (appSettings.model.AllowedToViewAppointmentDetails) {
            $("#appointmentDetailsContainer").removeClass("hidden");
            $("#appointmentNotFoundContainer").addClass("hidden");
            api.getById(appSettings.AppointmentId).done(function (data) {
                //appointment details
                appSettings.detailsModel = $.extend(appSettings.model, data.Data);
                appSettings.detailsModel = $.extend(appSettings.detailsModel, data.Data.ProcessedBy);
                appSettings.detailsModel = $.extend(appSettings.detailsModel, data.Data.ProcessedBy.LegalEntity);
                appSettings.detailsModel.PossibleTime = moment(appSettings.detailsModel.PossibleTime, ["HH:mm"]).format("h: mm A");
                appSettings.detailsModel.lookup = appSettings.lookup;
                //patient
                appSettings.patientModel = data.Data.Patient;
                appSettings.patientModel = $.extend(appSettings.patientModel, data.Data.Patient.LegalEntity);
                appSettings.patientModel = $.extend(appSettings.patientModel, data.Data.Patient.LegalEntity.Gender);
                appSettings.patientModel.lookup = appSettings.lookup;
                $("#btnViewRecentDiagnosis").attr("href", "/Admin/Patients/" + appSettings.patientModel.PatientId + "?tab=diagnosis");
                //status
                appSettings.statusModel = $.extend(appSettings.statusModel, data.Data.AppointmentStatus);
                appSettings.statusModel.IsPending = appSettings.statusModel.AppointmentStatusId === 1;
                appSettings.statusModel.IsProcessed = appSettings.statusModel.AppointmentStatusId === 2;
                appSettings.statusModel.IsApproved = appSettings.statusModel.AppointmentStatusId === 3;
                appSettings.statusModel.IsCompleted = appSettings.statusModel.AppointmentStatusId === 4;
                appSettings.statusModel.IsCanceled = appSettings.statusModel.AppointmentStatusId === 5;
                appSettings.statusModel.IsDeclined = appSettings.statusModel.AppointmentStatusId === 6;
                appSettings.statusModel.lookup = appSettings.lookup;


                appSettings.statusModel.CanProcess = appSettings.statusModel.IsPending && appSettings.model.AllowedToProcessedAppointment;
                appSettings.statusModel.CanCancelProcess = (appSettings.statusModel.IsProcessed || appSettings.statusModel.IsApproved) && appSettings.model.AllowedToProcessedAppointment;
                appSettings.statusModel.CanApprove = appSettings.statusModel.IsProcessed && appSettings.model.AllowedToProcessedAppointment;
                appSettings.statusModel.CanComplete = appSettings.statusModel.IsApproved && appSettings.model.AllowedToProcessedAppointment;
                appSettings.statusModel.CanDecline = (appSettings.statusModel.IsProcessed || appSettings.statusModel.IsApproved) && appSettings.model.AllowedToDeclineAppointment;

                $(".select-simple").select2({
                    theme: "bootstrap",
                    minimumResultsForSearch: Infinity,
                });
                appointmentDetailsTemplate.link("#appointmentDetailsView", appSettings.detailsModel);
                appointmentPatientTemplate.link("#appointmentPatientView", appSettings.patientModel);
                appointmentStatusTemplate.link("#appointmentStatusView", appSettings.statusModel);

                circleProgress.close();
                initEvent();

                $('[data-toggle="tooltip"]').tooltip();



                if (dataTableDiagnosis === null || dataTableDiagnosis === undefined)
                    initGridDiagnosis();
                appSettings.statusModel.CanEditDiagnosis = appSettings.statusModel.CanApprove || appSettings.statusModel.CanComplete;
                appSettings.statusModel.CanEditDiagnosis ? $("#btnNewDiagnosis").removeClass("hidden") : $("#btnNewDiagnosis").addClass("hidden");

                if (appSettings.statusModel.IsPending || appSettings.statusModel.IsCanceled) {
                    $("#tab-control-appointment").trigger('click');
                    $(".tab-page-diagnosis").addClass("hidden");
                    $(".tab-control-diagnosis").addClass("hidden");
                }
                else {
                    $(".tab-page-diagnosis").removeClass("hidden");
                    $(".tab-control-diagnosis").removeClass("hidden");
                    LoadDiagnosis();
                }

            });
        }
        else {
            circleProgress.close();
            $("#appointmentDetailsContainer").addClass("hidden");
            $("#appointmentNotFoundContainer").removeClass("hidden");
        }
    }

    var LoadDiagnosis = function () {
        appSettings.model.Diagnosis = [];
        dataTableDiagnosis.clear().draw();
        api.getDiagnosisByAppointmentId(appSettings.model.AppointmentId).done(function (data) {
            for (var i in data.Data) {
                var model = {
                    DiagnosisId: data.Data[i].DiagnosisId,
                    DiagnosisDate: data.Data[i].DiagnosisDate,
                    DescOfDiagnosis: data.Data[i].DescOfDiagnosis,
                    DescOfTreatment: data.Data[i].DescOfTreatment,
                    IsActive: data.Data[i].IsActive,
                }
                appSettings.model.Diagnosis.push(model);
                dataTableDiagnosis.row.add(model).draw();
            }
            dataTableDiagnosis.columns.adjust();
        });
    }


    var initGridDiagnosis = function () {
        dataTableDiagnosis = $("#table-diagnosis").DataTable({
            processing: true,
            responsive: {
                details: {
                    type: 'column',
                    target: 'tr'
                }
            },
            columnDefs: [
                {
                    targets: [0, 1, 2, 6], width: 1
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
                { "data": "DiagnosisId", "sortable": false, "orderable": false, "searchable": false },
                {
                    "data": "DiagnosisDate",
                    render: function (data, type, full, meta) {
                        var date = new Date(data);
                        var newFormat = date.getFullYear() + '-' + ((date.getMonth() > 8) ? (date.getMonth() + 1) : ('0' + (date.getMonth() + 1))) + '-' + ((date.getDate() > 9) ? date.getDate() : ('0' + date.getDate()))
                        return newFormat;
                    }
                },
                { "data": "DescOfDiagnosis" },
                { "data": "DescOfTreatment" },
                {
                    "data": "IsActive",
                    render: function (data, type, full, meta) {
                        var badgeStatus = '';
                        if (data === true) {
                            return '<span class="badge badge-info" style="padding: 10px">Active</span>';
                        } else
                            return '';
                    }
                },
                {
                    "data": null, "searchable": false, "orderable": false,
                    render: function (data, type, full, meta) {
                        var dropdownMenu = '<span class="dropdown pmd-dropdown dropup clearfix">'
                            + '<button class="btn btn-sm pmd-btn-fab pmd-btn-flat pmd-ripple-effect btn-primary" type="button" id="drop-role-' + full.DiagnosisId + '" data-toggle="dropdown" aria-expanded="true">'
                            + '<i class="material-icons pmd-sm">more_vert</i>'
                            + '</button>'
                            + '<ul aria-labelledby="drop-role-' + full.DiagnosisId + '" role="menu" class="dropdown-menu pmd-dropdown-menu-top-right">'
                            + '<li role="presentation"><a class="edit" style="color:#000" href="javascript:void(0);" tabindex="-1" data-value="' + full.DiagnosisId + '" role="menuitem">View</a></li>'
                            + '</ul>'
                            + '</span>';
                        if (appSettings.statusModel.CanEditDiagnosis) {

                            dropdownMenu = '<span class="dropdown pmd-dropdown dropup clearfix">'
                                + '<button class="btn btn-sm pmd-btn-fab pmd-btn-flat pmd-ripple-effect btn-primary" type="button" id="drop-role-' + full.DiagnosisId + '" data-toggle="dropdown" aria-expanded="true">'
                                + '<i class="material-icons pmd-sm">more_vert</i>'
                                + '</button>'
                                + '<ul aria-labelledby="drop-role-' + full.DiagnosisId + '" role="menu" class="dropdown-menu pmd-dropdown-menu-top-right">'
                                //+ '<li role="presentation"><a class="status" style="color:#000" href="javascript:void(0);" tabindex="-1" data-key="' + full.DiagnosisId + '" data-value="' + full.IsActive + '" role="menuitem">' + (full.IsActive ? 'Set as not Active' : 'Set as Active') + '</a></li>'
                                + '<li role="presentation"><a class="edit" style="color:#000" href="javascript:void(0);" tabindex="-1" data-value="' + full.DiagnosisId + '" role="menuitem">Edit</a></li>'
                                + '<li role="presentation"><a class="remove" style="color:#000" href="javascript:void(0);" tabindex="-1" data-value="' + full.DiagnosisId + '" role="menuitem">Remove</a></li>'
                                + '</ul>'
                                + '</span>'
                        }
                        return dropdownMenu;
                    }
                }
            ],
            "order": [[1, "asc"]],
            select: {
                style: 'single'
            },
            bFilter: false,
            bLengthChange: false,

            "paging": false,
            "searching": false,
            "language": {
                "info": " _START_ - _END_ of _TOTAL_ ",
                "sLengthMenu": "<div class='diagnosis-lookup-table-length-menu form-group pmd-textfield pmd-textfield-floating-label'><label>Rows per page:</label>_MENU_</div>",
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
                $(".diagnosis-lookup-table-length-menu select").select2({
                    theme: "bootstrap",
                    minimumResultsForSearch: Infinity,
                });
                circleProgress.close();
            }
        });
        dataTableDiagnosis.columns.adjust();
    };

    var UpdateAppointmentStatus = function () {
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
                    url: app.appSettings.HRMSAPIURI + "Appointment/UpdateStatus",
                    type: "PUT",
                    contentType: 'application/json;charset=utf-8',
                    data: JSON.stringify(appSettings.ChangeStatusModel),
                    dataType: "json",
                    headers: {
                        Authorization: 'Bearer ' + app.appSettings.apiToken
                    },
                    success: function (result) {
                        if (result.IsSuccess) {
                            circleProgress.close();
                            Swal.fire("Success!", result.Message, "success").then((prompt) => {
                                $(".content").find("input,button,a").prop("disabled", false).removeClass("disabled");
                                circleProgress.close();
                                initDetails();
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


    var SaveDiagnosis = function (e) {
        if (!formDiagnosis.valid()) {
            return;
        }
        if (appSettings.diagnosisModel.IsNew) {
            Swal.fire({
                title: 'Save',
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
                            url: app.appSettings.HRMSAPIURI + "/Diagnosis/",
                            type: 'POST',
                            dataType: "json",
                            contentType: 'application/json;charset=utf-8',
                            headers: {
                                Authorization: 'Bearer ' + app.appSettings.apiToken
                            },
                            data: JSON.stringify(appSettings.diagnosisModel),
                            success: function (result) {
                                if (result.IsSuccess) {
                                    circleProgress.close();
                                    Swal.fire("Success!", result.Message, "success").then((prompt) => {
                                        circleProgress.show(true);
                                        $(".content").find("input,button,a").prop("disabled", false).removeClass("disabled");
                                        target.empty();
                                        target.html(targetName);
                                        LoadDiagnosis();
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
                                $(".content").find("input,button,a").prop("disabled", false).removeClass("disabled");
                                target.empty();
                                target.html(targetName);
                                Swal.fire("Error!", data.responseJSON.DeveloperMessage, 'error');
                                circleProgress.close();
                            }
                        });
                    }
                });
        }
        else {
            Swal.fire({
                title: 'Update',
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
                            url: app.appSettings.HRMSAPIURI + "/Diagnosis/",
                            type: "PUT",
                            dataType: "json",
                            contentType: 'application/json;charset=utf-8',
                            data: JSON.stringify(appSettings.diagnosisModel),
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
                                        LoadDiagnosis();
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
                                $(".content").find("input,button,a").prop("disabled", false).removeClass("disabled");
                                target.empty();
                                target.html(targetName);
                                Swal.fire("Error!", data.responseJSON.DeveloperMessage, 'error');
                                circleProgress.close();
                            }
                        });
                    }
                });
        }
    }


    var DeleteDiagnosis = function () {
        if (appSettings.diagnosisModel.DiagnosisId !== null || appSettings.diagnosisModel.DiagnosisId !== undefined || appSettings.diagnosisModel.DiagnosisId !== "") {
            Swal.fire({
                title: 'Remove',
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
                            url: app.appSettings.HRMSAPIURI + "/Diagnosis/" + appSettings.diagnosisModel.DiagnosisId,
                            type: "DELETE",
                            contentType: 'application/json;charset=utf-8',
                            dataType: "json",
                            headers: {
                                Authorization: 'Bearer ' + app.appSettings.apiToken
                            },
                            success: function (result) {
                                if (result.IsSuccess) {
                                    circleProgress.close();
                                    Swal.fire("Success!", result.Message, "success").then((prompt) => {
                                        circleProgress.show(true);
                                        $(".content").find("input,button,a").prop("disabled", false).removeClass("disabled");
                                        LoadDiagnosis();
                                        circleProgress.close();
                                    });
                                } else {
                                    Swal.fire("Error!", result.Message, "error").then((result) => {
                                        $(".content").find("input,button,a").prop("disabled", false).removeClass("disabled");
                                    });
                                }
                            },
                            error: function (result) {
                                $(".content").find("input,button,a").prop("disabled", false).removeClass("disabled");
                                Swal.fire("Error!", data.responseJSON.DeveloperMessage, 'error');
                                circleProgress.close();
                            }
                        });
                    }
                });
        }
    };

    var UpdateDiagnosisStatus = function () {
        Swal.fire({
            title: appSettings.diagnosisModel.IsActive ? "Set as not Active" : "Set as Active",
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
                        url: app.appSettings.HRMSAPIURI + "Diagnosis/UpdateStatus",
                        type: "PUT",
                        contentType: 'application/json;charset=utf-8',
                        data: JSON.stringify(appSettings.diagnosisModel),
                        dataType: "json",
                        headers: {
                            Authorization: 'Bearer ' + app.appSettings.apiToken
                        },
                        success: function (result) {
                            if (result.IsSuccess) {
                                circleProgress.close();
                                Swal.fire("Success!", result.Message, "success").then((prompt) => {
                                    $(".content").find("input,button,a").prop("disabled", false).removeClass("disabled");
                                    circleProgress.close();
                                    LoadDiagnosis();
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

    //Function for clearing the textboxes
    return  {
        init: init
    };
}
var appointmentDetails = new appointmentDetailsController;
