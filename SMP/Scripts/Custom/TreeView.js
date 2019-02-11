/*
 * Срабатывает при клике на экспандер дерева.
 */
function OnExpanderClick() {
    $('.expander').click(function () {
        var $expander = $(event.target);
        var $node = $expander.closest('.tree-node');
        ShowHide($node, true);
        if ($node.hasClass('expanded')) {
            $expander.removeClass('glyphicon-chevron-down');
            $expander.addClass('glyphicon-chevron-up');
        } else {
            $expander.addClass('glyphicon-chevron-down');
            $expander.removeClass('glyphicon-chevron-up');
        }
        
    });
}

/*
 * Сворачивает или разворачивает дерево в узле.
 */
function ShowHide($node, root) {
    var $children = $(".tree-node[pId = '" + $node.attr('id') + "']");

    // Если находимся в root, то делаем его закрытым и прячем все дочерние узлы. 
    if (root) {
    if ($node.hasClass('expanded')) {
        $node.removeClass('expanded');
            $children.addClass('hidden');
        } else {
        $node.addClass('expanded');
            $children.removeClass('hidden');
        }
    } else {

    // Иначе распространяем видимость/невидимость на дочерние узлы.
    if ($node.hasClass('hidden') || !$node.hasClass('expanded')) {
            $children.addClass('hidden');
        } else {
            $children.removeClass('hidden');
        }
    }
    
    $children.each(function(ind, $el) {
        ShowHide($($el), false);
    });

    
}

function OnDocLoad() {
    $(document).ready(OnExpanderClick);
}

OnDocLoad();
