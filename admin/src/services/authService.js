import { api, getToken, setToken } from './api.js';

// POST /admin/auth/login. Customer accounts get INVALID_CREDENTIALS, same as a wrong password.
export async function login(email, password) {
  const { token } = await api.post('/admin/auth/login', { email, password });
  setToken(token);
}

export const logout = () => setToken(null);
export const isLoggedIn = () => Boolean(getToken());
