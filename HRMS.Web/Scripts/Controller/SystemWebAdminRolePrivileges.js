
var systemWebAdminRolePrivilegesController = function() {

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
        var getAll = function (systemWebAdminRoleId) {
            return $.ajax({
                url: apiURI + "SystemWebAdminRolePrivileges/getBySystemWebAdminRoleId?SystemWebAdminRoleId=" + systemWebAdminRoleId,
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

    var dataTable,form;
    var appSettings = {
        model: {},
        status:{ IsNew:false},
        currentId:null
    };
    var init = function (obj) {
        appSettings = $.extend(appSettings, obj);
        form = $("#form-systemWebAdminRolePrivileges");
        circleProgress.show(false);

        initLookup();
    };

    var initLookup = function(){
        api.getLookup("SystemWebAdminRole").done(function (data) {
        	appSettings.lookup = $.extend(appSettings.lookup, data.Data);
            appSettings.model = $.extend(appSettings.lookup, data.Data);

            appSettings.model.SystemWebAdminRolePrivileges = {};
            var selectTemplate = $.templates('#systemWebAdminRolePrivileges-select-template');
            selectTemplate.link(".select-container", appSettings.model);
            iniValidation();
            initEvent();
            initGrid();
            loadSystemWebAdminRolePrivileges();
            circleProgress.close();
        });
    }

    var iniValidation = function() {
        form.validate({
            ignore:[],
            rules: {
                SystemWebAdminRoleId: {
                    required: true
                }
            },
            messages: {
                SystemWebAdminRoleId: {
                    required: "Please select Role"
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
        $("#SystemWebAdminRoleId").on("select2:select", loadSystemWebAdminRolePrivileges);
        $("#btn-save").on("click", Save);
        $("#table-systemWebAdminRolePrivileges tbody").on('change', 'tr .pmd-checkbox input[type="checkbox"]', setAllowed);
    };

    var initGrid = function() {
        dataTable = $("#table-systemWebAdminRolePrivileges").DataTable({
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
                { "data": "SystemWebAdminPrivilegeId","sortable":false, "orderable": false, "searchable": false},
                { "data": "SystemWebAdminPrivilegeName"},
                { "data": null, "searchable": false, "orderable": false,
                    render: function(data, type, full, meta){
                        return '<label class="checkbox-inline pmd-checkbox pmd-checkbox-ripple-effect">'
                            + '<input data-value="' + full.SystemWebAdminPrivilegeId +'" type="checkbox" '+(full.IsAllowed ? 'checked="checked"' : '')+' class="pm-ini">'
                                    +'<span class="pmd-checkbox-label"></span>'
                                +'</label>'
                    }
                }
            ],
            bFilter: true,
            bLengthChange: true,
            "paging": true,
            "searching": true,
            "language": {
                "info": " _START_ - _END_ of _TOTAL_ ",
                "sLengthMenu": "<div class='systemWebAdminRolePrivileges-lookup-table-length-menu form-group pmd-textfield pmd-textfield-floating-label'><label>Rows per page:</label>_MENU_</div>",
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
                $(".systemWebAdminRolePrivileges-lookup-table-length-menu select").select2({
                    theme: "bootstrap",
                    minimumResultsForSearch: Infinity,
                });
            }
        });
    };

    var loadSystemWebAdminRolePrivileges = function(){ 
        if (appSettings.model.SystemWebAdminRoleId !== undefined){
            circleProgress.show(false);
            api.getAll(appSettings.model.SystemWebAdminRoleId).done(function(data){
                dataTable.clear().draw();
                appSettings.model.SystemWebAdminRolePrivileges.SystemWebAdminRoleId = appSettings.model.SystemWebAdminRoleId;
                appSettings.model.SystemWebAdminRolePrivileges.SystemWebAdminPrivilege = [];
                for(var i in data.Data){
                    if (data.Data[i].SystemWebAdminPrivilege.SystemWebAdminPrivilegeId !== undefined)
                    {
                        var obj = {
                            SystemWebAdminPrivilegeId: data.Data[i].SystemWebAdminPrivilege.SystemWebAdminPrivilegeId,
                            SystemWebAdminPrivilegeName: data.Data[i].SystemWebAdminPrivilege.SystemWebAdminPrivilegeName,
                            IsAllowed:data.Data[i].IsAllowed
                        };
                        appSettings.model.SystemWebAdminRolePrivileges.SystemWebAdminPrivilege.push(obj);
                        dataTable.row.add(obj).draw();
                        dataTable.columns.adjust();
                    }
                }
                circleProgress.close();
            });
        }
    }

    var setAllowed = function(){
        for (var i in appSettings.model.SystemWebAdminRolePrivileges.SystemWebAdminPrivilege){
            if (appSettings.model.SystemWebAdminRolePrivileges.SystemWebAdminPrivilege[i].SystemWebAdminPrivilegeId == $(this).attr("data-value")){
                appSettings.model.SystemWebAdminRolePrivileges.SystemWebAdminPrivilege[i].IsAllowed = this.checked;
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
                    circleProgress.show(false);
                    $.ajax({
                        url: app.appSettings.HRMSAPIURI + "SystemWebAdminRolePrivileges/SetSystemWebAdminRolePrivileges",
                        data: JSON.stringify(appSettings.model.SystemWebAdminRolePrivileges),
                        type: "PUT",
		                contentType: 'application/json;charset=utf-8',
		                dataType: "json",
		                headers: {
		                    Authorization: 'Bearer ' + app.appSettings.apiToken
		                },
                        success: function (result) {
                            if (result.IsSuccess) {
                                Swal.fire('Success!', result.Message, 'success');
                                circleProgress.close();
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
var systemWebAdminRolePrivileges = new systemWebAdminRolePrivilegesController;
