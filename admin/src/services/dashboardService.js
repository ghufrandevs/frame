import { api } from './api.js';

// GET /admin/dashboard ->
// { month: '2026-10', netRevenue, bookingsCount, occupancyNext7Days (0..1),
//   revenueByStudio: [{ studioId, name, revenue }], today: [booking rows] }
export const getDashboard = () => api.get('/admin/dashboard');
