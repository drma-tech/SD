"use strict";

export const slider = {
    initLists(id, size, refresh) {
        let mobile = window.innerHeight < 600 || window.innerWidth < 600;

        const el = document.getElementById(id);
        if (!el) return;
        const margin = mobile ? "4px" : "8px";

        if (el.dataset.splideInit === "1") {
            if (refresh && el.splide) {
                el.splide.refresh();
            }
            return;
        }

        const temp = new Splide(el, {
            autoWidth: true,
            gap: margin,
            pagination: false
        });

        temp.mount();
        el.splide = temp;
        el.dataset.splideInit = "1";
    },
    initTrailers(id) {
        let mobile = window.innerHeight < 600 || window.innerWidth < 600;

        const el = document.getElementById(id);
        if (!el) return;
        const margin = mobile ? "4px" : "8px";

        if (el.dataset.splideInit === "1") return;
        el.dataset.splideInit = "1";

        const temp = new Splide(el, {
            autoWidth: true,
            gap: margin,
            pagination: false,
            grid: {
                rows: 3,
                gap: {
                    row: margin,
                },
            },
        });

        temp.mount(window.splide.Extensions);
    },
};