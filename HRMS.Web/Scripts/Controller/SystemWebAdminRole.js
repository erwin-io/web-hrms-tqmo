
var systemWebAdminRoleController = function() {

    var apiService = function (apiURI) {
        var getById = function (Id) {
            return $.ajax({
                url: apiURI + "SystemWebAdminRole/" + Id + "/detail",
                data: { Id: Id },
                type: "GET",
                contentType: 'application/json;charset=utf-8',
                dataType: "json",
                headers: {
                    Authorization: 'Bearer ' + app.appSettings.apiToken
                }
            });
        }

        return {
            getById: getById
        };
    }
    var api = new apiService(app.appSettings.HRMSAPIURI);

    var dataTable;
    var appSettings = {
        model: {},
        status:{ IsNew:false},
        currentId:null
    };
    var init = function (obj) {
        initEvent();
        setTimeout(function () {
            initPrivileges();
            initGrid();
        }, 1000);

    };

    var initPrivileges = function () {

        appSettings.AllowedToAddWebAdminRole = false;
        appSettings.AllowedToUpdateWebAdminRole = false;
        appSettings.AllowedToDeleteWebAdminRole = false;

        if (app.appSettings.appState.Privileges !== undefined && app.appSettings.appState.Privileges !== null) {
            appSettings.AllowedToAddWebAdminRole = app.appSettings.appState.Privileges.filter(p => p.SystemWebAdminPrivilegeName.includes("Allowed to add system web admin role")).length > 0;
            appSettings.AllowedToUpdateWebAdminRole = app.appSettings.appState.Privileges.filter(p => p.SystemWebAdminPrivilegeName.includes("Allowed to update system web admin role")).length > 0;
            appSettings.AllowedToDeleteWebAdminRole = app.appSettings.appState.Privileges.filter(p => p.SystemWebAdminPrivilegeName.includes("Allowed to delete system web admin role")).length > 0;
        }


        if (!appSettings.AllowedToAddWebAdminRole) {
            $("#btnAdd").addClass("hidden");
            $("#btnAdd").attr("disabled", "true");
        } else {
            $("#btnAdd").removeClass("hidden");
            $("#btnAdd").removeAttr("disabled");
        }
    }

    var iniValidation = function() {
        form.validate({
            ignore:[],
            rules: {
                RoleName: {
                    required: true
                }
            },
            messages: {
                RoleName: "Please enter Role Name"
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
        $("#btnAdd").on("click", Add);
        $("#btnSave").on("click", Save);
        $("#table-systemWebAdminRole tbody").on("click", "tr .dropdown-menu a.edit", Edit);
        $("#table-systemWebAdminRole tbody").on("click", "tr .dropdown-menu a.remove", Delete);

    };

    var initGrid = function() {
        dataTable = $("#table-systemWebAdminRole").DataTable({
            processing: true,
            responsive: true,
            columnDefs: [
                {
                    targets: 0, className:"hidden",
                },
                {
                    targets: [2], width:1
                }
            ],
            "columns": [
                { "data": "SystemWebAdminRoleId","sortable":false, "orderable": false, "searchable": false},
                { "data": "RoleName" },
                { "data": null, "searchable": false, "orderable": false, 
                    render: function (data, type, full, meta) {
                        var editCtrl = '<li role="presentation"><a class="edit" style="color:#000" href="javascript:void(0);" tabindex="-1" data-value="' + full.SystemWebAdminRoleId + '" role="menuitem">Edit</a></li>';
                        var deleteCtrl = '<li role="presentation"><a class="remove" style="color:#000" href="javascript:void(0);" tabindex="-1" data-value="' + full.SystemWebAdminRoleId + '" role="menuitem">Remove</a></li>';
                        
                        if (!appSettings.AllowedToUpdateWebAdminRole) {
                            editCtrl = '';
                        }
                        if (!appSettings.AllowedToDeleteWebAdminRole) {
                            deleteCtrl = '';
                        }
                        var controls = '<span class="dropdown pmd-dropdown dropup clearfix">'
                            + '<button class="btn btn-sm pmd-btn-fab pmd-btn-flat pmd-ripple-effect btn-primary" type="button" id="drop-role-' + full.SystemWebAdminRoleId + '" data-toggle="dropdown" aria-expanded="true">'
                            + '<i class="material-icons pmd-sm">more_vert</i>'
                            + '</button>'
                            + '<ul aria-labelledby="drop-role-' + full.SystemWebAdminRoleId + '" role="menu" class="dropdown-menu pmd-dropdown-menu-top-right">'
                            + editCtrl
                            + deleteCtrl
                            + '</ul>'
                            + '</span>';
                        if (!appSettings.AllowedToUpdateWebAdminRole && !appSettings.AllowedToDeleteWebAdminRole) {
                            controls = '<span class="dropdown pmd-dropdown dropup clearfix">'
                            + '<button disabled class="btn btn-sm pmd-btn-fab pmd-btn-flat pmd-ripple-effect btn-primary" type="button" id="drop-role-' + full.SystemWebAdminRoleId + '" data-toggle="dropdown" aria-expanded="true">'
                            + '<i class="material-icons pmd-sm">more_vert</i>'
                            + '</button>'
                            + '</span>';
                        }
                        return controls;
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
                "url": app.appSettings.HRMSAPIURI + "SystemWebAdminRole/GetPage",
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
                        OrderDir: data.order[0].dir
                    }
                    return dataFilter;
                }
            },

            "paging": true,
            "searching": true,
            "language": {
                "info": " _START_ - _END_ of _TOTAL_ ",
                "sLengthMenu": "<div class='systemWebAdminRole-lookup-table-length-menu form-group pmd-textfield pmd-textfield-floating-label'><label>Rows per page:</label>_MENU_</div>",
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
                $(".systemWebAdminRole-lookup-table-length-menu select").select2({
                    theme: "bootstrap",
                    minimumResultsForSearch: Infinity,
                });
                circleProgress.close();
            }
        });
        dataTable.columns.adjust();
    };

    var Add = function(){
        appSettings.status.IsNew = true;
        var systemWebAdminRoleTemplate = $.templates('#systemWebAdminRole-template');
        $("#modal-dialog").find('.modal-title').html('New System Web Admin Role');
        $("#modal-dialog").find('.modal-footer #btnSave').html('Save');
        $("#modal-dialog").find('.modal-footer #btnSave').attr("data-name","Save");

        //reset model 
        appSettings.model = {
        };
        //end reset model
        //render template
        systemWebAdminRoleTemplate.link("#modal-dialog .modal-body", appSettings.model);

        //init form validation
        form = $('#form-systemWebAdminRole');
        setTimeout(function(){
            iniValidation();
            //end init form

            //custom init for ui
            form.find(".group-fields").first().addClass("hidden");
            //end custom init
            //show modal
            $("#modal-dialog").modal('show');
            //end show modal
        }, 1000);

    }

    var Edit = function () {
        if ($(this).attr("data-value") != "") {
            appSettings.status.IsNew = false;
            var systemWebAdminRoleTemplate = $.templates('#systemWebAdminRole-template');
            $("#modal-dialog").find('.modal-title').html('Update System Web Admin Role');
            $("#modal-dialog").find('.modal-footer #btnSave').html('Update');
            $("#modal-dialog").find('.modal-footer #btnSave').attr("data-name","Update");
            circleProgress.show(true);
            api.getById($(this).attr("data-value")).done(function (data) {
                appSettings.model = data.Data;
                //render template
                systemWebAdminRoleTemplate.link("#modal-dialog .modal-body", appSettings.model);
                //end render template
                form = $('#form-systemWebAdminRole');
                iniValidation();
                $("#modal-dialog").modal('show');
                circleProgress.close();


            });
        }
    }
    //Save Data Function 
    var Save = function(e){
        if(!form.valid())
            return;
        if(appSettings.status.IsNew){
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
                    target.html(targetName+'&nbsp;<span class="spinner-border spinner-border-sm"></span>');
                    circleProgress.show(true);
                    $.ajax({
                        url: app.appSettings.HRMSAPIURI + "/SystemWebAdminRole/",
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
                                    dataTable.ajax.reload();
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
        else{
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
                    target.html(targetName+'&nbsp;<span class="spinner-border spinner-border-sm"></span>');
                    circleProgress.show(true);
                    $.ajax({
                        url: app.appSettings.HRMSAPIURI + "/SystemWebAdminRole/",
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
                                    dataTable.ajax.reload();
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

    var Delete = function() {
        if($(this).attr("data-value")!= ""){
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
                    var target = $(this);
                    var targetName = target.attr("data-name");
                    target.html(targetName+'&nbsp;<span class="spinner-border spinner-border-sm"></span>');
                    circleProgress.show(true);
                    $.ajax({
                        url: app.appSettings.HRMSAPIURI + "/SystemWebAdminRole/" + $(this).attr("data-value"),
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
                                    target.empty();
                                    target.html(targetName);
                                    dataTable.ajax.reload();
                                    circleProgress.close();
                                });
                            } else {
                                Swal.fire("Error!", result.Message, "error").then((result) => {
                                    $(".content").find("input,button,a").prop("disabled", false).removeClass("disabled");
                                    target.empty();
                                    target.html(targetName);
                                });
                            }
                        },
                        error: function (errormessage) {
                            $(".content").find("input,button,a").prop("disabled", false).removeClass("disabled");
                            target.empty();
                            target.html(targetName);
                            Swal.fire('Error!',errormessage.Message,'error');
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
var systemWebAdminRole = new systemWebAdminRoleController;
