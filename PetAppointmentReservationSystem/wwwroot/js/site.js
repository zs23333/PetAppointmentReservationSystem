document.addEventListener("DOMContentLoaded", function () {
    var toggle = document.getElementById("sidebarToggle");
    var sidebar = document.getElementById("sidebar");
    var overlay = document.getElementById("sidebarOverlay");

    function closeSidebar() {
        sidebar?.classList.remove("open");
        overlay?.classList.remove("open");
    }

    toggle?.addEventListener("click", function () {
        sidebar?.classList.toggle("open");
        overlay?.classList.toggle("open");
    });

    overlay?.addEventListener("click", closeSidebar);

    document.querySelectorAll(".pc-nav .nav-link").forEach(function (link) {
        link.addEventListener("click", closeSidebar);
    });

    var banner = document.querySelector(".alert-banner");
    if (banner) {
        setTimeout(function () {
            banner.style.transition = "opacity 0.4s ease";
            banner.style.opacity = "0";
            setTimeout(function () { banner.remove(); }, 400);
        }, 4000);
    }
});