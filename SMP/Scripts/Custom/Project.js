﻿/*
 * Срабатывает при клике на кнопку вкладки.
 */
function OnTabClick() {
    var projectInfoBtnId = 'project-info-btn';
    var teamBtn = 'team-btn';
    var worksBtn = 'works-btn';
    var reportBtn = 'report-btn';

    // Привязываем кнопки к контентам вкладок. 
    $('#' + projectInfoBtnId).click(function() {
        ActivateTab(projectInfoBtnId, 'project-info-content');
    });
    $('#' + teamBtn).click(function() {
        ActivateTab(teamBtn, 'team-content');
    });
    $('#' + worksBtn).click(function() {
        ActivateTab(worksBtn, 'works-content');
    });
    $('#' + reportBtn).click(function() {
        ActivateTab(reportBtn, 'report-content');
    });
}

/*
 * Активирует вкладку и ее контент. 
 */
function ActivateTab(tabBtnId, tabContentId) {
    var activeClass = 'active';
    $('.nav-link.active, .tab-content.active').removeClass(activeClass);
    $('#' + tabBtnId + ',#' + tabContentId).addClass(activeClass);
    return false;
}

function OnDocLoad() {
    $(document).ready(OnTabClick);
}

OnDocLoad();