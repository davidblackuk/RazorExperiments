window.explorerSplitters = (function () {
    'use strict';

    function initSplitter(splitter, orientation, shell, sidebar, top, main) {
        if (!splitter) {
            return;
        }

        var dragging = false;

        function onMouseDown(e) {
            dragging = true;
            splitter.classList.add('explorer-splitter-dragging');
            document.body.style.userSelect = 'none';
            e.preventDefault();
        }

        function onMouseMove(e) {
            if (!dragging) {
                return;
            }

            if (orientation === 'horizontal') {
                var shellRect = shell.getBoundingClientRect();
                var newWidth = e.clientX - shellRect.left;
                newWidth = Math.max(160, Math.min(newWidth, shellRect.width - 240));
                sidebar.style.flexBasis = newWidth + 'px';
            } else {
                var mainRect = main.getBoundingClientRect();
                var newHeight = e.clientY - mainRect.top;
                newHeight = Math.max(80, Math.min(newHeight, mainRect.height - 80));
                top.style.flexBasis = newHeight + 'px';
            }
        }

        function onMouseUp() {
            if (!dragging) {
                return;
            }
            dragging = false;
            splitter.classList.remove('explorer-splitter-dragging');
            document.body.style.userSelect = '';
        }

        splitter.addEventListener('mousedown', onMouseDown);
        document.addEventListener('mousemove', onMouseMove);
        document.addEventListener('mouseup', onMouseUp);
    }

    // orientation 'horizontal' resizes the sidebar's width (drag left/right);
    // 'vertical' resizes the top pane's height (drag up/down).
    function init(shellSelector) {
        var shell = document.querySelector(shellSelector);
        if (!shell || shell.dataset.splittersBound === 'true') {
            return;
        }
        shell.dataset.splittersBound = 'true';

        var sidebar = shell.querySelector('.explorer-sidebar');
        var main = shell.querySelector('.explorer-main');
        var top = shell.querySelector('.explorer-top');
        var splitterV = shell.querySelector('.explorer-splitter-v');
        var splitterH = shell.querySelector('.explorer-splitter-h');

        initSplitter(splitterV, 'horizontal', shell, sidebar, top, main);
        initSplitter(splitterH, 'vertical', shell, sidebar, top, main);
    }

    return { init: init };
})();
