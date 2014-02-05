function isValidDate(d) {
    if (Object.prototype.toString.call(d) !== "[object Date]")
        return false;
    return !isNaN(d.getTime());
}

jQuery.validator.addMethod('requiredif',
    function (value, element, parameters) {
        var id = '#' + parameters['dependentproperty'];

        // get the target value (as a string, 
        // as that's what actual value will be)
        var targetvalue = parameters['targetvalue'];        
        targetvalue =
          (targetvalue == null ? '' : targetvalue).toString();
        // get the actual value of the target control
        // note - this probably needs to cater for more 
        // control types, e.g. radios
        var control = $(id);
        var controltype = control.attr('type');

        var actualvalue =
            controltype === 'checkbox' ? control.is(':checked').toString() : control.val().toString();
                            
        // if the condition is true, reuse the existing 
        // required field validator functionality                
        if (targetvalue === actualvalue)
            return $.validator.methods.required.call(
              this, value, element, parameters);

        return true;
    }
);

jQuery.validator.unobtrusive.adapters.add(
    'requiredif',
    ['dependentproperty', 'targetvalue'],
    function (options) {
        options.rules['requiredif'] = {
            dependentproperty: options.params['dependentproperty'],
            targetvalue: options.params['targetvalue']
        };
        options.messages['requiredif'] = options.message;
    });


    jQuery.validator.addMethod("comparevalues", function (value, element, param) {
        if (this.optional(element)) {
            return true;
        }
        var othervalue = document.getElementById(param.otherproperty).value;
        if (isDate(value, 'dd/MM/y') == true) {
            value = new Date(getDateFromFormat(value, 'dd/MM/y'));
            othervalue = new Date(getDateFromFormat(othervalue, 'dd/MM/y'));
        }
        else {
            if (isValidDate(new Date(value)) == true) {
                value = new Date(value);
                othervalue = new Date(othervalue);
            }
        }

        switch (param.comparer) {
            case ">=":
                if (value >= othervalue) {
                    return true;
                }
                break;
            case "<=":
                if (value <= othervalue) {
                    return true;
                }
                break;
            case ">":
                if (value > othervalue) {
                    return true;
                }
                break;
            case "<":
                if (value < othervalue) {
                    return true;
                }
                break;
            case "=":
                if (value = othervalue) {
                    return true;
                }
                break;
        }
        return false;
    });

jQuery.validator.unobtrusive.adapters.add
("comparevalues", ["otherproperty", "comparer"], function (options) {

    options.rules["comparevalues"] = {
        otherproperty: options.params.otherproperty,
        comparer: options.params.comparer
    };
    options.messages["comparevalues"] = options.message;
});
