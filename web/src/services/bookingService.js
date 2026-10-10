import { api } from './api.js';

// GET /studios/{id}/days?month=YYYY-MM -> { month, minDate, maxDate, days: [{ date, hasAvailability }] }
export const getStudioDays = (apiId, month) => api.get(`/studios/${apiId}/days?month=${month}`);

// GET /studios/{id}/availability?date=YYYY-MM-DD -> { studioId, date, slots: [{ hour, status: 'Available' | 'Booked' }] }
export const getAvailability = (apiId, date) => api.get(`/studios/${apiId}/availability?date=${date}`);

// POST /bookings/quote (no login needed) -> { hours, hourlyRate, photographerRatePerHour, photographerFee, subtotal, vatRate, vatAmount, total, ... }
export const getQuote = (request) => api.post('/bookings/quote', request);

// POST /bookings { ...quote request, paymentToken } -> 201 BookingResponse
// 402 PAYMENT_DECLINED | INSUFFICIENT_FUNDS | PAYMENT_FAILED (nothing charged), 409 SLOT_TAKEN (nothing charged)
export const createBooking = (request) => api.post('/bookings', request);

// GET /bookings/my -> [{ id, bookingNumber, studioName, date, startHour, endHour, total, status }]
// status: 'Upcoming' | 'InProgress' | 'Completed' | 'Cancelled'
export const getMyBookings = () => api.get('/bookings/my');

// GET /bookings/{id} -> BookingResponse = { id, bookingNumber, status, studio: { id, name }, date, startHour, endHour, invoice, payment, cancellation }
export const getBooking = (id) => api.get(`/bookings/${id}`);
