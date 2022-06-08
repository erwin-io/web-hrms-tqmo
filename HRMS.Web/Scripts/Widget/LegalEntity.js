    
var legalEntityController = function() {



    var form;
    var appSettings = {
        model:null,
        status:{ IsNew:false},
    };
    var init = function (obj) {
        appSettings = $.extend(appSettings, obj);
        form = $('#form-legalentity');
        iniValidation(appSettings.forms.Rules, appSettings.forms.Messages);
        initEvent();
    };

    var iniValidation = function(rules,messages) {
        form.validate({
            ignore:[],
            rules: rules,
            messages: messages,
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
        $('#BirthDate').datetimepicker({
            format: 'MM/DD/YYYY'
        });

        $('#BirthDate').parent().addClass('pmd-textfield-floating-label-completed');
        $(".select-simple").select2({
            theme: "bootstrap",
            minimumResultsForSearch: Infinity,
        });
    };

    var valid = ()=>{
        return form.valid();
    }

    //Function for clearing the textboxes
    return  {
        appSettings: appSettings,
        valid: valid,
        init: init,
    };
}
var legalEntity = new legalEntityController;
