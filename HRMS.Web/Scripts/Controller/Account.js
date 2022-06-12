
var accountUserController = function() {

    var apiService = function (apiURI) {
        var getById = function (Id) {
            return $.ajax({
                url: apiURI + "SystemUser/" + Id + "/detail",
                type: "GET",
                contentType: 'application/json;charset=utf-8',
                dataType: "json",
                headers: {
                    Authorization: 'Bearer ' + app.appSettings.apiToken
                }
            });
        }
        var getAddressByLegalEntityId = function (legalEntityId) {
            return $.ajax({
                url: apiURI + "SystemUser/GetAddressByLegalEntityId?legalEntityId=" + legalEntityId,
                type: "GET",
                contentType: 'application/json;charset=utf-8',
                dataType: "json",
                headers: {
                    Authorization: 'Bearer ' + app.appSettings.apiToken
                }
            });
        }
        var setAppRefreshToken = function (ApplicationToken) {
            return $.ajax({
                url: "/Account/SetApplicationToken",
                type: "POST",
                data: JSON.stringify({ ApplicationToken: ApplicationToken }),
                contentType: 'application/json;charset=utf-8',
                dataType: "json",
            });
        }
        var setApplicationState = function (applicationState) {
            return $.ajax({
                url: "/Account/SetApplicationState",
                data: JSON.stringify({ ApplicationState: applicationState }),
                type: "POST",
                contentType: 'application/json;charset=utf-8',
                dataType: "json",
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

        return {
            getById: getById,
            getAddressByLegalEntityId: getAddressByLegalEntityId,
            getLookup: getLookup,
            getDefaultProfilePic: getDefaultProfilePic,
            setAppRefreshToken: setAppRefreshToken,
            setApplicationState: setApplicationState,
        };
    }
    var api = new apiService(app.appSettings.HRMSAPIURI);

    var formLegalEntityAddress, dataTableLegalEntityAddress, formChangeUsername, formChangePassword;
    var appSettings = {
        model: {},
        status:{ IsNew:false},
        currentId:null
    };
    var init = function (obj) {

        circleProgress.show(true);

        setTimeout(function () {
            initDefaultProfilePic();
            initLookup();
            setTimeout(function () {
                appSettings.currentId = app.appSettings.appState.User.UserId;
                initDetails();
            }, 1000);
        }, 1000);

        legalEntity.appSettings.forms = {
	        Rules: {
                FirstName: {
                    required: true
                },
                MiddleName: {
                    required: false
                },
                LastName: {
                    required: true
                },
                BirthDate: {
                    required: true
                },
                GenderId: {
                    required: true
                },
                EmailAddress: {
                    required: true,
                    emailCheck: function(){
                        return true;
                    }
                },
                MobileNumber: {
                    required: true,
                    digits: true
                }
            },
            Messages: {
                FirstName: "Please enter a Firstname",
                MiddleName: "Please enter Middlename",
                LastName: "Please enter Lastname",
                BirthDate: "Please select Birth Date",
                GenderId: "Please select Gender",
                EmailAddress: {
                	required: "Please enter Email Address",
                    emailCheck : "Please enter valid email",
                },
                MobileNumber: {
                	required: "Please enter Mobile Number",
                    digits : "Please enter valid Mobile Number",
                }
            },
        }


        $(window).resize(function () {
            if ($("#table-systemUser").hasClass('collapsed')) {
                $("#btnDelete").removeClass("hidden");
                $("#btnEdit").removeClass("hidden");
            } else {
                $("#btnDelete").addClass("hidden");
                $("#btnEdit").addClass("hidden");
            }
        });
        $(document).ready(function () {
            if ($("#table-systemUser").hasClass('collapsed')) {
                $("#btnDelete").removeClass("hidden");
                $("#btnEdit").removeClass("hidden");
            } else {
                $("#btnDelete").addClass("hidden");
                $("#btnEdit").addClass("hidden");
            }
        });
        $.validator.addMethod("pwcheck", function (value) {
            return /^[A-Za-z0-9\d=!\-@._*]*$/.test(value) // consists of only these
                && /[a-z]/.test(value) // has a lowercase letter
                && /[A-Z]/.test(value) // has a uppercase letter
                && /\d/.test(value) // has a digit
        });
        $.validator.addMethod("emailCheck", function (value) {
            return /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/.test(value) // consists of only these
        });

    };

    var initDefaultProfilePic = function () {
        api.getDefaultProfilePic().done(function (data) {
            appSettings.DefaultProfilePic = data.Data;
        });
    }

    var initLookup = function(){
        api.getLookup("SystemWebAdminRole,EntityGender").done(function (data) {
        	appSettings.lookup = $.extend(appSettings.lookup, data.Data);
        });
    }

    var iniValidation = function() {
    };

    var initValidationUsername = function () {
        formChangeUsername.validate({
            ignore: [],
            rules: {
                UserName: {
                    required: true,
                    minlength: 4,
                }
            },
            messages: {
                UserName: {
                    required: "Please enter Username",
                    minlength: $.validator.format("Please enter at least {0} characters."),
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
    }
    var initValidationPassword = function () {
        formChangePassword.validate({
            ignore: [],
            rules: {
                OldPassword: {
                    required: true
                },
                NewPassword: {
                    required: function () {
                        return appSettings.model.IsNew;
                    },
                    minlength: function () {
                        var min = appSettings.model.IsNew ? 6 : 0;
                        return min;
                    },
                    pwcheck: function () {
                        return appSettings.model.IsNew;
                    }
                },
                ConfirmPassword: {
                    required: true,
                    equalTo: function () {
                        return "#NewPassword";
                    }
                }
            },
            messages: {
                OldPassword: {
                    required: "Please enter Old Password"
                },
                NewPassword: {
                    required: "Please enter New Password",
                    minlength: $.validator.format("Please enter at least {0} characters."),
                    pwcheck: "This field must consists of the following : uppercase, uowercase, digit and special characters",
                },
                ConfirmPassword: {
                    required: "Please Confirm Password",
                    equalTo: "Password not match"
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
    }

    var initEvent = function () {
        $("#btnSave").on("click", Save);
        $("#btnManageUsername").on("click", function () {
            var changeUsernameTemplate = $.templates('#changeUsername-template');
            $("#modal-dialog-manageCredentials").find('.modal-title').html('Change Username');
            appSettings.changeUsernameModel = {
                SystemUserId: appSettings.currentId,
                IsChangeUsername: true
            };
            changeUsernameTemplate.link("#modal-dialog-manageCredentials .modal-body-container", appSettings.changeUsernameModel);
            $("#modal-dialog-manageCredentials").modal('show');
            $("body").addClass("modal-open");
            formChangeUsername = $('#form-changeUsername');
            initValidationUsername();
            $("#btnSaveUsername").on("click", SaveUsername);
        });
        $("#btnManagePassword").on("click", function () {
            var changePasswordTemplate = $.templates('#changePassword-template');
            $("#modal-dialog-manageCredentials").find('.modal-title').html('Reset Password');
            appSettings.changePasswordModel = {
                SystemUserId: appSettings.currentId,
                IsChangePassword: true
            };
            changePasswordTemplate.link("#modal-dialog-manageCredentials .modal-body-container", appSettings.changePasswordModel);
            $("#modal-dialog-manageCredentials").modal('show');
            $("body").addClass("modal-open");
            formChangePassword = $('#form-changePassword');
            initValidationPassword();
            $("#btnSavePassword").on("click", SavePassword);
        });
        $("#btnCancel").on("click", function () {
            window.location.replace("/");
        });
    }

    var OpenWebCam = function(){
        $("#camera_View").removeClass("hidden");
        $("#modal-dialog-webcam #btnCapture").removeClass("hidden");
        $("#capture_View").addClass("hidden");
        $("#modal-dialog-webcam #btnReset").addClass("hidden");
        $("#modal-dialog-webcam #btnSave").addClass("hidden");
         Webcam.set({
          width: 320,
          height: 240,
          // // device capture size
          dest_width: 320,
          dest_height: 240,

          // // final cropped size
          crop_width: 240,
          crop_height: 240,

          image_format: 'jpeg',
          jpeg_quality: 200,
          flip_horiz: true


         });
         Webcam.attach('#camera_View');

        $("#modal-dialog-webcam").modal('show');

        //events

        //close camera function
        $('#modal-dialog-webcam').on('hidden.bs.modal', function () {

            const video = document.querySelector('#camera_View video');

            // A video's MediaStream object is available through its srcObject attribute
            const mediaStream = video.srcObject;

            // Through the MediaStream, you can get the MediaStreamTracks with getTracks():
            const tracks = mediaStream.getTracks();

            // Tracks are returned as an array, so if you know you only have one, you can stop it with: 
            tracks[0].stop();

            // Or stop all like so:
            tracks.forEach(track => track.stop())
        });
        //capture
        var cameraCapture = {
            data: null
        }
        $("#modal-dialog-webcam #btnCapture").on("click", function(){
            $("#camera_View").addClass("hidden");
            $("#btnCapture").addClass("hidden");
            $("#btnReset").removeClass("hidden");
            $("#capture_View").removeClass("hidden");
            $("#modal-dialog-webcam #btnSave").removeClass("hidden");
            Webcam.snap( function(data) {
                cameraCapture.data = data;
                $("#capture_View img").attr("src",cameraCapture.data);
            });
        });
        //reset preview
        $("#modal-dialog-webcam #btnReset").on("click", function(){
            $("#camera_View").removeClass("hidden");
            $("#modal-dialog-webcam #btnCapture").removeClass("hidden");

            $("#capture_View").addClass("hidden");
            $("#modal-dialog-webcam #btnReset").addClass("hidden");
            $("#modal-dialog-webcam #btnSave").addClass("hidden");
            
        });
        //save capture
        $("#modal-dialog-webcam #btnSave").on("click", function(){

            var base64String = cameraCapture.data.replace('data:image/jpeg;base64,', '');
            var sizeInBytes = 4 * Math.ceil((base64String.length / 3))*0.5624896334383812;
            var sizeInKb=sizeInBytes/1000;

            
            appSettings.model.ProfilePicture.FileName = "SILUPOST_CAPTURE_" + moment(new Date()).format("YYYY-MM-DD_HH:mm:ss.sss");
            appSettings.model.ProfilePicture.MimeType = "image/jpeg";
            appSettings.model.ProfilePicture.FileSize = parseInt(sizeInKb);
            // appSettings.model.ProfilePicture.IsDefault = false;
            appSettings.model.ProfilePicture.FileFromBase64String = base64String;
            appSettings.model.ProfilePicture.FileData = cameraCapture.data;
            $("#img-upload").attr("src", appSettings.model.ProfilePicture.FileData);
            $('#modal-dialog-webcam').modal('hide');
            $('#ProfilePicturePicker').val('');

        });
    }

    var OpeFile = function () {
        var file = $("#ProfilePicturePicker").get(0).files[0];

        var reader = new FileReader();
        reader.onload = function() {

            if (file && fileValid(file)) {
                var arrayBuffer = this.result;
                var fileFromBase64String = arrayBuffer.replace('data:'+file.type+';base64,', '');
                $("#img-upload").attr("src", reader.result);

            
                appSettings.model.ProfilePicture.FileName = file.name;
                appSettings.model.ProfilePicture.MimeType = file.type;
                appSettings.model.ProfilePicture.FileSize = file.size;
                appSettings.model.ProfilePicture.FileFromBase64String = fileFromBase64String;
                // if(!appSettings.model.ProfilePicture.IsDefault)
                // 	appSettings.model.ProfilePicture.IsDefault = false;
            }
        }
        reader.readAsDataURL(file);
    }

    var initGridLegalEntityAddress = function() {
        dataTableLegalEntityAddress = $("#table-legalEntityAddress").DataTable({
            processing: true,
            responsive: false,
            columnDefs: [
                {
                    targets: 0, className:"hidden",
                },
                {
                    targets: [2], width:1
                }, 
                {
                	className: 'control',
					orderable: false,
					targets:   1
				}
            ],
            "columns": [
                { "data": "LegalEntityAddressId","sortable":false, "orderable": false, "searchable": false},
                { "data": "Address" },
                { "data": null, "searchable": false, "orderable": false, 
                    render: function(data, type, full, meta){
                        var tableControl;
                        if(appSettings.status.IsNew){
                                tableControl = '<button class="btn btn-sm pmd-btn-fab pmd-btn-flat pmd-ripple-effect btn-primary" type="button" id="drop-role-'+full.LegalEntityAddressId+'" data-value='+full.LegalEntityAddressId+' data-toggle="dropdown" aria-expanded="true">'
                                            +'<i class="material-icons pmd-sm">delete</i>'
                                        +'</button>';
                        }
                        else{
                                tableControl = '<span class="dropdown pmd-dropdown dropup clearfix">'
                                        +'<button class="btn btn-sm pmd-btn-fab pmd-btn-flat pmd-ripple-effect btn-primary" type="button" id="drop-role-'+full.LegalEntityAddressId+'" data-toggle="dropdown" aria-expanded="true">'
                                            +'<i class="material-icons pmd-sm">more_vert</i>'
                                        +'</button>'
                                        +'<ul aria-labelledby="drop-role-'+full.LegalEntityAddressId+'" role="menu" class="dropdown-menu pmd-dropdown-menu-top-right">'
                                            +'<li role="presentation"><a class="edit" style="color:#000" href="javascript:void(0);" tabindex="-1" data-value="'+full.LegalEntityAddressId+'" role="menuitem">Edit</a></li>'
                                            +'<li role="presentation"><a class="remove" style="color:#000" href="javascript:void(0);" tabindex="-1" data-value="'+full.LegalEntityAddressId+'" role="menuitem">Remove</a></li>'
                                        +'</ul>'
                                        +'</span>';
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

            "paging": true,
            "searching": true,
            "language": {
                "info": " _START_ - _END_ of _TOTAL_ ",
                "sLengthMenu": "<div class='legalEntityAddress-lookup-table-length-menu form-group pmd-textfield pmd-textfield-floating-label'><label>Rows per page:</label>_MENU_</div>",
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
                $(".legalEntityAddress-lookup-table-length-menu select").select2({
                    theme: "bootstrap",
                    minimumResultsForSearch: Infinity,
                });
                circleProgress.close();
            }
        });
        dataTableLegalEntityAddress.columns.adjust();
    };

    var initDetails = function () {
        if (appSettings.currentId !== null || appSettings.currentId !== undefined || appSettings.currentId !== "") {
            appSettings.status.IsNew = false;
            var systemUserTemplate = $.templates('#systemUser-template');
            $("#modal-dialog").find('.modal-title').html('Update System Web User');
            $("#modal-dialog").find('.modal-footer #btnSave').html('Update');
            $("#modal-dialog").find('.modal-footer #btnSave').attr("data-name","Update");
            api.getById(appSettings.currentId).done(function (data) {
                appSettings.model = {
                    SystemUserId: data.Data.SystemUserId,
                    UserName: data.Data.UserName,
                    Password: data.Data.Password,
                    ConfirmPassword: data.Data.Password
                };
                appSettings.model = $.extend(appSettings.model, data.Data.LegalEntity);
                appSettings.model.BirthDate = moment(data.Data.LegalEntity.BirthDate).format("MM/DD/YYYY");
                appSettings.model.GenderId = data.Data.LegalEntity.Gender.GenderId;
                appSettings.model.ProfilePicture = data.Data.ProfilePicture;
                if (appSettings.model.ProfilePicture === null){

                    appSettings.model.ProfilePicture = {
                        FileId: appSettings.DefaultProfilePic.FileId,
                        FileName: appSettings.DefaultProfilePic.FileName,
                        MimeType: appSettings.DefaultProfilePic.MimeType,
                        FileSize: appSettings.DefaultProfilePic.FileSize,
                        FileContent: appSettings.DefaultProfilePic.FileContent,
                        IsDefault: true,
                        FileFromBase64String: appSettings.DefaultProfilePic.FileContent
                    }
                }
                else{
                    appSettings.model.ProfilePicture.FileFromBase64String = appSettings.model.ProfilePicture.FileContent;
                }

                appSettings.model.ProfilePicture.FileData = 'data:' + appSettings.model.ProfilePicture.MimeType + ';base64,' + appSettings.model.ProfilePicture.FileContent;

                appSettings.model.lookup = {
                    EntityGender: appSettings.lookup.EntityGender,
                    SystemWebAdminRole : []
                };

                for (var i in appSettings.lookup.SystemWebAdminRole) {
                    if (appSettings.lookup.SystemWebAdminRole[i].Id != undefined)
                        appSettings.model.lookup.SystemWebAdminRole.push({ id: appSettings.lookup.SystemWebAdminRole[i].Id, name: appSettings.lookup.SystemWebAdminRole[i].Name });
                }
                var selectedSystemWebAdminUserRoles = [];
                for(var i in data.Data.SystemWebAdminUserRoles){
                    if (data.Data.SystemWebAdminUserRoles[i].SystemWebAdminRole.SystemWebAdminRoleId != undefined)
                        selectedSystemWebAdminUserRoles.push({id:data.Data.SystemWebAdminUserRoles[i].SystemWebAdminRole.SystemWebAdminRoleId, name:data.Data.SystemWebAdminUserRoles[i].SystemWebAdminRole.RoleName});
                }
                appSettings.model.SelectedSystemWebAdminUserRoles = selectedSystemWebAdminUserRoles;
                //render template
                systemUserTemplate.link("#AccountDetails", appSettings.model);
                //end render template

                $(".select-tags").select2({
                    tags: false,
                    theme: "bootstrap",
                });

		        //init form validation
		        legalEntity.init();
		        //end init form
                circleProgress.close();
                initEvent();

                setTimeout(1000, function()
                {
                    $("body").addClass("modal-open");
                    $("#user-details-tab-nav").css("width", "100%");
                    
                })
                $("#ProfilePicturePicker").on("change", OpeFile);

                $("#btnOpenWebCam").on("click", OpenWebCam);

                appSettings.model.LegalEntityAddress = [];
                initGridLegalEntityAddress();
                LoadSystemUserAddress();
                $('#table-legalEntityAddress tbody').on('click', 'tr a.edit', function () {


                    for(var i in appSettings.model.LegalEntityAddress){
                        if(appSettings.model.LegalEntityAddress[i].LegalEntityAddressId == $(this).attr("data-value")){

                            appSettings.LegalEntityAddress = appSettings.model.LegalEntityAddress[i];
                        }
                    }

                    var legalEntityAddressTemplate = $.templates('#legalEntityAddress-template');
                    $("#modal-dialog-legalEntityAddress").find('.modal-title').html('Update Address');
                    $("#modal-dialog-legalEntityAddress").find('.modal-footer #btnSave').html('Update');
                    $("#modal-dialog-legalEntityAddress").find('.modal-footer #btnSave').attr("data-name","Update");

                    legalEntityAddressTemplate.link("#modal-dialog-legalEntityAddress .modal-body", appSettings.LegalEntityAddressModel);

                    //show modal
                    $("#modal-dialog-legalEntityAddress").modal('show');
                    $("body").addClass("modal-open");

                    
                    formLegalEntityAddress = $("#form-legalEntityAddress");

                    initValidationLegalEntityAddress();

                });
                $('#table-legalEntityAddress tbody').on('click', 'tr a.remove', function () {

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
                            circleProgress.show(true);
                            $.ajax({
                                    url: app.appSettings.HRMSAPIURI + "/SystemUser/RemoveSystemUserAddress/" + $(this).attr("data-value"),
                                    type: 'DELETE',
                                    dataType: "json",
                                    contentType: 'application/json;charset=utf-8',
                                    headers: {
                                        Authorization: 'Bearer ' + app.appSettings.apiToken
                                    },
                                    data: JSON.stringify(appSettings.LegalEntityAddressModel),
                                    success: function (result) {
                                        if (result.IsSuccess) {
                                            circleProgress.close();
                                            Swal.fire('Success!',result.Message,'success');
                                            LoadSystemUserAddress();
                                        } else {
                                            Swal.fire('Error!',result.Message,'error');
                                        }
                                    },
                                    failure: function (response) {
                                        alert(response.responseText);
                                    },
                                    error: function (errormessage) {
                                        $(".content").find("input,button,a").prop("disabled", false).removeClass("disabled");
                                        Swal.fire('Error!',errormessage.Message,'error');
                                        circleProgress.close();
                                    }
                                });
                        }
                    });
                });

                $("#modal-dialog-legalEntityAddress #btnSave").on("click", function(){
                    if (!formLegalEntityAddress.valid()) {
                        return;
                    }
                    
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
                        if (result.value){
                            circleProgress.show(true);
                            if(appSettings.LegalEntityAddressModel.IsNew){
                                $.ajax({
                                        url: app.appSettings.HRMSAPIURI + "/SystemUser/createSystemUserAddress",
                                        type: 'POST',
                                        dataType: "json",
                                        contentType: 'application/json;charset=utf-8',
                                        headers: {
                                            Authorization: 'Bearer ' + app.appSettings.apiToken
                                        },
                                        data: JSON.stringify(appSettings.LegalEntityAddressModel),
                                        success: function (result) {
                                            if (result.IsSuccess) {
                                                circleProgress.close();
                                                Swal.fire("Success!", result.Message, "success").then((prompt) => {
                                                    $("#modal-dialog-legalEntityAddress").modal("hide");
                                                    LoadSystemUserAddress();
                                                });
                                            } else {
                                                Swal.fire("Error!", result.Message, "error").then((result) => {
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
                                            Swal.fire(erroTitle, errormessage, 'error');
                                            circleProgress.close();
                                        }
                                    });
                            }
                            else{

                                $.ajax({
                                        url: app.appSettings.HRMSAPIURI + "/SystemUser/UpdateSystemUserAddress",
                                        type: 'PUT',
                                        dataType: "json",
                                        contentType: 'application/json;charset=utf-8',
                                        headers: {
                                            Authorization: 'Bearer ' + app.appSettings.apiToken
                                        },
                                        data: JSON.stringify(appSettings.LegalEntityAddressModel),
                                        success: function (result) {
                                            if (result.IsSuccess) {
                                                circleProgress.close();
                                                Swal.fire("Success!", result.Message, "success").then((prompt) => {
                                                    $("#modal-dialog-legalEntityAddress").modal("hide");
                                                    LoadSystemUserAddress();
                                                });
                                            } else {
                                                Swal.fire("Error!", result.Message, "error").then((result) => {
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
                                            Swal.fire(erroTitle, errormessage, 'error');
                                            circleProgress.close();
                                        }
                                    });
                            }
                                
                        }
                    });
                });
                $("#tab-page-legalEntityAddress #btnNewAddress").on("click", function(){
                    var legalEntityAddressTemplate = $.templates('#legalEntityAddress-template');
                    $("#modal-dialog-legalEntityAddress").find('.modal-title').html('New Address');
                    $("#modal-dialog-legalEntityAddress").find('.modal-footer #btnSave').html('Save');
                    $("#modal-dialog-legalEntityAddress").find('.modal-footer #btnSave').attr("data-name","Save");

                    appSettings.LegalEntityAddressModel = { LegalEntityId: appSettings.model.LegalEntityId};
                    appSettings.LegalEntityAddressModel.IsNew = true;
                    legalEntityAddressTemplate.link("#modal-dialog-legalEntityAddress .modal-body", appSettings.LegalEntityAddressModel);

                    //show modal
                    $("#modal-dialog-legalEntityAddress").modal('show');
                    $("body").addClass("modal-open");

                    formLegalEntityAddress = $("#form-legalEntityAddress");

                    initValidationLegalEntityAddress();
                });
            });
        }
    }

    var LoadSystemUserAddress = function(){
        appSettings.model.LegalEntityAddress = [];
        dataTableLegalEntityAddress.clear().draw();
        api.getAddressByLegalEntityId(appSettings.model.LegalEntityId).done(function (data) {
            for(var i in data.Data){
                appSettings.LegalEntityAddressModel = {
                    LegalEntityAddressId:data.Data[i].LegalEntityAddressId,
                    Address:data.Data[i].Address
                }
                appSettings.model.LegalEntityAddress.push(appSettings.LegalEntityAddressModel);
                dataTableLegalEntityAddress.row.add(appSettings.LegalEntityAddressModel).draw();
            }
        });
    }

    var initValidationLegalEntityAddress = function(){

            formLegalEntityAddress.validate({
                ignore:[],
                rules: {
                    Address: {
                        required: true
                    }
                },
                messages: {
                    Address: {
                        required: "Please enter Address",
                        minlength: $.validator.format("Please enter at least {0} characters."),
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
                }
            });
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
        if (!legalEntity.valid()) {
            $("#tab-control-legalentity").trigger('click');
            return;
        }

        Swal.fire({
            title: 'Update',
            text: "Do you want to continue!",
            type: 'question',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes',
            allowOutsideClick: false
        }).then((result) => {
            if (result.value) {
                $(".content").find("input,button,a").prop("disabled", true).addClass("disabled");
                circleProgress.show(true);
                $.ajax({
                    url: app.appSettings.HRMSAPIURI + "/SystemUser/UpdatePersonalDetails",
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
                                $(".content").find("input,button,a").prop("disabled", false).removeClass("disabled");
                                window.location.replace("/Account");
                            });
                        } else {
                            Swal.fire("Error!", result.Message, "error").then((result) => {
                                $(".content").find("input,button,a").prop("disabled", false).removeClass("disabled");
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
                        Swal.fire(erroTitle, errormessage, 'error');
                        circleProgress.close();
                    }
                });
            }
        });
    }
    var SaveUsername = function (e) {
        if (!formChangeUsername.valid()) {
            return;
        }

        Swal.fire({
            title: 'Update Username and Logout',
            text: "Do you want to continue!",
            type: 'question',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes',
            allowOutsideClick: false
        }).then((result) => {
            if (result.value) {
                $(".content").find("input,button,a").prop("disabled", true).addClass("disabled");
                circleProgress.show(true);
                $.ajax({
                    url: app.appSettings.HRMSAPIURI + "/SystemUser/UpdateUsername",
                    type: "PUT",
                    dataType: "json",
                    contentType: 'application/json;charset=utf-8',
                    data: JSON.stringify(appSettings.changeUsernameModel),
                    headers: {
                        Authorization: 'Bearer ' + app.appSettings.apiToken
                    },
                    success: function (result) {
                        if (result.IsSuccess) {
                            circleProgress.close();
                            Swal.fire("Success!", result.Message, "success").then((prompt) => {
                                $(".content").find("input,button,a").prop("disabled", false).removeClass("disabled");
                                api.setAppRefreshToken(null).done(function () {
                                    api.setApplicationState(null).done(function () {
                                        window.location.replace("/Account/Login");
                                    });
                                });
                            });
                        } else {
                            Swal.fire("Error!", result.Message, "error").then((result) => {
                                $(".content").find("input,button,a").prop("disabled", false).removeClass("disabled");
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
                            errormessage = "Username Already taken!";
                        }
                        $(".content").find("input,button,a").prop("disabled", false).removeClass("disabled");
                        Swal.fire(erroTitle, errormessage, 'error');
                        circleProgress.close();
                    }
                });
            }
        });
    }
    var SavePassword = function (e) {
        if (!formChangePassword.valid()) {
            return;
        }

        Swal.fire({
            title: 'Update Password and Logout',
            text: "Do you want to continue!",
            type: 'question',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes',
            allowOutsideClick: false
        }).then((result) => {
            if (result.value) {
                $(".content").find("input,button,a").prop("disabled", true).addClass("disabled");
                circleProgress.show(true);
                $.ajax({
                    url: app.appSettings.HRMSAPIURI + "/SystemUser/UpdatePassword",
                    type: "PUT",
                    dataType: "json",
                    contentType: 'application/json;charset=utf-8',
                    data: JSON.stringify(appSettings.changePasswordModel),
                    headers: {
                        Authorization: 'Bearer ' + app.appSettings.apiToken
                    },
                    success: function (result) {
                        if (result.IsSuccess) {
                            circleProgress.close();
                            Swal.fire("Success!", result.Message, "success").then((prompt) => {
                                $(".content").find("input,button,a").prop("disabled", false).removeClass("disabled");
                                api.setAppRefreshToken(null).done(function () {
                                    api.setApplicationState(null).done(function () {
                                        window.location.replace("/Account/Login");
                                    });
                                });
                            });
                        } else {
                            Swal.fire("Error!", result.Message, "error").then((result) => {
                                $(".content").find("input,button,a").prop("disabled", false).removeClass("disabled");
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
                        if (data.responseJSON.Message != null && data.responseJSON.Message.includes("No record found")) {
                            erroTitle = "Not Allowed!";
                            errormessage = "Old password is incorrect";
                        }
                        $(".content").find("input,button,a").prop("disabled", false).removeClass("disabled");
                        Swal.fire(erroTitle, errormessage, 'error');
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
var account = new accountUserController;
