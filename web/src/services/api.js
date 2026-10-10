// Central HTTP client. Every backend call goes through here: base URL, language, auth token, JSON and errors.
// Pages never call fetch; they call a service, and the service calls api.
import { API_BASE_URL } from '../data/config.js';
import { getLanguage } from '../i18n/i18n.js';

const TOKEN_KEY = 'frame_token';
const NETWORK_ERROR = 'NETWORK_ERROR';
const UNKNOWN_ERROR = 'UNKNOWN_ERROR';

// Every failed request becomes an ApiError with the server's error code.
// Server body: { status, code, message, errors: { field: ['REQUIRED'] } | null, traceId }
export class ApiError extends Error {
  constructor(status, code, fieldErrors = {}) {
    super(code);
    this.name = 'ApiError';
    this.status = status;          // 0 = server unreachable
    this.code = code;              // SLOT_TAKEN, VALIDATION_ERROR, STUDIO_NOT_FOUND...
    this.fieldErrors = fieldErrors; // { date: ['REQUIRED'] } for 400 validation errors
  }
}

// The JWT lives for the browser tab only (sessionStorage), never in a cookie or in the URL.
export const getToken = () => { try { return sessionStorage.getItem(TOKEN_KEY); } catch { return null; } };
export const setToken = (token) => { try { token ? sessionStorage.setItem(TOKEN_KEY, token) : sessionStorage.removeItem(TOKEN_KEY); } catch { /* storage unavailable */ } };
export const isLoggedIn = () => Boolean(getToken());

async function request(path, { method = 'GET', body } = {}) {
  const headers = { Accept: 'application/json', 'Accept-Language': getLanguage() };
  if (body !== undefined) headers['Content-Type'] = 'application/json';
  const token = getToken();
  if (token) headers.Authorization = `Bearer ${token}`;

  let res;
  try {
    res = await fetch(API_BASE_URL + path, { method, headers, body: body === undefined ? undefined : JSON.stringify(body) });
  } catch {
    throw new ApiError(0, NETWORK_ERROR); // server down, no internet, or blocked by CORS
  }

  const data = res.status === 204 ? null : await res.json().catch(() => null);
  if (res.ok) return data;

  if (res.status === 401) setToken(null); // expired or invalid token: forget it so the user logs in again
  throw new ApiError(res.status, data?.code ?? UNKNOWN_ERROR, data?.errors ?? {});
}

export const api = {
  get: (path) => request(path),
  post: (path, body) => request(path, { method: 'POST', body }),
  put: (path, body) => request(path, { method: 'PUT', body }),
  delete: (path) => request(path, { method: 'DELETE' }),
};