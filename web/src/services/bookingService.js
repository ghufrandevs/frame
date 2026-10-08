import { api, USE_MOCK } from './api.js';
import { bookings } from '../data/mock/bookings.js';

// Expected API: POST /bookings   GET /bookings   POST /bookings/availability
// Booking payload comes from bookingState.toBookingPayload().
export const createBooking = async (payload) => {
  if (!USE_MOCK) return api.post('/bookings', payload);
  const booking = { id: `BK-${Date.now()}`, status: 'received', ...payload };
  bookings.push(booking);
  return booking;
};
export const getBookings = async () => (USE_MOCK ? [...bookings] : api.get('/bookings'));
// { studioId, dateStart, dateEnd, startHour, endHour } -> { available: boolean }
export const getAvailability = async (query) => (USE_MOCK ? { available: true } : api.post('/bookings/availability', query));
