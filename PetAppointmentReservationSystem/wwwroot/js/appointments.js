function openAppointmentModal(el) {
    const modal = document.getElementById("appointmentModal");
    if (!modal) return;

    document.getElementById("modalPetName").textContent = el.dataset.pet || "";
    document.getElementById("modalOwner").textContent = el.dataset.owner || "";
    document.getElementById("modalService").textContent = el.dataset.service || "";
    document.getElementById("modalType").textContent = el.dataset.type || "";
    document.getElementById("modalStaff").textContent = el.dataset.staff || "";
    document.getElementById("modalStart").textContent = el.dataset.start || "";
    document.getElementById("modalDuration").textContent = (el.dataset.duration || "") + " min";
    document.getElementById("modalPhone").textContent = el.dataset.phone || "";
    document.getElementById("modalEmail").textContent = el.dataset.email || "";
    document.getElementById("modalNotes").textContent = el.dataset.notes || "";

    modal.classList.remove("hidden");
}

function closeAppointmentModal() {
    const modal = document.getElementById("appointmentModal");
    if (!modal) return;
    modal.classList.add("hidden");
}

document.addEventListener("click", function (e) {
    const modal = document.getElementById("appointmentModal");
    if (!modal || modal.classList.contains("hidden")) return;
    if (e.target === modal) closeAppointmentModal();
});

document.addEventListener("keydown", function (e) {
    if (e.key === "Escape") closeAppointmentModal();
});

document.addEventListener("keydown", function (e) {
    if (e.key !== "Enter" && e.key !== " ") return;
    const el = document.activeElement;
    if (el && el.classList && el.classList.contains("appointment-bar")) {
        e.preventDefault();
        openAppointmentModal(el);
    }
});

document.addEventListener("DOMContentLoaded", function () {
    const toggle = document.getElementById("sidebarToggle");
    const sidebar = document.getElementById("sidebar");
    const overlay = document.getElementById("sidebarOverlay");

    function closeSidebar() {
        sidebar?.classList.remove("open");
        overlay?.classList.remove("open");
    }

    toggle?.addEventListener("click", function () {
        sidebar?.classList.toggle("open");
        overlay?.classList.toggle("open");
    });

    overlay?.addEventListener("click", closeSidebar);

    document.querySelectorAll(".sidebar-nav a").forEach(function (link) {
        link.addEventListener("click", closeSidebar);
    });

    const pills = document.querySelectorAll(".pill-switcher .pill");
    pills.forEach(function (pill) {
        pill.addEventListener("click", function () {
            pills.forEach(p => p.classList.remove("active"));
            pill.classList.add("active");
        });
    });
});