
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

    var dataTableDiagnosis;
    var appSettings = {
        model: {},
        detailsModel: {},
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
                initDetails();
            }, 1000);
        }
        catch (err) {
            Swal.fire("Error", err, 'error');
        }
    };

    var initEvent = function () {
        $("#table-diagnosis tbody").on("click", "tr .dropdown-menu a.view", function () {
            appSettings.diagnosisModel.DiagnosisId = $(this).attr("data-value");
            if (appSettings.diagnosisModel.DiagnosisId !== null || appSettings.diagnosisModel.DiagnosisId !== undefined || appSettings.diagnosisModel.DiagnosisId !== "") {
                appSettings.diagnosisModel.IsNew = false;
                var diagnosisTemplate = $.templates('#diagnosis-template');
                $("#modal-dialog").find('.modal-title').html('Diagnosis');
                $("#modal-dialog").find('.modal-footer #btnSave').addClass('hidden');
                $("#modal-dialog").find(".modal-footer [data-dismiss]").html("Close");
                circleProgress.show(true);
                api.getDiagnosisById(appSettings.diagnosisModel.DiagnosisId).done(function (data) {
                    appSettings.diagnosisModel = $.extend(appSettings.diagnosisModel, data.Data);
                    appSettings.diagnosisModel.DiagnosisDate = moment(data.Data.DiagnosisDate).format("MM/DD/YYYY");
                    //render template
                    diagnosisTemplate.link("#modal-dialog .modal-body", appSettings.diagnosisModel);
                    //end render template

                    $('#DiagnosisDate').datetimepicker({
                        format: 'MM/DD/YYYY'
                    });
                    //end init form
                    circleProgress.close();

                    $("#modal-dialog").modal('show');
                    $("body").addClass("modal-open");
                    setTimeout(1000, function () {
                        $("body").addClass("modal-open");

                    })
                });
            }
        });
    }

    var initDetails = function () {
        var appointmentStatusTemplate = $.templates('#appointment-status-template');
        var appointmentDetailsTemplate = $.templates('#appointment-details-template');

        api.getById(appSettings.AppointmentId).done(function (data) {
            //appointment details
            appSettings.detailsModel = $.extend(appSettings.model, data.Data);
            appSettings.detailsModel = $.extend(appSettings.detailsModel, data.Data.ProcessedBy);
            appSettings.detailsModel = $.extend(appSettings.detailsModel, data.Data.ProcessedBy.LegalEntity);
            appSettings.detailsModel.PossibleTime = moment(appSettings.detailsModel.PossibleTime, ["HH:mm"]).format("h: mm A");

            appSettings.statusModel = $.extend(appSettings.statusModel, data.Data.AppointmentStatus);
            appSettings.statusModel.IsPending = appSettings.statusModel.AppointmentStatusId === 1;
            appSettings.statusModel.IsProcessed = appSettings.statusModel.AppointmentStatusId === 2;
            appSettings.statusModel.IsApproved = appSettings.statusModel.AppointmentStatusId === 3;
            appSettings.statusModel.IsCompleted = appSettings.statusModel.AppointmentStatusId === 4;
            appSettings.statusModel.IsCanceled = appSettings.statusModel.AppointmentStatusId === 5;
            appSettings.statusModel.IsDeclined = appSettings.statusModel.AppointmentStatusId === 6;


            $(".select-simple").select2({
                theme: "bootstrap",
                minimumResultsForSearch: Infinity,
            });
            appointmentDetailsTemplate.link("#appointmentDetailsView", appSettings.detailsModel);
            appointmentStatusTemplate.link("#appointmentStatusView", appSettings.statusModel);

            circleProgress.close();
            initEvent();

            $('[data-toggle="tooltip"]').tooltip();



            if (dataTableDiagnosis === null || dataTableDiagnosis === undefined)
                initGridDiagnosis();

            if (appSettings.statusModel.IsCompleted) {
                $(".tab-page-diagnosis").removeClass("hidden");
                $(".tab-control-diagnosis").removeClass("hidden");
                LoadDiagnosis();
            }
            else {
                $("#tab-control-appointment").trigger('click');
                $(".tab-page-diagnosis").addClass("hidden");
                $(".tab-control-diagnosis").addClass("hidden");
            }

        });
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
                        return '<span class="dropdown pmd-dropdown dropup clearfix">'
                            + '<button class="btn btn-sm pmd-btn-fab pmd-btn-flat pmd-ripple-effect btn-primary" type="button" id="drop-role-' + full.DiagnosisId + '" data-toggle="dropdown" aria-expanded="true">'
                            + '<i class="material-icons pmd-sm">more_vert</i>'
                            + '</button>'
                            + '<ul aria-labelledby="drop-role-' + full.DiagnosisId + '" role="menu" class="dropdown-menu pmd-dropdown-menu-top-right">'
                            + '<li role="presentation"><a class="view" style="color:#000" href="javascript:void(0);" tabindex="-1" data-value="' + full.DiagnosisId + '" role="menuitem">View</a></li>'
                            + '</ul>'
                            + '</span>';
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
    //Function for clearing the textboxes
    return  {
        init: init
    };
}
var appointmentDetails = new appointmentDetailsController;
