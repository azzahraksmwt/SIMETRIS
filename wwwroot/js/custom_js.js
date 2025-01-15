document.addEventListener("DOMContentLoaded", function (event) {
    const showNavbar = (toggleId, navId, bodyId) => {
        const toggle = document.getElementById(toggleId),
            nav = document.getElementById(navId),
            bodypd = document.getElementById(bodyId);

        // Validate that all variables exist
        if (toggle && nav && bodypd) {
            toggle.addEventListener('click', () => {
                // show/hide navbar
                nav.classList.toggle('show');
                // change icon
                toggle.classList.toggle('bx-x');
                // add/remove padding to body
                bodypd.classList.toggle('body-pd');
                // Shift the header toggle button
                document.querySelector('.header_toggle').classList.toggle('shift-right');
                // Add/remove active class on navbar
                nav.classList.toggle('active');
            });
        }
    }

    showNavbar('header-toggle', 'nav-bar', 'body-pd');

    /*===== LINK ACTIVE =====*/
    const linkColor = document.querySelectorAll('.nav_link');

    function colorLink() {
        if (linkColor) {
            linkColor.forEach(l => l.classList.remove('active'));
            this.classList.add('active');
        }
    }

    linkColor.forEach(l => l.addEventListener('click', colorLink));
});