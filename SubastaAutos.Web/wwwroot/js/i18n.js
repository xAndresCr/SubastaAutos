/**
 * ============================================================================
 * i18n.js — Configuración de internacionalización con i18next
 * ============================================================================
 * 
 * Librerías usadas (cargadas vía CDN en _Layout.cshtml):
 *   - i18next:                      Motor principal de traducciones.
 *   - i18next-http-backend:         Carga los archivos JSON de /locales/{lng}/.
 *   - i18next-browser-languagedetector: Detecta y persiste el idioma en cookie.
 *
 * Flujo:
 *   1. Al cargar la página, i18next revisa la cookie "i18next_lng".
 *   2. Si no existe cookie, usa "es" como idioma por defecto.
 *   3. Carga el JSON correspondiente desde /locales/{lng}/translation.json.
 *   4. Recorre todos los elementos con atributo [data-i18n] y reemplaza su texto.
 *   5. Al cambiar idioma (switch), guarda la nueva preferencia en cookie (365 días).
 * ============================================================================
 */

i18next
    .use(i18nextHttpBackend)
    .use(i18nextBrowserLanguageDetector)
    .init({
        // Idioma por defecto si no hay cookie ni preferencia del navegador
        fallbackLng: "es",

        // Configuración del detector de idioma (persistencia)
        detection: {
            // Orden de prioridad para detectar el idioma:
            // 1. cookie  → lee la cookie "i18next_lng"
            // 2. navigator → idioma del navegador (solo si no hay cookie)
            order: ["cookie", "navigator"],

            // Dónde guardar la preferencia del usuario
            caches: ["cookie"],

            // Nombre de la cookie donde se almacena el idioma
            cookieName: "i18next_lng",

            // Expiración de la cookie: 365 días (persiste entre sesiones)
            cookieMinutes: 525600,

            // Opciones de la cookie
            cookieOptions: {
                path: "/",
                sameSite: "lax"
            }
        },

        // Configuración del backend (carga de archivos JSON)
        backend: {
            loadPath: "/locales/{{lng}}/translation.json"
        },

        // No usar namespaces, solo el archivo translation.json
        ns: ["translation"],
        defaultNS: "translation"

    }, function (err) {
        if (err) {
            console.error("Error al inicializar i18next:", err);
            return;
        }
        // Una vez cargadas las traducciones, actualizar toda la UI
        applyTranslations();
        syncLanguageSwitch();
    });

/**
 * Recorre TODOS los elementos con [data-i18n] y reemplaza su contenido.
 * 
 * Soporta tres formas:
 *   data-i18n="nav.users"                → reemplaza innerHTML
 *   data-i18n="[placeholder]common.search" → reemplaza atributo placeholder
 *   data-i18n="[title]auto.detailTitle"    → reemplaza atributo title
 */
function applyTranslations() {
    document.querySelectorAll("[data-i18n]").forEach(function (el) {
        var key = el.getAttribute("data-i18n");

        // Si la clave empieza con "[atributo]", traduce ese atributo
        if (key.charAt(0) === "[") {
            var closeBracket = key.indexOf("]");
            var attr = key.substring(1, closeBracket);
            var actualKey = key.substring(closeBracket + 1);
            el.setAttribute(attr, i18next.t(actualKey));
        } else {
            // Traduce el contenido del elemento
            el.innerHTML = i18next.t(key);
        }
    });
}

/**
 * Cambia el idioma activo.
 * i18next guarda automáticamente la nueva preferencia en la cookie.
 * Luego recarga la página para que Razor también refleje el cambio en datos dinámicos.
 */
function changeLanguage(lng) {
    i18next.changeLanguage(lng, function (err) {
        if (err) {
            console.error("Error al cambiar idioma:", err);
            return;
        }
        // Recargar la página para asegurar consistencia completa
        // (los elementos Razor server-side no cambian sin recarga)
        window.location.reload();
    });
}

/**
 * Sincroniza el estado visual del switch con el idioma actual.
 */
function syncLanguageSwitch() {
    var currentLng = i18next.language;
    // Normalizar: "es-CR" → "es", "en-US" → "en"
    if (currentLng && currentLng.length > 2) {
        currentLng = currentLng.substring(0, 2);
    }

    var switchEl = document.getElementById("languageSwitch");
    if (switchEl) {
        // checked = true → English, checked = false → Spanish
        switchEl.checked = (currentLng === "en");
    }

    // Actualizar la etiqueta visual
    var labelEl = document.getElementById("languageSwitchLabel");
    if (labelEl) {
        labelEl.textContent = (currentLng === "en") ? "EN" : "ES";
    }
}
