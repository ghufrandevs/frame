import { api, setToken, isLoggedIn } from './api.js';

// Customer sign-up, login and logout. Both endpoints answer AuthResponse = { token, expiresAt, user };
// the token is kept by api.js and sent on every request from then on.
// POST /auth/login    { email, password }                     -> 200 AuthResponse | 401 INVALID_CREDENTIALS
// POST /auth/register { fullName, email, phone, password }    -> 201 AuthResponse | 409 EMAIL_TAKEN | 400 VALIDATION_ERROR
export { isLoggedIn };

const RETURN_KEY = 'frame_return_path';

export async function login(email, password) {
  const response = await api.post('/auth/login', { email: email.trim(), password });
  setToken(response.token);
  return response.user;
}

export async function register({ fullName, email, phone, password }) {
  const response = await api.post('/auth/register', { fullName: fullName.trim(), email: email.trim(), phone: phone.trim(), password });
  setToken(response.token);
  return response.user;
}

export const logout = () => setToken(null);

// A page that needs login sends the customer to #/login and remembers where they were going.
// After a successful login the login page calls takeReturnPath() and navigates there.
export function rememberReturnPath(path) {
  try { sessionStorage.setItem(RETURN_KEY, path); } catch { /* storage unavailable */ }
}
export function takeReturnPath() {
  try {
    const path = sessionStorage.getItem(RETURN_KEY) || '/';
    sessionStorage.removeItem(RETURN_KEY);
    return path;
  } catch {
    return '/';
  }
}
