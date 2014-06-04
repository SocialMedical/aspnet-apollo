function toggleMoreSearchCriteria() {
    $(".linkToggleMoreSearchCriteria").toggle('fast');
    $(".moreSearchCriteria").toggle('fast');
}

function toggleInsurances() {
    $(".linkToggleMoreSearchCriteria").toggle('fast');
    $(".moreSearchCriteria").toggle('fast');
}

$(function(){
    $(".linkToggleMoreSearchCriteria a").attr('href', 'javascript:toggleMoreSearchCriteria();');    
});