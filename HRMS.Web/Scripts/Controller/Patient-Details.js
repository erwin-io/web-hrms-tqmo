
var patientController = function() {

    var apiService = function (apiURI) {
        var getById = function (Id) {
            return $.ajax({
                url: apiURI + "Patient/" + Id + "/detail",
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

        var getDefaultProfilePic = function () {
            return $.ajax({
                url: apiURI + "File/getDefaultSystemUserProfilePic",
                data: null,
                type: "GET",
                contentType: "application/json;charset=utf-8",
                dataType: "json",
                headers: {
                    Authorization: 'Bearer ' + app.appSettings.apiToken
                }
            });
        }

        var getDiagnosisByPatientId = function (patientId) {
            return $.ajax({
                url: apiURI + "Diagnosis/getByPatientId?PatientId=" + patientId,
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
            getDefaultProfilePic: getDefaultProfilePic,
            getDiagnosisByPatientId: getDiagnosisByPatientId,
            getDiagnosisById: getDiagnosisById,
        };
    }
    var api = new apiService(app.appSettings.HRMSAPIURI);

    var form, dataTableDiagnosis;
    var appSettings = {
        model: {},
        diagnosisModel: {},
        status:{ IsNew:false},
        currentId:null
    };
    var init = function (obj) {
        appSettings = $.extend(appSettings, obj);
        appSettings = $.extend(appSettings, app.appSettings);
        initEvent();
        if (appSettings.urlParams["tab"] !== null && appSettings.urlParams["tab"] !== undefined && appSettings.urlParams["tab"].toLowerCase() === "diagnosis")
            $("#tab-control-diagnosis").trigger('click');
        setTimeout(function () {
            initDefaultProfilePic();
            initLookup();
        }, 1000);

    };

    var initDefaultProfilePic = function () {
        api.getDefaultProfilePic().done(function (data) {
            appSettings.DefaultProfilePic = data.Data;
        });
    }

    var initLookup = function(){
        api.getLookup("EntityGender,CivilStatus").done(function (data) {
            appSettings.lookup = $.extend(appSettings.lookup, data.Data);
            initDetails();
        });
    }

    var initEvent = function () {

        $("#table-diagnosis tbody").on("click", "tr .dropdown-menu a.view", function () {
            appSettings.diagnosisModel.DiagnosisId = $(this).attr("data-value");
            if (appSettings.diagnosisModel.DiagnosisId !== null || appSettings.diagnosisModel.DiagnosisId !== undefined || appSettings.diagnosisModel.DiagnosisId !== "") {
                appSettings.diagnosisModel.IsNew = false;
                var diagnosisTemplate = $.templates('#diagnosis-template');
                $("#modal-dialog").find('.modal-title').html('Diagnosis');
                $("#modal-dialog").find('.modal-footer #btnSave').addClass('hidden');
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
        var patientTemplate = $.templates('#patient-template');
        api.getById(appSettings.PatientId).done(function (data) {
            appSettings.model = $.extend(appSettings.model, data.Data);
            appSettings.model = $.extend(appSettings.model, data.Data.LegalEntity);
            appSettings.model = $.extend(appSettings.model, data.Data.CivilStatus);
            appSettings.model = $.extend(appSettings.model, data.Data.LegalEntity.Gender);
            patientTemplate.link("#patientDetailsView", appSettings.model);

            if (dataTableDiagnosis === null || dataTableDiagnosis === undefined)
                initGridDiagnosis();
            LoadDiagnosis();
        });
    }
    var LoadDiagnosis = function () {
        appSettings.model.Diagnosis = [];
        dataTableDiagnosis.clear().draw();
        api.getDiagnosisByPatientId(appSettings.PatientId).done(function (data) {
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
            responsive:true,
            columnDefs: [
                {
                    targets: [5], width: 1
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
var patient = new patientController;
