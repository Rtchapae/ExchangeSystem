// Authentication utilities
class Auth {
    static getToken() {
        return localStorage.getItem('token');
    }

    static setToken(token) {
        localStorage.setItem('token', token);
    }

    static removeToken() {
        localStorage.removeItem('token');
    }

    static isAuthenticated() {
        return !!this.getToken();
    }

    static async validateToken() {
        const token = this.getToken();
        if (!token) return false;

        try {
            const response = await fetch('/api/auth/validate', {
                method: 'POST',
                credentials: 'include', // Включаем отправку cookies
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });

            const result = await response.json();
            return result.success;
        } catch (error) {
            console.error('Token validation error:', error);
            return false;
        }
    }

    static async getCurrentUser() {
        const token = this.getToken();
        if (!token) return null;

        try {
            const response = await fetch('/api/auth/validate', {
                method: 'POST',
                credentials: 'include', // Включаем отправку cookies
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });

            const result = await response.json();
            return result.success ? result.user : null;
        } catch (error) {
            console.error('Get current user error:', error);
            return null;
        }
    }

    static logout() {
        this.removeToken();
        window.location.href = '/';
    }
}

// Check authentication on page load
document.addEventListener('DOMContentLoaded', async function() {
    // ВРЕМЕННО ОТКЛЮЧЕНО - проверка авторизации
    console.log('Auth check disabled');
    return;
    
    const currentPath = window.location.pathname;
    
    // Skip auth check for login page
    if (currentPath === '/' || currentPath === '/Home/Index') {
        return;
    }

    const isAuthenticated = await Auth.validateToken();
    if (!isAuthenticated) {
        window.location.href = '/';
        return;
    }

    // Update user info in navbar
    const user = await Auth.getCurrentUser();
    if (user) {
        const usernameElement = document.getElementById('username');
        if (usernameElement) {
            usernameElement.textContent = user.fullName || user.username;
        }
    }
});

// Global logout function
function logout() {
    Auth.logout();
}



