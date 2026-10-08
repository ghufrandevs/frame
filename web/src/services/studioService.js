import { api, USE_MOCK } from './api.js';
import { studios } from '../data/mock/studios.js';

// Expected API: GET /studios -> Studio[]   GET /studios/:id -> Studio
// Studio = { id, image, ratePerHour, areaM2, maxPeople } (text comes from i18n keyed by id)
export const getStudios = async () => (USE_MOCK ? studios : api.get('/studios'));
export const getStudioById = async (id) =>
  USE_MOCK ? studios.find((s) => s.id === id) ?? null : api.get(`/studios/${encodeURIComponent(id)}`);
