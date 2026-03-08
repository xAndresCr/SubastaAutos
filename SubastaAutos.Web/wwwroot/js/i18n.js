i18next
    .use(i18nextHttpBackend)
    .init({
        lng: "en",
        fallbackLng: "en",
        backend: {
            loadPath: "/locales/{{lng}}/translation.json"
        }
    }, function () {
        document.querySelectorAll("[data-i18n]").forEach(function (element) {
            element.innerHTML = i18next.t(element.getAttribute("data-i18n"));
        });
    });