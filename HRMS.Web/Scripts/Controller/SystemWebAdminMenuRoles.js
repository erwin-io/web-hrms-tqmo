
var systemWebAdminMenuRolesController = function() {

    var apiService = function (apiURI) {
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
        var getAll = function (systemWebAdminRoleId,systemWebAdminModuleId) {
            return $.ajax({
                url: apiURI + "SystemWebAdminMenuRoles/getBySystemWebAdminRoleIdAndSystemWebAdminModuleId?SystemWebAdminRoleId=" + systemWebAdminRoleId + "&SystemWebAdminModuleId=" + systemWebAdminModuleId,
                type: "GET",
                contentType: 'application/json;charset=utf-8',
                dataType: "json",
                headers: {
                    Authorization: 'Bearer ' + app.appSettings.apiToken
                }
            });
        }

        return {
            getAll: getAll,
            getLookup: getLookup
        };
    }
    var api = new apiService(app.appSettings.HRMSAPIURI);

    var dataTable,form,systemWebAdminMenuRolesTemplate;
    var appSettings = {
        model: {},
        status:{ IsNew:false},
        currentId:null
    };
    var init = function (obj) {
        appSettings = $.extend(appSettings, obj);
        form = $("#form-systemWebAdminMenuRoles");
        circleProgress.show(false);

        setTimeout(function () {
            initLookup();
            initPrivileges();
            setTimeout(function () {
                appSettings.model.SystemWebAdminMenuRoles = {};
                var selectTemplate = $.templates('#systemWebAdminMenuRoles-select-template');
                selectTemplate.link(".select-container", appSettings.model);
                iniValidation();
                initEvent();
                initGrid();
                loadSystemWebAdminMenuRoles();
                circleProgress.close();
            }, 1000);
        }, 1000);
    };

    var initLookup = function(){
        api.getLookup("SystemWebAdminModule,SystemWebAdminRole").done(function (data) {
        	appSettings.lookup = $.extend(appSettings.lookup, data.Data);
        	appSettings.model = $.extend(appSettings.lookup, data.Data);
        });
    }


    var initPrivileges = function () {
        appSettings.AllowedToUpdateWebAdminMenuRole = false;
        if (app.appSettings.appState.Privileges !== undefined && app.appSettings.appState.Privileges !== null)
            appSettings.AllowedToUpdateWebAdminMenuRole = app.appSettings.appState.Privileges.filter(p => p.SystemWebAdminPrivilegeName.includes("Allowed to update system web admin menu role")).length > 0;

        if (!appSettings.AllowedToUpdateWebAdminMenuRole) {
            $("#btn-save").addClass("hidden");
            $("#btn-save").attr("disabled", "true");
        } else {
            $("#btn-save").removeClass("hidden");
            $("#btn-save").removeAttr("disabled");
        }
    }

    var iniValidation = function() {
        form.validate({
            ignore:[],
            rules: {
                SystemWebAdminRoleId: {
                    required: true
                },
                SystemWebAdminModuleId: {
                    required: true
                }
            },
            messages: {
                SystemWebAdminRoleId: {
                    required: "Please select Role"
                },
                SystemWebAdminModuleId: {
                    required: "Please select Module"
                }
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
        $(".select-simple").select2({
            theme: "bootstrap",
            minimumResultsForSearch: Infinity,
        });
        $("#SystemWebAdminRoleId").on("select2:select", loadSystemWebAdminMenuRoles);
        $("#SystemWebAdminModuleId").on("select2:select", loadSystemWebAdminMenuRoles);
        $("#btn-save").on("click", Save);
        $("#table-systemWebAdminMenuRoles tbody").on('change', 'tr .pmd-checkbox input[type="checkbox"]', setAllowed);
    };

    var initGrid = function() {
        dataTable = $("#table-systemWebAdminMenuRoles").DataTable({
            processing: true,
            responsive: true,
            columnDefs: [
                {
                    targets: 0, className:"hidden",
                },
                {
                    targets: 2,width: 1
                }
            ],
            "columns": [
                { "data": "SystemWebAdminMenuId","sortable":false, "orderable": false, "searchable": false},
                { "data": "SystemWebAdminMenuName"},
                { "data": null, "searchable": false, "orderable": false,
                    render: function (data, type, full, meta) {
                        var controls = '<label class="checkbox-inline pmd-checkbox pmd-checkbox-ripple-effect">'
                                        + '<input data-link="' + full.SystemWebAdminMenuId + '" type="checkbox" ' + (full.IsAllowed ? 'checked="checked"' : '') + ' class="pm-ini">'
                                        + '<span class="pmd-checkbox-label"></span>'
                                        + '</label>';
                        if (!appSettings.AllowedToUpdateWebAdminMenuRole) {
                            controls = '<label class="checkbox-inline pmd-checkbox pmd-checkbox-ripple-effect">'
                                        + '<input disabled data-link="' + full.SystemWebAdminMenuId + '" type="checkbox" ' + (full.IsAllowed ? 'checked="checked"' : '') + ' class="pm-ini">'
                                        + '<span class="pmd-checkbox-label"></span>'
                                        + '</label>';
                        }
                        return controls;
                    }
                }
            ],
            bFilter: true,
            bLengthChange: true,
            "paging": true,
            "searching": true,
            "language": {
                "info": " _START_ - _END_ of _TOTAL_ ",
                "sLengthMenu": "<div class='systemWebAdminMenuRoles-lookup-table-length-menu form-group pmd-textfield pmd-textfield-floating-label'><label>Rows per page:</label>_MENU_</div>",
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
                $(".systemWebAdminMenuRoles-lookup-table-length-menu select").select2({
                    theme: "bootstrap",
                    minimumResultsForSearch: Infinity,
                });
            }
        });
    };

    var loadSystemWebAdminMenuRoles = function(){ 
        if(appSettings.model.SystemWebAdminRoleId != undefined && appSettings.model.SystemWebAdminModuleId != undefined){
            circleProgress.show(false);
            api.getAll(appSettings.model.SystemWebAdminRoleId,appSettings.model.SystemWebAdminModuleId).done(function(data){
                dataTable.clear().draw();
                appSettings.model.SystemWebAdminMenuRoles.SystemWebAdminRoleId = appSettings.model.SystemWebAdminRoleId;
                appSettings.model.SystemWebAdminMenuRoles.SystemWebAdminMenu = [];
                for(var i in data.Data){
                    if (data.Data[i].SystemWebAdminMenu.SystemWebAdminMenuId != undefined)
                    {
                        var obj = {
                            SystemWebAdminMenuId:data.Data[i].SystemWebAdminMenu.SystemWebAdminMenuId,
                            SystemWebAdminMenuName:data.Data[i].SystemWebAdminMenu.SystemWebAdminMenuName,
                            IsAllowed:data.Data[i].IsAllowed
                        };
                        appSettings.model.SystemWebAdminMenuRoles.SystemWebAdminMenu.push(obj);
                        dataTable.row.add(obj).draw();
                        dataTable.columns.adjust();
                    }
                }
                circleProgress.close();
            });
        }
    }

    var setAllowed = function(){
        for (var i in appSettings.model.SystemWebAdminMenuRoles.SystemWebAdminMenu){
            if (appSettings.model.SystemWebAdminMenuRoles.SystemWebAdminMenu[i].SystemWebAdminMenuId == $(this).attr("data-link")){
                appSettings.model.SystemWebAdminMenuRoles.SystemWebAdminMenu[i].IsAllowed = this.checked;
            }
        }
    }

    var Save = function(e){
        if(!form.valid())
            return;
        else{
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
                    circleProgress.show(false);
                    $.ajax({
                        url: app.appSettings.HRMSAPIURI + "SystemWebAdminMenuRoles/SetSystemWebAdminMenuRoles",
                        data: JSON.stringify(appSettings.model.SystemWebAdminMenuRoles),
                        type: "PUT",
		                contentType: 'application/json;charset=utf-8',
		                dataType: "json",
		                headers: {
		                    Authorization: 'Bearer ' + app.appSettings.apiToken
		                },
                        success: function (result) {
                            if (result.IsSuccess) {
                                Swal.fire({
                                    title: "Success!",
                                    text: result.Message + "Changes will effect after logout",
                                    type: "success",
                                    showCancelButton: false,
                                    confirmButtonColor: '#3085d6',  
                                    cancelButtonColor: '#d33',
                                    confirmButtonText: 'OK',
                                    allowOutsideClick: false
                                }).then((result) => {
                                    circleProgress.close();
                                });
                            } else {
                                Swal.fire('Error!',result.Message,'error');
                            }
                            $(".content").find("input,button,a").prop("disabled", false).removeClass("disabled");
                            circleProgress.close();
                        },
                        error: function (errormessage) {
                            $(".content").find("input,button,a").prop("disabled", false).removeClass("disabled");
                            Swal.fire('Error!',errormessage.Message,'error');
                            circleProgress.close();
                        }
                    });
                }
            });
        }
    }

    //Function for clearing the textboxes
    return  {
        init: init
    };
}
var systemWebAdminMenuRoles = new systemWebAdminMenuRolesController;
