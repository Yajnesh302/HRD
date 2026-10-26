<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SPSGRIGHT.aspx.cs" Inherits="HRD.SPSGRIGHT" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Human Resource Division - Bulletins &amp; Notices</title>
    <style type="text/css">
        /* Reset & Base System Typography (100% Intranet compatible - No external CDNs) */
        *, *::before, *::after {
            box-sizing: border-box;
            margin: 0;
            padding: 0;
        }

        html, body {
            height: 100%;
            width: 100%;
            background-color: #f1f5f9;
            color: #1e293b;
            font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, Verdana, sans-serif;
            font-size: 13px;
            line-height: 1.5;
            overflow: hidden;
        }

        /* Error Banner */
        .status-banner {
            background: #fef2f2;
            border-bottom: 1px solid #fecaca;
            color: #991b1b;
            padding: 8px 16px;
            font-size: 12px;
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .status-badge {
            background-color: #dc2626;
            color: #ffffff;
            font-weight: 700;
            font-size: 10px;
            padding: 2px 8px;
            border-radius: 10px;
            text-transform: uppercase;
        }

        /* Full Height Page Container */
        .portal-container {
            display: flex;
            flex-direction: column;
            height: 100vh;
            max-height: 100vh;
            padding: 10px;
            overflow: hidden;
        }

        /* Unified Portal Card */
        .portal-card {
            background: #ffffff;
            border-radius: 8px;
            border: 1px solid #cbd5e1;
            box-shadow: 0 2px 6px rgba(0, 0, 0, 0.05);
            flex: 1;
            display: flex;
            flex-direction: column;
            overflow: hidden;
            height: 100%;
        }

        /* Modern Integrated Tab Navigation Bar */
        .tab-bar-container {
            background: #ffffff;
            border-bottom: 2px solid #e2e8f0;
            padding: 0 16px;
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 16px;
            flex-shrink: 0;
            user-select: none;
        }

        .tabs-nav-bar {
            display: flex;
            gap: 4px;
            align-items: flex-end;
            margin-bottom: -2px; /* Aligns active indicator perfectly with container border */
            overflow-x: auto;
            scrollbar-width: none; /* Firefox */
        }

        .tabs-nav-bar::-webkit-scrollbar {
            display: none; /* Chrome, Safari, Edge */
        }

        /* Redesigned Tab Navigation Item */
        .tab-nav-btn {
            background: transparent;
            border: none;
            border-bottom: 3px solid transparent;
            padding: 12px 16px 10px 16px;
            font-family: inherit;
            font-size: 13px;
            font-weight: 600;
            color: #64748b;
            cursor: pointer;
            display: inline-flex;
            align-items: center;
            gap: 8px;
            transition: all 0.2s ease-in-out;
            outline: none;
            white-space: nowrap;
            position: relative;
        }

        .tab-nav-btn .tab-icon {
            color: #94a3b8;
            transition: color 0.2s ease-in-out;
            flex-shrink: 0;
        }

        .tab-nav-btn:hover {
            color: #0f172a;
            border-bottom-color: #cbd5e1;
            background-color: rgba(241, 245, 249, 0.6);
            border-top-left-radius: 6px;
            border-top-right-radius: 6px;
        }

        .tab-nav-btn:hover .tab-icon {
            color: #0284c7;
        }

        /* Active Tab - Clean Underline & Vibrant Brand Colors */
        .tab-nav-btn.active {
            color: #0284c7;
            border-bottom-color: #0284c7;
            background-color: #ffffff;
            font-weight: 700;
        }

        .tab-nav-btn.active .tab-icon {
            color: #0284c7;
        }

        /* Refined Count Badge */
        .tab-badge {
            background: #f1f5f9;
            color: #64748b;
            font-size: 11px;
            font-weight: 700;
            padding: 2px 7px;
            border-radius: 10px;
            min-width: 20px;
            text-align: center;
            line-height: 1.2;
            transition: all 0.2s;
        }

        .tab-nav-btn.active .tab-badge {
            background: #e0f2fe;
            color: #0284c7;
        }

        .tab-nav-btn:hover .tab-badge {
            background: #e2e8f0;
            color: #0f172a;
        }

        /* Search Filter on Right Side of Tab Bar */
        .search-wrapper {
            position: relative;
            display: flex;
            align-items: center;
            flex-shrink: 0;
        }

        .search-input {
            width: 190px;
            padding: 6px 10px 6px 28px;
            border: 1px solid #cbd5e1;
            border-radius: 6px;
            font-size: 12px;
            background: #f8fafc;
            color: #1e293b;
            transition: border-color 0.2s, background-color 0.2s, width 0.3s;
        }

        .search-input:focus {
            outline: none;
            background: #ffffff;
            border-color: #0284c7;
            width: 230px;
            box-shadow: 0 0 0 2px rgba(2, 132, 199, 0.15);
        }

        .search-icon {
            position: absolute;
            left: 8px;
            font-size: 12px;
            color: #94a3b8;
            pointer-events: none;
        }

        /* Content Area */
        .tab-panel {
            flex: 1;
            display: flex;
            flex-direction: column;
            height: 100%;
            overflow: hidden;
            background: #ffffff;
        }

        /* Ticker Viewport & Scroll Area */
        .ticker-viewport {
            flex: 1;
            height: 100%;
            overflow-y: auto;
            overflow-x: hidden;
            padding: 12px 16px;
            position: relative;
            scrollbar-width: thin;
            scrollbar-color: #cbd5e1 #f8fafc;
        }

        .ticker-viewport::-webkit-scrollbar {
            width: 7px;
        }

        .ticker-viewport::-webkit-scrollbar-track {
            background: #f8fafc;
        }

        .ticker-viewport::-webkit-scrollbar-thumb {
            background-color: #cbd5e1;
            border-radius: 4px;
        }

        .ticker-viewport::-webkit-scrollbar-thumb:hover {
            background-color: #94a3b8;
        }

        .ticker-content {
            display: flex;
            flex-direction: column;
            gap: 9px;
        }

        /* Notice Card Item */
        .notice-card {
            background: #ffffff;
            border: 1px solid #e2e8f0;
            border-left: 4px solid #0284c7;
            border-radius: 6px;
            padding: 10px 14px;
            transition: all 0.2s cubic-bezier(0.4, 0, 0.2, 1);
            position: relative;
        }

        .notice-card:hover {
            border-color: #cbd5e1;
            border-left-color: #800000;
            background: #f8fafc;
            transform: translateY(-1px);
            box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.08), 0 2px 4px -1px rgba(0, 0, 0, 0.04);
        }

        .notice-link {
            text-decoration: none;
            color: inherit;
            display: block;
        }

        .notice-row {
            display: flex;
            align-items: flex-start;
            gap: 14px;
        }

        /* Date Section - Classic Burgundy */
        .notice-date {
            flex-shrink: 0;
            display: flex;
            align-items: center;
            gap: 5px;
            background: #fff1f2;
            color: #800000;
            border: 1px solid #fecdd3;
            padding: 4px 9px;
            border-radius: 4px;
            font-size: 11.5px;
            font-weight: 700;
            white-space: nowrap;
        }

        .date-icon {
            font-size: 11px;
        }

        /* Notice Title */
        .notice-title-box {
            flex: 1;
            min-width: 0;
        }

        .notice-title {
            color: #0f172a;
            font-size: 13px;
            font-weight: 600;
            line-height: 1.45;
            display: block;
            word-wrap: break-word;
        }

        .notice-card:hover .notice-title {
            color: #0284c7;
        }

        .item-remarks {
            font-size: 11px;
            color: #64748b;
            margin-top: 3px;
            font-style: italic;
        }

        /* Empty State */
        .empty-state {
            padding: 50px 20px;
            text-align: center;
            color: #64748b;
            margin: auto;
        }

        .empty-icon {
            font-size: 40px;
            margin-bottom: 10px;
            opacity: 0.6;
        }

        .empty-title {
            font-size: 15px;
            font-weight: 700;
            color: #334155;
            margin-bottom: 4px;
        }

        .empty-desc {
            font-size: 12.5px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Literal ID="litStatusBanner" runat="server" EnableViewState="false"></asp:Literal>

        <div class="portal-container">
            <!-- Single Unified Portal Card -->
            <div class="portal-card">
                <!-- Top Integrated Tab Navigation Bar -->
                <div class="tab-bar-container">
                    <div class="tabs-nav-bar" id="tabsNavBar">
                        <asp:Literal ID="litTabsNav" runat="server" EnableViewState="false"></asp:Literal>
                    </div>

                    <!-- Clean Search Box -->
                    <div class="search-wrapper">
                        <span class="search-icon">&#128269;</span>
                        <input type="text" id="searchInput" class="search-input" placeholder="Search notices..." onkeyup="filterNotices()" title="Type keyword or date to filter" />
                    </div>
                </div>

                <!-- Content Area with Tab Panels -->
                <asp:Literal ID="litTabsContent" runat="server" EnableViewState="false"></asp:Literal>
            </div>
        </div>
    </form>

    <script type="text/javascript">
        // Global variables for Ticker and Tab Management
        var currentActiveTabId = "";
        var tickerTimer = null;
        var scrollSpeed = 1; // Pixels per tick
        var scrollIntervalMs = 50; // Milliseconds between ticks
        var isUserHovering = false;
        var isPaused = false;

        // Initialize on DOM Ready
        window.onload = function () {
            // Find initially active tab button
            var firstTabBtn = document.querySelector(".tab-nav-btn.active");
            if (firstTabBtn) {
                var tabId = firstTabBtn.getAttribute("data-tab-id");
                currentActiveTabId = tabId;
            } else {
                var anyBtn = document.querySelector(".tab-nav-btn");
                if (anyBtn) {
                    var tabId = anyBtn.getAttribute("data-tab-id");
                    switchTab(tabId, anyBtn);
                }
            }

            // Start Ticker Loop
            startTickerLoop();
        };

        // Switch Tab on Click
        function switchTab(tabId, btnElement) {
            // Update Tab Button States
            var allBtns = document.querySelectorAll(".tab-nav-btn");
            for (var i = 0; i < allBtns.length; i++) {
                allBtns[i].className = allBtns[i].className.replace(/\bactive\b/g, "").trim();
            }
            if (btnElement) {
                btnElement.className += " active";
            }

            // Update Panel Visibility
            var allPanels = document.querySelectorAll(".tab-panel");
            for (var j = 0; j < allPanels.length; j++) {
                allPanels[j].style.display = "none";
                allPanels[j].className = allPanels[j].className.replace(/\bactive\b/g, "").trim();
            }

            var targetPanel = document.getElementById(tabId);
            if (targetPanel) {
                targetPanel.style.display = "block";
                targetPanel.className += " active";
                // Reset scroll to top when changing tab
                var vp = targetPanel.querySelector(".ticker-viewport");
                if (vp) {
                    vp.scrollTop = 0;
                }
            }

            currentActiveTabId = tabId;

            // Clear search filter when switching tab
            var searchInput = document.getElementById("searchInput");
            if (searchInput && searchInput.value) {
                searchInput.value = "";
                filterNotices();
            }
        }

        // Ticker Auto-scroll Animation
        function startTickerLoop() {
            if (tickerTimer) {
                clearInterval(tickerTimer);
            }
            tickerTimer = setInterval(tickScroll, scrollIntervalMs);
        }

        function tickScroll() {
            if (isPaused || isUserHovering) {
                return;
            }

            if (!currentActiveTabId) {
                return;
            }

            var panel = document.getElementById(currentActiveTabId);
            if (!panel) {
                return;
            }

            var viewport = panel.querySelector(".ticker-viewport");
            if (!viewport) {
                return;
            }

            // Check if scrollable
            if (viewport.scrollHeight <= viewport.clientHeight) {
                return;
            }

            // Scroll down
            viewport.scrollTop += scrollSpeed;

            // If reached the bottom, smoothly loop back to top
            if (viewport.scrollTop + viewport.clientHeight >= viewport.scrollHeight - 2) {
                isPaused = true;
                setTimeout(function () {
                    viewport.scrollTop = 0;
                    isPaused = false;
                }, 1200);
            }
        }

        // Pause auto-scroll when mouse hovers over notice
        function pauseTicker(tabId) {
            isUserHovering = true;
        }

        // Resume auto-scroll when mouse leaves
        function resumeTicker(tabId) {
            isUserHovering = false;
        }

        // Client-side search and instant filter
        function filterNotices() {
            var searchInput = document.getElementById("searchInput");
            var term = searchInput.value.toLowerCase().trim();

            if (!currentActiveTabId) {
                return;
            }

            var panel = document.getElementById(currentActiveTabId);
            if (!panel) {
                return;
            }

            var cards = panel.querySelectorAll(".notice-card");

            for (var i = 0; i < cards.length; i++) {
                var searchData = cards[i].getAttribute("data-search") || "";
                if (term === "" || searchData.indexOf(term) > -1) {
                    cards[i].style.display = "block";
                } else {
                    cards[i].style.display = "none";
                }
            }

            // Pause scrolling if user is actively searching
            if (term !== "") {
                isUserHovering = true;
            } else {
                isUserHovering = false;
            }
        }
    </script>
</body>
</html>
