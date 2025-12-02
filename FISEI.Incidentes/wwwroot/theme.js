// Sistema de temas - UTA Incidentes
(function() {
    'use strict';

    // Obtener tema guardado o usar preferencia del sistema
    function getInitialTheme() {
        const savedTheme = localStorage.getItem('uta-theme');
        if (savedTheme) {
            return savedTheme;
        }
        
        // Detectar preferencia del sistema
        if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
            return 'dark';
        }
        
        return 'light';
    }

    // Aplicar tema al DOM
    function applyTheme(theme) {
        if (theme === 'dark') {
            document.documentElement.setAttribute('data-theme', 'dark');
        } else {
            document.documentElement.removeAttribute('data-theme');
        }
        localStorage.setItem('uta-theme', theme);
    }

    // Alternar tema
    function toggleTheme() {
        const currentTheme = document.documentElement.getAttribute('data-theme');
        const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
        applyTheme(newTheme);
        updateToggleButton(newTheme);
        
        // Disparar evento personalizado para componentes Blazor
        window.dispatchEvent(new CustomEvent('themeChanged', { detail: { theme: newTheme } }));
    }

    // Actualizar ícono del botón
    function updateToggleButton(theme) {
        const button = document.querySelector('.theme-toggle');
        if (button) {
            button.innerHTML = theme === 'dark' ? '☀️' : '🌙';
            button.setAttribute('aria-label', theme === 'dark' ? 'Cambiar a tema claro' : 'Cambiar a tema oscuro');
            button.setAttribute('title', theme === 'dark' ? 'Modo Claro' : 'Modo Oscuro');
        }
    }

    // Crear botón de toggle
    function createToggleButton() {
        // Verificar si ya existe
        if (document.querySelector('.theme-toggle')) {
            return;
        }

        const button = document.createElement('button');
        button.className = 'theme-toggle';
        button.setAttribute('type', 'button');
        button.setAttribute('aria-label', 'Cambiar tema');
        
        const currentTheme = document.documentElement.getAttribute('data-theme') || 'light';
        button.innerHTML = currentTheme === 'dark' ? '☀️' : '🌙';
        button.setAttribute('title', currentTheme === 'dark' ? 'Modo Claro' : 'Modo Oscuro');
        
        button.addEventListener('click', toggleTheme);
        
        document.body.appendChild(button);
    }

    // Escuchar cambios en la preferencia del sistema
    function setupSystemThemeListener() {
        if (window.matchMedia) {
            const darkModeQuery = window.matchMedia('(prefers-color-scheme: dark)');
            darkModeQuery.addEventListener('change', (e) => {
                // Solo aplicar si el usuario no ha establecido preferencia manual
                if (!localStorage.getItem('uta-theme')) {
                    const newTheme = e.matches ? 'dark' : 'light';
                    applyTheme(newTheme);
                    updateToggleButton(newTheme);
                }
            });
        }
    }

    // Inicialización
    function init() {
        // Aplicar tema inmediatamente para evitar flash
        const theme = getInitialTheme();
        applyTheme(theme);
        
        // Esperar a que el DOM esté listo
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', () => {
                createToggleButton();
                updateToggleButton(theme);
            });
        } else {
            createToggleButton();
            updateToggleButton(theme);
        }
        
        setupSystemThemeListener();
    }

    // Exponer funciones globales para uso desde Blazor
    window.utaTheme = {
        toggle: toggleTheme,
        set: applyTheme,
        get: () => document.documentElement.getAttribute('data-theme') || 'light',
        init: init
    };

    // Auto-inicializar
    init();

    // Re-crear botón después de navegaciones de Blazor
    let reconnectAttempts = 0;
    const maxReconnectAttempts = 10;
    
    const observer = new MutationObserver(() => {
        if (!document.querySelector('.theme-toggle') && reconnectAttempts < maxReconnectAttempts) {
            setTimeout(() => {
                createToggleButton();
                const currentTheme = document.documentElement.getAttribute('data-theme') || 'light';
                updateToggleButton(currentTheme);
                reconnectAttempts++;
            }, 100);
        }
    });

    if (document.body) {
        observer.observe(document.body, { childList: true, subtree: true });
    }
})();
