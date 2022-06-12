var resetPasswordController = function(){

    var apiService = function (apiURI) {
        var getUserById = function (Id) {
            return $.ajax({
                url: apiURI + "SystemUser/" + Id + "/detail",
                type: "GET",
                contentType: 'application/json;charset=utf-8',
                dataType: "json",
                headers: {
                    Authorization: 'Bearer ' + apiToken
                }
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
        var setAppRefreshToken = function (ApplicationToken) {
            return $.ajax({
                url: "/Account/SetApplicationToken",
                type: "POST",
                data: JSON.stringify({ ApplicationToken: ApplicationToken }),
                contentType: 'application/json;charset=utf-8',
                dataType: "json",
            });
        }
        var sendEmailChangePasswordRequest = function (verification) {
            return $.ajax({
                url: apiURI + "SystemUserVerification/sendEmailChangePasswordRequest",
                data: JSON.stringify(verification),
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
                dataType: "json"
            });
        }

        return {
            getUserById: getUserById,
            setApplicationState: setApplicationState, 
            setAppRefreshToken: setAppRefreshToken,
            sendEmailChangePasswordRequest: sendEmailChangePasswordRequest,
            getLookup: getLookup,
        };
    }

    var api, formVerificationEmail, formChangePassword;
    var appSettings = {
        model : {}
    }
    var init = function () {
        api = new apiService(app.appSettings.HRMSAPIURI);
        initLookup();

        
        var email = getUrlParameter('email');
        var id = getUrlParameter('id');
        var code = getUrlParameter('code');
        if (email !== "" && id !== "" && code !== "") {
            setTimeout(function () {
                appSettings.verificationModel = {};
                initChangePassword(id);
            }, 0);

        } else {
            setTimeout(function () {
                appSettings.verificationModel = {};
                initVerification();
            }, 0);

        }
        $.validator.addMethod("pwcheck", function (value) {
            return /^[A-Za-z0-9\d=!\-@._*]*$/.test(value) // consists of only these
                && /[a-z]/.test(value) // has a lowercase letter
                && /[A-Z]/.test(value) // has a uppercase letter
                && /\d/.test(value) // has a digit
        });
        $.validator.addMethod("emailCheck", function (value) {
            return /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/.test(value) // consists of only these
        });
    }

    var initLookup = function () {
        api.getLookup("SystemWebAdminRole,EntityGender").done(function (data) {
            appSettings.lookup = $.extend(appSettings.lookup, data.Data);
        });
    }

    var getUrlParameter = function getUrlParameter(sParam) {
        var sPageURL = window.location.search.substring(1),
            sURLVariables = sPageURL.split('&'),
            sParameterName,
            i;

        for (i = 0; i < sURLVariables.length; i++) {
            sParameterName = sURLVariables[i].split('=');

            if (sParameterName[0] === sParam) {
                return typeof sParameterName[1] === undefined ? true : decodeURIComponent(sParameterName[1]);
            }
        }
        return "";
    };

    //Form validation

    var initValidationVerification = function () {
        formVerificationEmail = $("#form-verification");
        formVerificationEmail.validate({
            ignore: [],
            rules: {
                EmailVerification: {
                    required: true,
                    emailCheck: true
                }
            },
            messages: {
                EmailVerification: {
                    required: "Please enter your email",
                    emailCheck: "Please enter valid email"
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

    var initValidationChangePassword = function () {
        formChangePassword = $("#form-changePassword");
        formChangePassword.validate({
            ignore: [],
            rules: {
                NewPassword: {
                    required: true,
                    minlength: 7,
                    pwcheck: true
                },
                ConfirmPassword: {
                    required: true,
                    equalTo: function () {
                        return "#NewPassword";
                    }
                }
            },
            messages: {
                Password: {
                    required: "Please enter New Password",
                    minlength: $.validator.format("Please enter at least {0} characters."),
                    pwcheck: "This field must consists of the following : uppercase, lowercase, digit and special characters",
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

    };

    //init page

    var initVerification = function () {
        appSettings.verificationModel.VerificationTypeId = 2;
        var verificationTemplate = $.templates('#verification-template');
        verificationTemplate.link("#accountcard", appSettings.verificationModel);
        initValidationVerification();


        $("#btn-submitemail").on("click", SubmitVerificationEmail);
    }

    var initSuccessVerification = function () {
        var verificationTemplate = $.templates('#successVerification-template');
        verificationTemplate.link("#accountcard", appSettings.verificationModel);
        //initValidationVerification();

        $("#btn-resendVerification").on("click", ResendVerification);
    }


    var initChangePassword = function (id) {
        appSettings.resetPasswordModel = { SystemUserId: id };
        var changePasswordTemplate = $.templates('#changePassword-template');
        changePasswordTemplate.link("#accountcard", appSettings.resetPasswordModel);
        initValidationChangePassword();

        $("#btn-changePassword").on("click", ResetPassword);
    }
    //function
    var SubmitVerificationEmail = function () {
        if (!formVerificationEmail.valid())
            return;
        circleProgress.show(false);
        api.sendEmailChangePasswordRequest(appSettings.verificationModel).done(function (result) {
            if (result.IsSuccess) {
                initSuccessVerification();
            }
            circleProgress.close();
        }).error(function (result) {
            Swal.fire('Error!', result.responseJSON.Message, 'error');
            circleProgress.close();
        });;
    }

    var ResendVerification = function () {
        Swal.fire({
            title: 'Resend Verification?',
            text: "Do you want to continue!",
            type: 'question',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes',
            allowOutsideClick: false
        }).then((result) => {
            if (result.value) {
                initVerification();
            }
        });
    }

    var ResetPassword = function () {
        if (!formChangePassword.valid()) {
            return;
        }

        Swal.fire({
            title: 'Reset Password?',
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
                    url: app.appSettings.HRMSAPIURI + "/SystemUser/ResetPassword",
                    type: "PUT",
                    dataType: "json",
                    contentType: 'application/json;charset=utf-8',
                    data: JSON.stringify(appSettings.resetPasswordModel),
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
                        $(".content").find("input,button,a").prop("disabled", false).removeClass("disabled");
                        Swal.fire(erroTitle, errormessage, 'error');
                        circleProgress.close();
                    }
                });
            }
        });
    }


    return  {
        init: init
    };
}
var resetPassword = new resetPasswordController;