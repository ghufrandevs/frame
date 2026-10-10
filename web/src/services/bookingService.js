import { api, USE_MOCK } from './api.js';
import { bookings } from '../data/mock/bookings.js';

// GET /studios/{id}/days?month=YYYY-MM -> { month, minDate, maxDate, days: [{ date, hasAvailability }] }
export const getStudioDays = (apiId, month) => api.get(`/studios/${apiId}/days?month=${month}`);

// GET /studios/{id}/availability?date=YYYY-MM-DD -> { studioId, date, slots: [{ hour, status: 'Available' | 'Booked' }] }
export const getAvailability = (apiId, date) => api.get(`/studios/${apiId}/availability?date=${date}`);

// POST /bookings/quote (no login needed) -> { hours, hourlyRate, photographerRatePerHour, photographerFee, subtotal, vatRate, vatAmount, total, ... }
export const getQuote = (request) => api.post('/bookings/quote', request);

// Still on mock data until the payment step is wired.
export const createBooking = async (payload) => {
  if (!USE_MOCK) return api.post('/bookings', payload);
  const booking = { id: `BK-${Date.now()}`, status: 'received', ...payload };
  bookings.push(booking);
  return booking;
};
export const getBookings = async () => (USE_MOCK ? [...bookings] : api.get('/bookings'));
