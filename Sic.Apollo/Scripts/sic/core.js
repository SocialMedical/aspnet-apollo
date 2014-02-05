SIC_DATE_FORMAT = 'dd/mm/yy';
SIC_DATE_DISPLAY_FORMAT = 'dd/M/yy';
SIC_CURRENCY_SYMBOL = '$';
SIC_CURRENCY_PRECISION = 2;

function sicGet(url, queryData, callback) {
    $.get(url, queryData,
        function (data) {
            if (data.IsRedirect && data.Url) {
                location.href = data.Url;
            }
            else
                callback(data);
        });
}

function sicPost(url, queryData, callback) {
    $.post(url, queryData,
        function (data) {
            if (data.IsRedirect && data.Url) {
                location.href = data.Url;
            }
            else
                callback(data);
        });
}

function sicConvertToInt(value) {
    try {
        if (sicIsNumeric(value))
            return parseInt(value);
        else
            return Math.round(({ value: value }).toUnformatted());
    } catch (ex) { }
    return 0;
}

function sicConvertToFloat(value) {
    try {
        if (sicIsNumeric(value))
            return parseFloat(value);
        else
            return sicNumberFormat({ value: value }).toUnformatted();
    } catch (ex) { }
    return 0;
}

function sicIsNumeric(value) {
    return $.isNumeric(value);
}

function sicMathRound(value, decimals) {
    var roundeValue = sicConvertToFloat(value);
    return roundeValue.toFixed(decimals);
}