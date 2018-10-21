/*
 * Срабатывает при клике на экспандер дерева.
 */
function OnExpanderClick() {
    $('.expander').click(function() {
        ShowHide($(event.target).closest('.treeNode'));
    });
}

/*
 * Сворачивает или разворачивает дерево в узле.
 */
function ShowHide($node, root = true) {
    var $node = $($node).closest('.treeNode');
    var $children = $(".treeNode[pId = '" + $node.attr('id') + "']");
    if ($node.hasClass('expanded')) {
        $node.removeClass('expanded');
        $children.addClass('hidden');
    } else {
        $node.addClass('expanded');
        $children.removeClass('hidden');
    }

    $children.each(function(ind, $el) {
        ShowHide($el);
    });

    if ($node.hasClass('hidden')) {
        $children.addClass('hidden');
    }
}

function OnDocLoad() {
    $(document).ready(OnExpanderClick);
}

OnDocLoad();
