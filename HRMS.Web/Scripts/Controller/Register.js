var registerController = function(){

    var apiService = function (apiURI){
        var getUserByCredentials = function (Username, Password)
        {
            return $.ajax({
                url: apiURI + "SystemUser/GetByCredentials?Username=" + Username + "&Password=" + Password,
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
        var sendVerification = function (verification) {
            return $.ajax({
                url: apiURI + "SystemUserVerification",
                data: JSON.stringify(verification),
                type: "POST",
                contentType: 'application/json;charset=utf-8',
                dataType: "json",
            });
        }
        var getBySender = function (sender, code) {
            return $.ajax({
                url: apiURI + "SystemUserVerification/GetBySender?sender=" + sender + "&code=" + code,
                type: "GET",
                contentType: 'application/json;charset=utf-8',
                dataType: "json"
            });
        }

        
        var saveAccount = function (account) {
            return $.ajax({
                url: app.appSettings.POSWebAPIURI + "/SystemUser/CreateWebAdminAccount",
                type: "POST",
                dataType: "json",
                contentType: 'application/json;charset=utf-8',
                data: JSON.stringify(account)
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
            setApplicationState: setApplicationState, 
            sendVerification: sendVerification, 
            getBySender: getBySender, 
            saveAccount: saveAccount,
            getUserByCredentials: getUserByCredentials,
            getLookup: getLookup,
        };
    }

    var api, form, formVerificationMobileNumber, formVerificationEmail, formConfirmVerification, formStepCredentials;
    var appSettings = {
        model : {}
    }
    var init = function () {
        api = new apiService(app.appSettings.POSWebAPIURI);
        initLookup();
        setTimeout(function () {
            appSettings.verificationModel = {
                IsMobile: true,
                IsEmail: false,
                AccountVerification: "EmailVerification"
            };
            initStepVerification();
        }, 0);

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
            console.log(appSettings);
        });
    }

    //Form validation

    var initValidationVerification = function () {
        formVerificationMobileNumber = $("#form-verification-number");
        formVerificationMobileNumber.validate({
            ignore: [],
            rules: {
                MobileNumberVerification: {
                    required: true,
                    digits: true
                }
            },
            messages: {
                MobileNumberVerification: {
                    required: "Please enter your mobile number",
                    digits: "Please enter valid mobile number"
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

        formVerificationEmail = $("#form-verification-email");
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

    var initValidationConfirmVerification = function () {
        formConfirmVerification = $("#form-confirmVerification");
        formConfirmVerification.validate({
            ignore: [],
            rules: {
                VerificationSender: {
                    required: true
                },
                VerificationCode: {
                    required: true
                }
            },
            messages: {
                VerificationCode: {
                    required: "Please enter verification code"
                },
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


    var initValidationCredentials = function () {
        formStepCredentials = $("#form-credentials");
        formStepCredentials.validate({
            ignore: [],
            rules: {
                Password: {
                    required: true,
                    minlength: 7,
                    pwcheck: true
                },
                ConfirmPassword: {
                    required: true,
                    equalTo: function () {
                        return "#Password";
                    }
                }
            },
            messages: {
                Password: {
                    required: "Please enter Password",
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

    var initValidationBasicinfo = function () {
        form = $("#form-basicinfo");
        form.validate({
            ignore: [],
            rules: {
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
                }
            },
            messages: {
                FirstName: "Please enter a Firstname",
                MiddleName: "Please enter Middlename",
                LastName: "Please enter Lastname",
                BirthDate: "Please select Birth Date",
                GenderId: "Please select Gender"
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

    //end Form validation

    //Step

    var initStepVerification = function () {

        var verificationTemplate = $.templates('#verification-template');
        verificationTemplate.link("#accountcard", appSettings.verificationModel);
        $("#form-verification-email").addClass("hidden");
        $("#form-verification-number").removeClass("hidden");
        initValidationVerification();

        appSettings.credentialsModel = {};
        appSettings.confirmVerificationModel = {};

        $("input")[0].focus();
        $("#btn-verify").on("click", Verify);
        appSettings.verificationModel.IsMobile = true;
        appSettings.verificationModel.IsEmail = false;
        $("#btnRadioEmailVerification, #btnRadioMobileNumberVerification").on("change", function () {
            if ($(this).is(':checked') && $(this).val() === "MobileNumber") {
                appSettings.verificationModel.IsMobile = true;
                appSettings.verificationModel.IsEmail = false;
                $("#form-verification-email").addClass("hidden");
                $("#form-verification-number").removeClass("hidden");
            } else if ($(this).is(':checked') && $(this).val() === "Email") {
                appSettings.verificationModel.IsMobile = false;
                appSettings.verificationModel.IsEmail = true;
                $("#form-verification-number").addClass("hidden");
                $("#form-verification-email").removeClass("hidden");
            }
            initValidationVerification();
        });
    }

    var initStepConfirmVerification = function () {

        var confirmVerificationTemplate = $.templates('#confirmVerification-template');
        confirmVerificationTemplate.link("#accountcard", appSettings.confirmVerificationModel);

        initValidationConfirmVerification();

        $("#btn-step-confirmverification").on("click", ConfirmVerification);
        $("#btn-step-confirmverification-back").on("click", initStepVerification);

    }

    var initStepCredentials = function () {
        appSettings.credentialsModel = {};
        appSettings.credentialsModel = appSettings.verificationModel;
        appSettings.credentialsModel.EmailAddress = appSettings.verificationModel.IsEmail ? appSettings.verificationModel.EmailVerification : "";
        appSettings.credentialsModel.MobileNumber = appSettings.verificationModel.IsMobile ? appSettings.verificationModel.MobileNumberVerification : "";
        appSettings.credentialsModel = $.extend(appSettings.credentialsModel, appSettings.confirmVerificationModel);
        var credentialsTemplate = $.templates('#credentials-template');
        credentialsTemplate.link("#accountcard", appSettings.credentialsModel);
        initValidationCredentials();


        $("#btn-step-credentials").on("click", CreateCredentials);
        $("#btn-step-credentials-back").on("click", initStepVerification);

        setTimeout(function () {
            $("#Password").val("");
        }, 1000);
    }

    var initStepBasicInfo = function () {
        if (appSettings.model === undefined || appSettings.model === null) {
            appSettings.model = appSettings.credentialsModel;
        } else {
            appSettings.model.Password = appSettings.credentialsModel.Password;
            appSettings.model.EmailAddress = appSettings.credentialsModel.EmailAddress;
            appSettings.model.MobileNumber = appSettings.credentialsModel.MobileNumber;
            appSettings.model.VerificationCode = appSettings.confirmVerificationModel.VerificationCode;
            appSettings.model.VerificationSender = appSettings.confirmVerificationModel.VerificationSender;
        }
        appSettings.model.lookup = {
            EntityGender: appSettings.lookup.EntityGender
        };
        var basicinfoTemplate = $.templates('#basicinfo-template');
        basicinfoTemplate.link("#accountcard", appSettings.model);
        initValidationBasicinfo();

        $(".select-tags").select2({
            tags: false,
            theme: "bootstrap",
        });

        $('#BirthDate').datetimepicker({
            format: 'MM/DD/YYYY'
        });

        $('#BirthDate').parent().addClass('pmd-textfield-floating-label-completed');
        $(".select-simple").select2({
            theme: "bootstrap",
            minimumResultsForSearch: Infinity,
        });

        $("#btn-step-basicinfo").on("click", SaveAccount);
        $("#btn-step-basicinfo-back").on("click", initStepCredentials);

    }

    //end Step


    var Verify = function () {
        appSettings.confirmVerificationModel = {};
        if (appSettings.verificationModel.IsMobile) {
            if (!formVerificationMobileNumber.valid())
                return;
            appSettings.confirmVerificationModel.VerificationSender = appSettings.verificationModel.MobileNumberVerification;
            appSettings.confirmVerificationModel.VerificationTypeId = 1;
        }
        if (appSettings.verificationModel.IsEmail) {
            if (!formVerificationEmail.valid())
                return;
            appSettings.confirmVerificationModel.VerificationSender = appSettings.verificationModel.EmailVerification;
            appSettings.confirmVerificationModel.VerificationTypeId = 2;
        }
        circleProgress.show(false);
        api.sendVerification(appSettings.confirmVerificationModel).done(function (result) {
            if (result.IsSuccess) {
                circleProgress.close();
                initStepConfirmVerification();
            }
        }).error(function (result) {
            Swal.fire('Error!', result.responseJSON.Message, 'error');
            circleProgress.close();
        });;
    }

    var ConfirmVerification = function () {
        if (!formConfirmVerification.valid())
            return;
        circleProgress.show(false);
        api.getBySender(appSettings.confirmVerificationModel.VerificationSender, appSettings.confirmVerificationModel.VerificationCode).done(function (result) {
            if (result.IsSuccess) {
                if (result.Data.VerificationCode === appSettings.confirmVerificationModel.VerificationCode) {
                    initStepCredentials();
                } else {
                    Swal.fire('Error!', "Incorrect verification code! Please try again", 'error');
                }
            }
            circleProgress.close();
        }).error(function (result) {
            Swal.fire('Error!', "Incorrect verification code! Please try again", 'error');
            circleProgress.close();
        });;
    }

    var CreateCredentials = function () {
        if (!formStepCredentials.valid())
            return;
        initStepBasicInfo();
    }

    var SaveAccount = function () {
        if (!form.valid())
            return;
        circleProgress.show(false);
        api.saveAccount(appSettings.model)
            .done(function (result) {
                if (result.IsSuccess) {
                    $(".content").find("input,button,a").prop("disabled", false).removeClass("disabled");
                    api.getUserByCredentials(appSettings.model.VerificationSender, appSettings.model.Password)
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
                                        ProfilePictureSource: app.appSettings.POSWebAPIURI + "File/getFile?FileId=" + result.Data.ProfilePicture.FileId,
                                        IsWebAdminGuestUser: result.Data.IsWebAdminGuestUser
                                    },
                                    ApplicationToken: {
                                        AccessToken: result.Data.Token.access_token,
                                        RefreshToken: result.Data.Token.refresh_token
                                    },
                                    UserViewAccess: []
                                };
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
                                    } else {
                                        circleProgress.close();
                                        Swal.fire('Error!', result.Message, 'error');
                                    }
                                });
                            } else {
                                $(".content").find("input,button,a").prop("disabled", false).removeClass("disabled");
                                circleProgress.close();
                                Swal.fire('Error!', result.Message, 'error');
                            }
                        }).error(function (result) {
                            $(".content").find("input,button,a").prop("disabled", false).removeClass("disabled");
                            Swal.fire('Error!', result.responseJSON.Message, 'error');
                            circleProgress.close();
                        });
                } else {
                    $(".content").find("input,button,a").prop("disabled", false).removeClass("disabled");
                    circleProgress.close();
                    Swal.fire('Error!', result.Message, 'error');
                }
            }).error(function (result) {
                var errormessage = "";
                var errorTitle = "";
                if (result.responseJSON.DeveloperMessage !== null && result.responseJSON.DeveloperMessage.includes("Cannot insert duplicate")) {
                    erroTitle = "Not Allowed!";
                    errormessage = "Already exist!";
                } else {
                    erroTitle = "Error!";
                    errormessage = result.responseJSON.Message;}
                $(".content").find("input,button,a").prop("disabled", false).removeClass("disabled");
                Swal.fire('Error!', errormessage, 'error');
                circleProgress.close();
            });
    }

    return  {
        init: init
    };
}
var register = new registerController;