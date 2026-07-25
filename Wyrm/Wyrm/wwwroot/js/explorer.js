(function () {
    'use strict';

    document.addEventListener('DOMContentLoaded', function () {
        var shell = document.getElementById('explorerShell');
        if (!shell) {
            return;
        }

        var gridUrl = shell.dataset.gridUrl;
        var detailUrl = shell.dataset.detailUrl;
        var gridBody = document.getElementById('explorerGridBody');
        var detailBody = document.getElementById('explorerDetailBody');

        initTree();
        initSplitter(document.getElementById('explorerSplitterV'), 'horizontal');
        initSplitter(document.getElementById('explorerSplitterH'), 'vertical');

        function initTree() {
            document.querySelectorAll('.explorer-tree-repo-node').forEach(function (node) {
                node.addEventListener('click', function () {
                    var target = document.getElementById(node.getAttribute('data-toggle-target'));
                    if (!target) {
                        return;
                    }
                    target.classList.toggle('explorer-tree-collapsed');
                    node.classList.toggle('explorer-tree-expanded');
                });
            });

            document.querySelectorAll('.explorer-tree-objecttype-node').forEach(function (node) {
                node.addEventListener('click', function () {
                    document.querySelectorAll('.explorer-tree-objecttype-node').forEach(function (n) {
                        n.classList.remove('active');
                    });
                    node.classList.add('active');

                    loadGrid(node.getAttribute('data-object-type-id'));
                    detailBody.innerHTML = '<p class="text-muted mb-0">Select an instance from the grid to view its details.</p>';
                });
            });
        }

        function loadGrid(objectTypeId) {
            gridBody.innerHTML = '<p class="text-muted mb-0">Loading&hellip;</p>';
            $.get(gridUrl, { objectTypeId: objectTypeId })
                .done(function (html) {
                    gridBody.innerHTML = html;
                    bindGridRows();
                })
                .fail(function () {
                    gridBody.innerHTML = '<p class="text-danger mb-0">Failed to load instances.</p>';
                });
        }

        function bindGridRows() {
            document.querySelectorAll('.explorer-instance-row').forEach(function (row) {
                row.addEventListener('click', function () {
                    document.querySelectorAll('.explorer-instance-row').forEach(function (r) {
                        r.classList.remove('table-active');
                    });
                    row.classList.add('table-active');
                    loadDetail(row.getAttribute('data-instance-id'));
                });
            });
        }

        function loadDetail(instanceId) {
            detailBody.innerHTML = '<p class="text-muted mb-0">Loading&hellip;</p>';
            $.get(detailUrl, { instanceId: instanceId })
                .done(function (html) {
                    detailBody.innerHTML = html;
                })
                .fail(function () {
                    detailBody.innerHTML = '<p class="text-danger mb-0">Failed to load instance details.</p>';
                });
        }

        // orientation 'horizontal' resizes the sidebar's width (drag left/right);
        // 'vertical' resizes the top pane's height (drag up/down).
        function initSplitter(splitter, orientation) {
            if (!splitter) {
                return;
            }

            var sidebar = document.getElementById('explorerSidebar');
            var top = document.getElementById('explorerTop');
            var main = document.getElementById('explorerMain');
            var dragging = false;

            splitter.addEventListener('mousedown', function (e) {
                dragging = true;
                splitter.classList.add('explorer-splitter-dragging');
                document.body.style.userSelect = 'none';
                e.preventDefault();
            });

            document.addEventListener('mousemove', function (e) {
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
            });

            document.addEventListener('mouseup', function () {
                if (!dragging) {
                    return;
                }
                dragging = false;
                splitter.classList.remove('explorer-splitter-dragging');
                document.body.style.userSelect = '';
            });
        }
    });
})();
