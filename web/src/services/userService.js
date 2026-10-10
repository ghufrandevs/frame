import { api } from './api.js';

// GET /auth/me -> ProfileResponse = { id, fullName, email, phone, role }
// PUT /auth/me { fullName, phone } -> ProfileResponse (the email cannot change: it is the login name)
const toUser = (p) => ({ name: p.fullName, email: p.email, phone: p.phone });

export const getProfile = async () => toUser(await api.get('/auth/me'));
export const updateProfile = async ({ name, phone }) => toUser(await api.put('/auth/me', { fullName: name.trim(), phone: phone.trim() }));
