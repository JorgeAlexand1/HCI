window.authStore = {
  setToken: function (token) {
    try { sessionStorage.setItem('auth_token', token); return true; } catch { return false; }
  },
  getToken: function () {
    try { return sessionStorage.getItem('auth_token'); } catch { return null; }
  },
  removeToken: function () {
    try { sessionStorage.removeItem('auth_token'); return true; } catch { return false; }
  }
};