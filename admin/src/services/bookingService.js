import { api } from './api.js';

// GET /admin/bookings?studioId=&date=&page= -> { items, page, pageSize, totalCount, totalPages }
// 20 per page, newest first. Empty filters are left out of the URL.
export function getBookings({ studioId, date, page = 1 }) {
  const query = new URLSearchParams({ page });
  if (studioId) query.set('studioId', studioId);
  if (date) query.set('date', date);
  return api.get(`/admin/bookings?${query}`);
}

// One booking with invoice, payment, cancellation and the customer's contact details.
export const getBooking = (id) => api.get(`/admin/bookings/${id}`);

// Cancels before the start, refunds the full amount and emails the customer. Returns the updated booking.
export const cancelBooking = (id) => api.post(`/admin/bookings/${id}/cancel`);
