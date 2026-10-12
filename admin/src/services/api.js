// Central HTTP client for the admin app: base URL, language, admin token, JSON and errors.
// Pages never call fetch; they call a service, and the service calls api.
import { API_BASE_URL } from '../data/config.js';
import { getLanguage } from '../i18n/i18n.js';

const TOKEN_KEY = 'frame_admin_token';

// Server error body: { status, code, message, errors, traceId } -> ApiError(status, code, fieldErrors).
export class ApiError extends Error {
  constructor(status, code, fieldErrors = {}) {
    super(code);
    this.name = 'ApiError';
    this.status = status; // 0 = server unreachable
    this.code = code;
    this.fieldErrors = fieldErrors; // 400 VALIDATION_ERROR: { pricePerHour: ['PRICE_INVALID'] }
  }
}

// The admin token lives for the browser tab only (sessionStorage), never in a cookie or the URL.
export const getToken = () => { try { return sessionStorage.getItem(TOKEN_KEY); } catch { return null; } };
export const setToken = (token) => { try { token ? sessionStorage.setItem(TOKEN_KEY, token) : sessionStorage.removeItem(TOKEN_KEY); } catch { /* storage unavailable */ } };

async function request(path, { method = 'GET', body } = {}) {
  const headers = { Accept: 'application/json', 'Accept-Language': getLanguage() };
  if (body !== undefined) headers['Content-Type'] = 'application/json';
  const token = getToken();
  if (token) headers.Authorization = `Bearer ${token}`;

  let res;
  try {
    res = await fetch(API_BASE_URL + path, { method, headers, body: body === undefined ? undefined : JSON.stringify(body) });
  } catch {
    throw new ApiError(0, 'NETWORK_ERROR'); // server down or blocked by CORS
  }

  const data = res.status === 204 ? null : await res.json().catch(() => null);
  if (res.ok) return data;

  if (res.status === 401) setToken(null); // expired or invalid token: log in again
  throw new ApiError(res.status, data?.code ?? 'UNKNOWN_ERROR', data?.errors ?? {});
}

export const api = {
  get: (path) => request(path),
  post: (path, body) => request(path, { method: 'POST', body }),
  put: (path, body) => request(path, { method: 'PUT', body }),
  patch: (path, body) => request(path, { method: 'PATCH', body }),
};
