import { api, ApiError } from './api.js';
import { getLanguage } from '../i18n/i18n.js';

// GET /admin/studios -> every studio, paused ones included, with both languages:
// { id, nameAr, nameEn, descriptionAr, descriptionEn, imageUrl, pricePerHour, openHour, closeHour, isActive }
export const getStudios = () => api.get('/admin/studios');

// The admin API has no "one studio" endpoint, so the edit page picks it from the list.
export async function getStudio(id) {
  const studio = (await getStudios()).find((s) => String(s.id) === String(id));
  if (!studio) throw new ApiError(404, 'STUDIO_NOT_FOUND');
  return studio;
}

// Both take the full form: { nameAr, nameEn, descriptionAr, descriptionEn, imageUrl, pricePerHour, openHour, closeHour }.
export const createStudio = (studio) => api.post('/admin/studios', studio);
export const updateStudio = (id, studio) => api.put(`/admin/studios/${id}`, studio);

// Pause (false) or resume (true). A paused studio disappears from the customer site; its bookings stay.
export const setStudioActive = (id, isActive) => api.patch(`/admin/studios/${id}/status`, { isActive });

// The studio's name / description in the current language.
export const studioName = (studio) => (getLanguage() === 'ar' ? studio.nameAr : studio.nameEn);
export const studioDescription = (studio) => (getLanguage() === 'ar' ? studio.descriptionAr : studio.descriptionEn);
