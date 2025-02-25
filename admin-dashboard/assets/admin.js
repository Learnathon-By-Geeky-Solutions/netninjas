document.addEventListener("DOMContentLoaded", function () {
// Toggle Sidebar
    const sidebar = document.querySelector('.sidebar');
    const sidebarMenu = document.querySelector('.sidebar-menu-icon');
    const closeSidebarIcon = document.querySelector('.close-sidebar-icon');
    const mainContainer = document.querySelector('.main-container');

    function toggleSidebar() {
        if (window.innerWidth >= 768) {
            sidebar.classList.toggle('collapsed');
            mainContainer.classList.toggle('expanded');
        } else {
            sidebar.classList.toggle('show-sidebar');
        }
    }

    function handleResize() {
        if (window.innerWidth >= 768) { // Ensure sidebar is not in "show-sidebar" mode on large screens
            sidebar.classList.remove('show-sidebar');
        } else { // Ensure sidebar is not in "collapsed" mode on small screens
            sidebar.classList.remove('collapsed');
            mainContainer.classList.remove('expanded');
        }
    }

    sidebarMenu.addEventListener('click', toggleSidebar);
    closeSidebarIcon?.addEventListener('click', () => {
        sidebar.classList.remove('show-sidebar'); 
    });

    window.addEventListener('resize', handleResize);
    handleResize(); // Initialize sidebar state based on screen size

// Header dropdown toggle
    const dropdownToggle = document.querySelector('.dropdown-toggle');
    const dropdownMenu = document.querySelector('.dropdown-menu');

    dropdownToggle.addEventListener('click', (e) => {
        e.stopPropagation();
        dropdownMenu.classList.toggle('show-dropdown-menu');
    });

    // Close dropdown when clicking outside
    document.addEventListener('click', (e) => {
        if (!dropdownMenu.contains(e.target) && !dropdownToggle.contains(e.target)) {
            dropdownMenu.classList.remove('show-dropdown-menu');
        }
    });

// Enter and exit fullscreen
    const fullscreenToggle = document.getElementById("fullscreen");
    const enterFullscreenIcon = document.getElementById("enter-fullscreen");
    const exitFullscreenIcon = document.getElementById("exit-fullscreen");

    fullscreenToggle.addEventListener("click", function () {
        if (!document.fullscreenElement) {
            document.documentElement.requestFullscreen();
        } else {
            document.exitFullscreen();
        }
    });

    // Update Icon: Listen for fullscreen changes (Esc key, system UI, etc.)
    document.addEventListener("fullscreenchange", function () {
        if (document.fullscreenElement) {
            enterFullscreenIcon.style.display = "none";
            exitFullscreenIcon.style.display = "inline";
        } else {
            enterFullscreenIcon.style.display = "inline";
            exitFullscreenIcon.style.display = "none";
        }
    });


// Theme switcher functionality
    const themes = ['dark-slate', 'dark-blue', 'charcoal-black', 'midnight-navy', 'deep-navy', 'white', 'midnight-blue', 'dark-gray'];
    let storedSidebarTheme = localStorage.getItem('sidebarTheme') || themes[0]; // Default to first theme
    let storedHeaderTheme = localStorage.getItem('headerTheme') || themes[5]; // Default to first theme

    if(!themes.some(theme => theme.includes(storedSidebarTheme)))
        storedSidebarTheme = themes[0];

    if(!themes.some(theme => theme.includes(storedHeaderTheme)))
        storedHeaderTheme = themes[5];

    function applyTheme(section, themeName) {
        // First remove and then show check icon on theme apply
        if(section === "sidebar") {
            document.querySelector(".selected-sidebar-theme")?.classList.remove("selected-sidebar-theme");
            document.querySelector('.changeSidebarTheme[data-theme="' + themeName + '"]').classList.add("selected-sidebar-theme");
        } else if(section === "header") {
            document.querySelector(".selected-header-theme")?.classList.remove("selected-header-theme");
            document.querySelector('.changeHeaderTheme[data-theme="' + themeName + '"]').classList.add("selected-header-theme");
        }
        
        if(themeName == "dark-slate") {
            document.documentElement.style.setProperty("--" + section + "-bg-color", "#212631");
            document.documentElement.style.setProperty("--" + section + "-icon-color", "#ffffff61");
            document.documentElement.style.setProperty("--" + section + "-text-color", "#ffffffde");
            document.documentElement.style.setProperty("--" + section + "-link-text-color", "#ffffffde");
            document.documentElement.style.setProperty("--" + section + "-link-hover-bg-color", "#2a303d");
            document.documentElement.style.setProperty("--" + section + "-link-hover-text-color", "#fff");
            document.documentElement.style.setProperty("--" + section + "-header-hover-text-color", "#fff");
            document.documentElement.style.setProperty("--" + section + "-border-color", "#323a49");
        } else if(themeName == "dark-blue") {
            document.documentElement.style.setProperty("--" + section + "-bg-color", "#153657");
            document.documentElement.style.setProperty("--" + section + "-icon-color", "#ffffffa3");
            document.documentElement.style.setProperty("--" + section + "-text-color", "#ffffffde");
            document.documentElement.style.setProperty("--" + section + "-link-text-color", "#ffffffde");
            document.documentElement.style.setProperty("--" + section + "-link-hover-bg-color", "#2d5f91");
            document.documentElement.style.setProperty("--" + section + "-link-hover-text-color", "#fff");
            document.documentElement.style.setProperty("--" + section + "-header-hover-text-color", "#fff");
            document.documentElement.style.setProperty("--" + section + "-border-color", "#12293f");
        } else if(themeName == "charcoal-black") {
            document.documentElement.style.setProperty("--" + section + "-bg-color", "#212529");
            document.documentElement.style.setProperty("--" + section + "-icon-color", "#ffffff80");
            document.documentElement.style.setProperty("--" + section + "-text-color", "#ffffffde");
            document.documentElement.style.setProperty("--" + section + "-link-text-color", "#ffffffde");
            document.documentElement.style.setProperty("--" + section + "-link-hover-bg-color", "#343a40");
            document.documentElement.style.setProperty("--" + section + "-link-hover-text-color", "#fff");
            document.documentElement.style.setProperty("--" + section + "-header-hover-text-color", "#fff");
            document.documentElement.style.setProperty("--" + section + "-border-color", "#323940");
            
        } else if(themeName == "midnight-navy") {
            document.documentElement.style.setProperty("--" + section + "-bg-color", "#121D3E");
            document.documentElement.style.setProperty("--" + section + "-icon-color", "#ffffffa3");
            document.documentElement.style.setProperty("--" + section + "-text-color", "#ffffffde");
            document.documentElement.style.setProperty("--" + section + "-link-text-color", "#c0c0c1");
            document.documentElement.style.setProperty("--" + section + "-link-hover-bg-color", "#2E4071");
            document.documentElement.style.setProperty("--" + section + "-link-hover-text-color", "#fff");
            document.documentElement.style.setProperty("--" + section + "-header-hover-text-color", "#fff");
            document.documentElement.style.setProperty("--" + section + "-border-color", "#2a3d76");
        } else if(themeName == "deep-navy") {
            document.documentElement.style.setProperty("--" + section + "-bg-color", "#1f283e");
            document.documentElement.style.setProperty("--" + section + "-icon-color", "#ffffffa3");
            document.documentElement.style.setProperty("--" + section + "-text-color", "#ffffffde");
            document.documentElement.style.setProperty("--" + section + "-link-text-color", "#c0c0c1");
            document.documentElement.style.setProperty("--" + section + "-link-hover-bg-color", "#283555");
            document.documentElement.style.setProperty("--" + section + "-link-hover-text-color", "#fff");
            document.documentElement.style.setProperty("--" + section + "-header-hover-text-color", "#fff");
            document.documentElement.style.setProperty("--" + section + "-border-color", "#273455");
        } else if(themeName == "white") {
            document.documentElement.style.setProperty("--" + section + "-bg-color", "#ffffff");
            document.documentElement.style.setProperty("--" + section + "-icon-color", "#314252");
            document.documentElement.style.setProperty("--" + section + "-text-color", "#293846");
            document.documentElement.style.setProperty("--" + section + "-link-text-color", "#293846");
            document.documentElement.style.setProperty("--" + section + "-link-hover-bg-color", "#c8c8c833");
            document.documentElement.style.setProperty("--" + section + "-link-hover-text-color", "#293846");
            document.documentElement.style.setProperty("--" + section + "-header-hover-text-color", "#293846");
            document.documentElement.style.setProperty("--" + section + "-border-color", "#dadada");
        } else if(themeName == "midnight-blue") {
            document.documentElement.style.setProperty("--" + section + "-bg-color", "#1a2035");
            document.documentElement.style.setProperty("--" + section + "-icon-color", "#ffffffa3");
            document.documentElement.style.setProperty("--" + section + "-text-color", "#ffffffde");
            document.documentElement.style.setProperty("--" + section + "-link-text-color", "#c0c0c1");
            document.documentElement.style.setProperty("--" + section + "-link-hover-bg-color", "#232b47");
            document.documentElement.style.setProperty("--" + section + "-link-hover-text-color", "#fff");
            document.documentElement.style.setProperty("--" + section + "-header-hover-text-color", "#fff");
            document.documentElement.style.setProperty("--" + section + "-border-color", "#252d4c");
        } else if(themeName == "dark-gray") {
            document.documentElement.style.setProperty("--" + section + "-bg-color", "#343a40");
            document.documentElement.style.setProperty("--" + section + "-icon-color", "#ffffffa3");
            document.documentElement.style.setProperty("--" + section + "-text-color", "#c2c7d0");
            document.documentElement.style.setProperty("--" + section + "-link-text-color", "#c2c7d0");
            document.documentElement.style.setProperty("--" + section + "-link-hover-bg-color", "#ffffff1a");
            document.documentElement.style.setProperty("--" + section + "-link-hover-text-color", "#fff");
            document.documentElement.style.setProperty("--" + section + "-header-hover-text-color", "#fff");
            document.documentElement.style.setProperty("--" + section + "-border-color", "#495057");
        }
    }

    // Changing theme on load
    applyTheme("sidebar", storedSidebarTheme);
    applyTheme("header", storedHeaderTheme);

    // Changing theme on click
    document.querySelectorAll('.changeSidebarTheme').forEach(button => {
        button.addEventListener('click', function () {
            applyTheme("sidebar", this.getAttribute('data-theme'));
            localStorage.setItem('sidebarTheme', this.getAttribute('data-theme'));
        });
    });

    document.querySelectorAll('.changeHeaderTheme').forEach(button => {
    button.addEventListener('click', function () {
            applyTheme("header", this.getAttribute('data-theme'));
            localStorage.setItem('headerTheme', this.getAttribute('data-theme'));
        });
    });
});