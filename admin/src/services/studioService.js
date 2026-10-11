import { api } from './api.js';
import { getLanguage } from '../i18n/i18n.js';

// GET /admin/studios -> every studio, paused ones included, with both languages.
export const getStudios = () => api.get('/admin/studios');

// The studio's name in the current language.
export const studioName = (studio) => (getLanguage() === 'ar' ? studio.nameAr : studio.nameEn);
