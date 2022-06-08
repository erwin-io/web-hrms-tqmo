
var itemTypeController = function() {

    var apiService = function (apiURI) {
        var getById = function (Id) {
            return $.ajax({
                url: apiURI + "ItemType/" + Id + "/detail",
                data: { Id: Id },
                type: "GET",
                contentType: 'application/json;charset=utf-8',
                dataType: "json",
                headers: {
                    Authorization: 'Bearer ' + app.appSettings.apiToken
                }
            });
        }
        var getDefaultIconPic = function (Id) {
            return $.ajax({
                url: apiURI + "File/getDefaultItemTypeProfilePic",
                data: null,
                type: "GET",
                contentType: "application/json;charset=utf-8",
                dataType: "json",
                headers: {
                    Authorization: 'Bearer ' + app.appSettings.apiToken
                }
            });
        }

        return {
            getById: getById,
            getDefaultIconPic: getDefaultIconPic
        };
    }
    var api = new apiService(app.appSettings.silupostWebAPIURI);

    var dataTable;
    var appSettings = {
        model: {},
        status:{ IsNew:false},
        currentId:null
    };
    var init = function (obj) {
        initEvent();
        setTimeout(function () {
            initGrid();
            initDefaultIconPic();
        }, 1000);

        

        $(window).resize(function () {
            if ($("#table-itemType").hasClass('collapsed')) {
                $("#btnDelete").removeClass("hidden");
                $("#btnEdit").removeClass("hidden");
            } else {
                $("#btnDelete").addClass("hidden");
                $("#btnEdit").addClass("hidden");
            }
        });
        $(document).ready(function () {
            if ($("#table-itemType").hasClass('collapsed')) {
                $("#btnDelete").removeClass("hidden");
                $("#btnEdit").removeClass("hidden");
            } else {
                $("#btnDelete").addClass("hidden");
                $("#btnEdit").addClass("hidden");
            }
        });
    };

    var initDefaultIconPic = function () {
        api.getDefaultIconPic().done(function (data) {
            appSettings.DefaultIconPic = data.Data;
        });
    }


    var iniValidation = function() {
        form.validate({
            ignore:[],
            rules: {
                ItemTypeName: {
                    required: true
                },
                ItemTypeDescription: {
                    required: true
                }
            },
            messages: {
                ItemTypeName: "Please enter Item Type Type Name",
                ItemTypeDescription: "Please enter Item Type Type Description"
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
        $("#btnEdit").on("click", Edit);
        $("#btnDelete").on("click", Delete);
        // $("#table-itemType tbody").on("click", "tr .dropdown-menu a.edit", Edit);
        // $("#table-itemType tbody").on("click", "tr .dropdown-menu a.remove", Delete);


        $("#table-itemType tbody").on("click", "tr .dropdown-menu a.edit", function () {
            appSettings.currentId = $(this).attr("data-value");
            Edit();
        });
        $("#table-itemType tbody").on("click", "tr .dropdown-menu a.remove", function () {
            appSettings.currentId = $(this).attr("data-value");
            Delete();
        });

        $('#table-itemType tbody').on('click', 'tr', function () {
            appSettings.currentId = dataTable.row(this).data().ItemTypeId;
            var isSelected = !$(this).hasClass('selected');
            if (isSelected && $("#table-itemType").hasClass('collapsed')) {
                $("#btnDelete").removeClass("hidden");
                $("#btnEdit").removeClass("hidden");
            } else {
                $("#btnDelete").addClass("hidden");
                $("#btnEdit").addClass("hidden");
            }
        });
    };

    var initGrid = function() {
        dataTable = $("#table-itemType").DataTable({
            processing: true,
            responsive: true,
            columnDefs: [
                {
                    targets: 0, className:"hidden",
                },
                {
                    targets: [1,4], width:1
                }
            ],
            "columns": [
                { "data": "ItemTypeId","sortable":false, "orderable": false, "searchable": false},
                {
                    "data": null, "searchable": false, "orderable": false,
                    render: function (data, type, full, meta) {
                        var fileData = 'data:' + full.IconFile.MimeType + ';base64,' + full.IconFile.FileContent;
                        return '<image class="btn btn-sm pmd-btn-fab pmd-btn-flat pmd-ripple-effect btn-primary" style="width:50px;height:50px" src="' + fileData + '"></image>'
                    }
                },
                { "data": "ItemTypeName" },
                { "data": "ItemTypeDescription" },
                { "data": null, "searchable": false, "orderable": false, 
                    render: function(data, type, full, meta){
                        return '<span class="dropdown pmd-dropdown dropup clearfix">'
                                +'<button class="btn btn-sm pmd-btn-fab pmd-btn-flat pmd-ripple-effect btn-primary" type="button" id="drop-role-'+full.ItemTypeId+'" data-toggle="dropdown" aria-expanded="true">'
                                    +'<i class="material-icons pmd-sm">more_vert</i>'
                                +'</button>'
                                +'<ul aria-labelledby="drop-role-'+full.ItemTypeId+'" role="menu" class="dropdown-menu pmd-dropdown-menu-top-right">'
                                    +'<li role="presentation"><a class="edit" style="color:#000" href="javascript:void(0);" tabindex="-1" data-value="'+full.ItemTypeId+'" role="menuitem">Edit</a></li>'
                                    +'<li role="presentation"><a class="remove" style="color:#000" href="javascript:void(0);" tabindex="-1" data-value="'+full.ItemTypeId+'" role="menuitem">Remove</a></li>'
                                +'</ul>'
                                +'</span>'
                    }
                }
            ],
            "order": [[0, "asc"]],
            select: {
                style: 'single'
            },
            bFilter: true,
            bLengthChange: true,
            "serverSide": true,
            "ajax": {
                "url": app.appSettings.silupostWebAPIURI + "ItemType/GetPage",
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
                "sLengthMenu": "<div class='itemType-lookup-table-length-menu form-group pmd-textfield pmd-textfield-floating-label'><label>Rows per page:</label>_MENU_</div>",
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
                $(".itemType-lookup-table-length-menu select").select2({
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
        var itemTypeTemplate = $.templates('#itemType-template');
        $("#modal-dialog").find('.modal-title').html('New Item Type Type');
        $("#modal-dialog").find('.modal-footer #btnSave').html('Save');
        $("#modal-dialog").find('.modal-footer #btnSave').attr("data-name","Save");

        //reset model 
        appSettings.model = {
            IconFile: {
                FileName: appSettings.DefaultIconPic.FileName,
                MimeType: appSettings.DefaultIconPic.MimeType,
                FileSize: appSettings.DefaultIconPic.FileSize,
                IsDefault: true,
                FileFromBase64String: appSettings.DefaultIconPic.FileContent,
                FileData: 'data:' + appSettings.DefaultIconPic.MimeType + ';base64,' + appSettings.DefaultIconPic.FileContent
            }
        };
        //end reset model
        //render template
        itemTypeTemplate.link("#modal-dialog .modal-body", appSettings.model);

        //init form validation
        form = $('#form-itemType');
        iniValidation();
        //end init form
        //custom init for ui
        form.find(".group-fields").first().addClass("hidden");
        //end custom init

        //show modal
        $("#modal-dialog").modal('show');
        //end show modal


        $("#IconFilePicker").on("change", async function () {
            var file = $("input[type=file]").get(0).files[0];

            var reader = new FileReader();
            reader.onload = function() {

                if (file && fileValid(file)) {
                    var arrayBuffer = this.result;
                    var fileFromBase64String = arrayBuffer.replace('data:'+file.type+';base64,', '');
                    $("#img-upload").attr("src", reader.result);

                    appSettings.model.IconFile = {
                        FileName: file.name,
                        MimeType: file.type,
                        FileSize: file.size,
                        FileFromBase64String: fileFromBase64String,
                        IsDefault: false
                    }
                }
            }
            reader.readAsDataURL(file);
        });
    }

    var Edit = function () {
        if (appSettings.currentId !== null || appSettings.currentId !== undefined || appSettings.currentId !== "") {
            appSettings.status.IsNew = false;
            var itemTypeTemplate = $.templates('#itemType-template');
            $("#modal-dialog").find('.modal-title').html('Update Item Type Type');
            $("#modal-dialog").find('.modal-footer #btnSave').html('Update');
            $("#modal-dialog").find('.modal-footer #btnSave').attr("data-name","Update");
            circleProgress.show(true);
            api.getById(appSettings.currentId).done(function (data) {
                appSettings.model = data.Data;
                if(appSettings.model.IconFile == null){
                    appSettings.model.IconFile = {
                        FileName: appSettings.DefaultIconPic.FileName,
                        MimeType: appSettings.DefaultIconPic.MimeType,
                        FileSize: appSettings.DefaultIconPic.FileSize,
                        FileContent: appSettings.DefaultIconPic.FileContent,
                        IsDefault: true
                    }
                }

                appSettings.model.IconFile.FileData = 'data:' + appSettings.model.IconFile.MimeType + ';base64,' + appSettings.model.IconFile.FileContent;
                appSettings.model.IconFile.FileFromBase64String = appSettings.model.IconFile.FileContent;

                //render template
                itemTypeTemplate.link("#modal-dialog .modal-body", appSettings.model);
                //end render template
                form = $('#form-itemType');
                iniValidation();
                $("#modal-dialog").modal('show');
                circleProgress.close();


                $("#IconFilePicker").val(null);
                $("#IconFilePicker").on("change", function () {
                    var file = $("input[type=file]").get(0).files[0];

                        if (file && fileValid(file)) {
                        var reader = new FileReader();

                        reader.onload = function () {
                            var arrayBuffer = this.result;
                            var fileFromBase64String = arrayBuffer.replace('data:'+file.type+';base64,', '');
                            $("#img-upload").attr("src", reader.result);
                            appSettings.model.IconFile.FileName = file.name;
                            appSettings.model.IconFile.MimeType = file.type;
                            appSettings.model.IconFile.FileSize = file.size;
                            appSettings.model.IconFile.FileFromBase64String = fileFromBase64String;

                            appSettings.model.IconFile.HasChange = true;
                        }

                        reader.readAsDataURL(file);
                    }
                });

            });
        }
    }


    var fileValid = function (file) {
        var max_size = 10000000;
        if (file.size > max_size) {
            Swal.fire("Not Allowed!", file.name + " exceeds the maximum upload size", 'error');
            return false;
        }

        var extensions = ["jpg", "jpeg", "png"];
        var extension = file.name.replace(/.*\./, '').toLowerCase();

        if (extensions.indexOf(extension) < 0) {
            Swal.fire("Not Allowed!", "File not allowed", 'error');
            return false;
        }
        return true;
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
                        url: app.appSettings.silupostWebAPIURI + "/ItemType/",
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
                        url: app.appSettings.silupostWebAPIURI + "/ItemType/",
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
        if (appSettings.currentId !== null || appSettings.currentId !== undefined || appSettings.currentId !== "") {
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
                        url: app.appSettings.silupostWebAPIURI + "/ItemType/" + appSettings.currentId,
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
                        error: function (result) {
                            var errormessage = "";
                            var errorTitle = "";
                            if (result.responseJSON.Message != null) {
                                erroTitle = "Error!";
                                errormessage = result.responseJSON.Message;
                            }
                            if (result.responseJSON.DeveloperMessage != null && result.responseJSON.DeveloperMessage.includes("Cannot delete")) {
                                erroTitle = "Not Allowed!";
                                errormessage = "Data in used!";
                            }
                            $(".content").find("input,button,a").prop("disabled", false).removeClass("disabled");
                            Swal.fire('Error!', errormessage, 'error');
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
var itemType = new itemTypeController;
