var loginController = function(){

    var apiService = function (apiURI){
        var getUserByCredentials = function(model)
        {
            return $.ajax({
                url: apiURI + "SystemUser/GetByCredentials?Username=" + model.Username + "&Password=" + model.Password,
                type: "GET",
                contentType: "application/json;charset=utf-8",
                dataType: "json",
            });
        }
        var setApplicationState = function (ApplicationState) {
            return $.ajax({
                url: "/Account/SetApplicationState",
                data: JSON.stringify(ApplicationState),
                type: "POST",
                contentType: 'application/json;charset=utf-8',
                dataType: "json",
            });
        }

        return {
            getUserByCredentials: getUserByCredentials,
            setApplicationState: setApplicationState
        };
    }

    var api,form;
    var appSettings = {
        model : {}
    }
    var init = function () {
        api = new apiService(app.appSettings.POSWebAPIURI);
        setTimeout(function () {
            var loginTemplate = $.templates('#login-template');
            loginTemplate.link("#accountcard", appSettings.model);
            form = $("#form-login");
            initValidation();
            initEvent();
        }, 0);
	}


    var initValidation = function() {
        form.validate({
            ignore:[],
            rules: {
                Username: {
                    required: true
                },
                Password: {
                    required: true
                }
            },
            messages: {
                Username: {
                    required: "Please enter Username"
                },
                Password: {
                    required: "Please enter Password"
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
        $("#btn-login").on("click", Login);
    };

    var Login = function(){
        if(!form.valid())
            return;
        else {

            circleProgress.show(false);
            api.getUserByCredentials(appSettings.model)
                .done(function (result) {
                    if (result.IsSuccess) {
                        $(".content").find("input,button,a").prop("disabled", false).removeClass("disabled");
                        console.log(result);
                        var appState = {
                            User: {
                                UserId: result.Data.SystemUserId,
                                Username: result.Data.UserName,
                                Password: result.Data.Password,
                                LegalEntityId: result.Data.LegalEntity.LegalEntityId,
                                Firstname: result.Data.LegalEntity.FullName,
                                Middlename: result.Data.LegalEntity.FirstName,
                                Lastname: result.Data.LegalEntity.LastName,
                                FullName: result.Data.LegalEntity.MiddleName,
                                IsWebAdminGuestUser: result.Data.IsWebAdminGuestUser,
                                IsEnforcementUnit: result.Data.IsEnforcementUnit,
                                SystemUserConfig: result.Data.SystemUserConfig
                            },
                            ApplicationToken: {
                                AccessToken: result.Data.Token.access_token,
                                RefreshToken: result.Data.Token.refresh_token
                            },
                            UserViewAccess: [],
                            Privileges: result.Data.SystemWebAdminPrivileges,
                        };
                        if (appState.User.IsEnforcementUnit) {
                            appState.User.EnforcementStationId = result.Data.EnforcementUnit.EnforcementStation.EnforcementStationId;
                        }
                        //if (result.Data.ProfilePicture.IsFromStorage) {
                        //    appState.User.ProfilePictureSource = app.appSettings.POSWebAPIURI + "File/getFile?FileId=" + result.Data.ProfilePicture.FileId;
                        //} else {
                        //    appState.User.ProfilePictureSource = 'data:' + result.Data.ProfilePicture.MimeType + ';base64,' + result.Data.ProfilePicture.FileContent;
                        //}
                        if (result.Data?.ProfilePicture !== undefined && result.Data?.ProfilePicture !== null && result.Data?.ProfilePicture?.FileId !== undefined) {
                            appState.User.ProfilePictureSource = app.appSettings.POSWebAPIURI + "File/getFile?FileId=" + result.Data.ProfilePicture.FileId;
                        }
                        for (var i in result.Data.SystemWebAdminMenus) {
                            appState.UserViewAccess.push({
                                MenuId: result.Data.SystemWebAdminMenus[i].SystemWebAdminMenuId,
                                PageName: result.Data.SystemWebAdminMenus[i].SystemWebAdminMenuName,
                                ModuleName: result.Data.SystemWebAdminMenus[i].SystemWebAdminModule.SystemWebAdminModuleName,
                            });

                        }

                        api.setApplicationState(appState).done(function (result) {
                            if (result.Success) {
                                window.location.replace("/");
                                circleProgress.close();
                            } else {
                                Swal.fire('Error!', result.Message, 'error');
                                circleProgress.close();
                            }
                        });
                    } else {
                        $(".content").find("input,button,a").prop("disabled", false).removeClass("disabled");
                        circleProgress.close();
                        Swal.fire('Error!', result.Message, 'error');
                    }
                }).error(function (result) {
                    $(".content").find("input,button,a").prop("disabled", false).removeClass("disabled");
                    if (result.responseJSON !== undefined) {
                        Swal.fire('Error!', result.responseJSON.Message, 'error');
                    } else if (result.statusText && result.statusText.includes("Invalid URL")) {
                        Swal.fire('Error!', 'Unable to connect to server!', 'error');
                    } else {
                        Swal.fire('Error!', 'Unable to connect to server!', 'error');
                    }
                    circleProgress.close();
                });
        }
    }

    return  {
        init: init
    };
}
var login = new loginController;