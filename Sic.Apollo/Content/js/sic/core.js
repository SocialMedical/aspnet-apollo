SIC_DATE_FORMAT = 'dd/mm/yy';
SIC_DATE_DISPLAY_FORMAT = 'dd/M/yy';
SIC_CURRENCY_SYMBOL = '$';
SIC_CURRENCY_PRECISION = 2;

function sicGet(url, queryData, callback, dataType) {
    $.ajax({
        url: url,
        data: queryData,
        type: 'GET',
        contentType: dataType,
        success: function (data) {
            if (data.IsRedirect && data.Url) {
                location.href = data.Url;
            }
            else
                callback(data);
        }
    });
}

function sicPost(url, queryData, callback, dataType) {    
    $.ajax({
        url: url,
        data: queryData,
        type: 'POST',
        //data: { viewDesign: jsonViewDesign },
        contentType: dataType,
        success: function (data) {
            if (data.IsRedirect && data.Url) {
                location.href = data.Url;
            }
            else {
                if (callback)
                    callback(data);
                else {
                    if (data.Message)                        
                        sicNotifyAsPopup(data.Messages);
                }
            }
        }
    });
}

function sicJSONDataGet(url, queryData, callback) {
    sicGet(url, JSON.stringify(queryData), callback, "application/json");
}

function sicJSONDataPost(url, queryData, callback) {    
    sicPost(url, JSON.stringify(queryData), callback, "application/json");
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


$(function () {
    var accent_map = {
        'á': 'a',
        'à': 'a',
        'â': 'a',
        'å': 'a',
        'ä': 'a',
        'a': 'a',
        'ã': 'a',
        'ç': 'c',
        'é': 'e',
        'è': 'e',
        'ê': 'e',
        'ë': 'e',
        'í': 'i',
        'ì': 'i',
        'î': 'i',
        'ï': 'i',
        'ñ': 'n',
        'ó': 'o',
        'ò': 'o',
        'ô': 'o',
        'ö': 'o',
        'õ': 'o',
        'ú': 'u',
        'ù': 'u',
        'û': 'u',
        'ü': 'u',
    };


    String.prototype.sicReplaceAccentsChars = function () {
        var ret = '', s = this.toString();
        if (!s) { return ''; }
        for (var i = 0; i < s.length; i++) {
            ret += accent_map[s.charAt(i)] || s.charAt(i);
        }
        return ret;
    };

    String.prototype.sicContainWords = function (words) {
        return this.toString().toLowerCase().sicReplaceAccentsChars().indexOf(words.toLowerCase().sicReplaceAccentsChars()) != -1;
        
    };
});