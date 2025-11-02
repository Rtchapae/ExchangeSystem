// API utilities
class ApiClient {
    constructor() {
        this.baseUrl = '/api';
        // Определяем базовый URL для запросов
        this.protocol = window.location.protocol;
        this.host = window.location.host;
        this.fullBaseUrl = `${this.protocol}//${this.host}/api`;
    }

    async request(endpoint, options = {}) {
        const token = localStorage.getItem('authToken') || localStorage.getItem('token');
        // Используем относительный URL для избежания проблем с портами
        // Убираем дублирование /api если endpoint уже начинается с /api
        const cleanEndpoint = endpoint.startsWith('/api/') ? endpoint : `${this.baseUrl}${endpoint}`;
        const url = cleanEndpoint;

        const defaultOptions = {
            credentials: 'include', // Включаем отправку cookies для авторизации
            headers: {
                'Content-Type': 'application/json',
                ...(token && { 'Authorization': `Bearer ${token}` })
            }
        };

        const config = {
            ...defaultOptions,
            ...options,
            headers: {
                ...defaultOptions.headers,
                ...options.headers
            },
            credentials: 'include' // Убеждаемся, что credentials включен
        };

        try {
            const response = await fetch(url, config);
            
            // Check if response is JSON
            const contentType = response.headers.get('content-type');
            if (!contentType || !contentType.includes('application/json')) {
                throw new Error('Сервер вернул не JSON ответ. Возможно, сервер недоступен.');
            }
            
            const data = await response.json();

            if (!response.ok) {
                if (response.status === 401) {
                    // Unauthorized - redirect to login
                    localStorage.removeItem('authToken');
                    localStorage.removeItem('token');
                    window.location.href = '/';
                    return;
                }
                throw new Error(data.message || 'Ошибка API');
            }

            return data;
        } catch (error) {
            console.error('API request error:', error);
            throw error;
        }
    }

    async get(endpoint) {
        return this.request(endpoint, { method: 'GET' });
    }

    async post(endpoint, data = null) {
        return this.request(endpoint, {
            method: 'POST',
            body: data ? JSON.stringify(data) : null
        });
    }

    async put(endpoint, data = null) {
        return this.request(endpoint, {
            method: 'PUT',
            body: data ? JSON.stringify(data) : null
        });
    }

    async delete(endpoint) {
        return this.request(endpoint, { method: 'DELETE' });
    }

    async uploadFile(endpoint, file, additionalData = {}) {
        const formData = new FormData();
        formData.append('file', file);
        
        Object.keys(additionalData).forEach(key => {
            formData.append(key, additionalData[key]);
        });

        const token = localStorage.getItem('authToken') || localStorage.getItem('token');
        const url = `${this.baseUrl}${endpoint}`;

        try {
            const response = await fetch(url, {
                method: 'POST',
                credentials: 'include', // Включаем отправку cookies для авторизации
                headers: {
                    ...(token && { 'Authorization': `Bearer ${token}` })
                },
                body: formData
            });

            const data = await response.json();

            if (!response.ok) {
                if (response.status === 401) {
                    localStorage.removeItem('authToken');
                    localStorage.removeItem('token');
                    window.location.href = '/';
                    return;
                }
                throw new Error(data.message || 'Ошибка загрузки файла');
            }

            return data;
        } catch (error) {
            console.error('File upload error:', error);
            throw error;
        }
    }
}

// Global API instance
const api = new ApiClient();

// Utility functions
function showAlert(message, type = 'info') {
    // Remove existing alerts
    const existingAlerts = document.querySelectorAll('.alert-dismissible');
    existingAlerts.forEach(alert => alert.remove());

    // Create new alert
    const alertDiv = document.createElement('div');
    alertDiv.className = `alert alert-${type} alert-dismissible fade show`;
    alertDiv.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
    `;

    // Insert at the top of the main content
    const main = document.querySelector('main');
    if (main) {
        main.insertBefore(alertDiv, main.firstChild);
    }

    // Auto-dismiss after 5 seconds
    setTimeout(() => {
        if (alertDiv.parentNode) {
            alertDiv.remove();
        }
    }, 5000);
}

function formatDate(dateString) {
    return new Date(dateString).toLocaleDateString('ru-RU');
}

function formatDateTime(dateString) {
    return new Date(dateString).toLocaleString('ru-RU');
}

function formatCurrency(amount) {
    return new Intl.NumberFormat('ru-RU', {
        style: 'currency',
        currency: 'BYN'
    }).format(amount);
}

function formatNumber(number, decimals = 2) {
    return new Intl.NumberFormat('ru-RU', {
        minimumFractionDigits: decimals,
        maximumFractionDigits: decimals
    }).format(number);
}

