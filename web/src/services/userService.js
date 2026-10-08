import { api, USE_MOCK } from './api.js';
import { mockUser } from '../data/mock/users.js';

let user = { ...mockUser };

// Expected API: GET /me   PUT /me   (photo can be a URL or data URL until uploads exist)
export const getProfile = async () => (USE_MOCK ? { ...user } : api.get('/me'));
export const updateProfile = async (patch) => {
  if (!USE_MOCK) return api.put('/me', patch);
  user = { ...user, ...patch };
  return { ...user };
};
