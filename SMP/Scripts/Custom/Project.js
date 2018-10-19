/*
 * Срабатывает при клике на кнопку вкладки.
 */
function OnTabClick() {
    var projectInfoBtnId = 'project-info-btn';
    var teamBtn = 'team-btn';
    var worksBtn = 'works-btn';

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
}

/*
 * Активирует вкладку и ее контент. 
 */
function ActivateTab(tabBtnId, tabContentId) {
    var activeClass = 'active';
    $('.tab-button, .tab-content').removeClass(activeClass);
    $('#' + tabBtnId + ',#' + tabContentId).addClass(activeClass);
}

function OnDocLoad() {
    $(document).ready(OnTabClick);
}

OnDocLoad();