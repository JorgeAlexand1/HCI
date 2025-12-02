// Solicitar permisos para notificaciones del navegador
async function requestNotificationPermission() {
    if ('Notification' in window) {
        const permission = await Notification.requestPermission();
        return permission === 'granted';
    }
    return false;
}

// Mostrar notificación del navegador
function showNotification(mensaje, tipo) {
    if ('Notification' in window && Notification.permission === 'granted') {
        const iconMap = {
            'info': '📋',
            'assignment': '👤',
            'escalation': '⚠️',
            'status_change': '🔄',
            'success': '✅',
            'error': '❌',
            'warning': '⚠️'
        };

        const icon = iconMap[tipo] || '🔔';
        
        new Notification('FISEI Incidentes', {
            body: mensaje,
            icon: '/favicon.ico',
            badge: '/favicon.ico',
            tag: tipo,
            requireInteraction: tipo === 'escalation' || tipo === 'assignment',
            vibrate: [200, 100, 200]
        });

        // Reproducir sonido de notificación
        playNotificationSound(tipo);
    } else {
        // Fallback: mostrar notificación en la UI
        showInAppNotification(mensaje, tipo);
    }
}

// Reproducir sonido de notificación
function playNotificationSound(tipo) {
    try {
        const audioContext = new (window.AudioContext || window.webkitAudioContext)();
        const oscillator = audioContext.createOscillator();
        const gainNode = audioContext.createGain();
        
        oscillator.connect(gainNode);
        gainNode.connect(audioContext.destination);
        
        // Diferentes frecuencias según el tipo
        const frequencies = {
            'info': 440,
            'assignment': 523,
            'escalation': 659,
            'status_change': 392,
            'success': 523,
            'error': 330,
            'warning': 587
        };
        
        oscillator.frequency.value = frequencies[tipo] || 440;
        oscillator.type = 'sine';
        
        gainNode.gain.setValueAtTime(0.3, audioContext.currentTime);
        gainNode.gain.exponentialRampToValueAtTime(0.01, audioContext.currentTime + 0.5);
        
        oscillator.start(audioContext.currentTime);
        oscillator.stop(audioContext.currentTime + 0.5);
    } catch (error) {
        console.log('No se pudo reproducir el sonido:', error);
    }
}

// Mostrar notificación dentro de la aplicación
function showInAppNotification(mensaje, tipo) {
    const container = document.getElementById('notification-container');
    if (!container) {
        const newContainer = document.createElement('div');
        newContainer.id = 'notification-container';
        newContainer.style.cssText = 'position:fixed;top:20px;right:20px;z-index:9999;max-width:350px;';
        document.body.appendChild(newContainer);
    }

    const notification = document.createElement('div');
    const colorMap = {
        'info': '#0dcaf0',
        'assignment': '#0d6efd',
        'escalation': '#ffc107',
        'status_change': '#6f42c1',
        'success': '#198754',
        'error': '#dc3545',
        'warning': '#fd7e14'
    };

    notification.className = 'alert alert-dismissible fade show';
    notification.style.cssText = `
        background: ${colorMap[tipo] || '#0dcaf0'};
        color: white;
        padding: 15px 20px;
        border-radius: 8px;
        margin-bottom: 10px;
        box-shadow: 0 4px 12px rgba(0,0,0,0.15);
        animation: slideIn 0.3s ease-out;
    `;
    
    notification.innerHTML = `
        <strong>${tipo.toUpperCase()}</strong>
        <p style="margin:5px 0 0 0;">${mensaje}</p>
        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="alert"></button>
    `;

    const notifContainer = document.getElementById('notification-container');
    notifContainer.appendChild(notification);

    // Auto-remover después de 5 segundos
    setTimeout(() => {
        notification.classList.remove('show');
        setTimeout(() => notification.remove(), 300);
    }, 5000);
}

// Estilos CSS para animación
const style = document.createElement('style');
style.textContent = `
    @keyframes slideIn {
        from {
            transform: translateX(400px);
            opacity: 0;
        }
        to {
            transform: translateX(0);
            opacity: 1;
        }
    }
`;
document.head.appendChild(style);

// Solicitar permisos al cargar la página
window.addEventListener('load', () => {
    requestNotificationPermission();
});
