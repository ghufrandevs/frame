import { createStore } from './store.js';
import { DEFAULT_BOOKING, CLOSE_HOUR, PHOTOGRAPHER_RATE, TAX_RATE } from '../data/config.js';
import { daysBetween } from '../utils/date.js';
import { roundMoney } from '../utils/formatting.js';

// Single source of truth for the booking flow (studio, dates, time, photographer, payment method, step).
export const bookingState = createStore(DEFAULT_BOOKING);

export const getDayCount = (b = bookingState.get()) => (b.dateEnd ? daysBetween(b.dateStart, b.dateEnd) + 1 : 1);
export const getSessionHours = (b = bookingState.get()) => b.endHour - b.startHour;

// First click = one day; a later date extends to a range; clicking again starts over.
export function selectDate(iso) {
  const { dateStart, dateEnd } = bookingState.get();
  if (dateStart && !dateEnd && iso > dateStart) bookingState.set({ dateEnd: iso });
  else if (dateStart && !dateEnd && iso === dateStart) return;
  else bookingState.set({ dateStart: iso, dateEnd: null });
}
export function setStartHour(hour) {
  const { endHour } = bookingState.get();
  bookingState.set({ startHour: hour, endHour: endHour <= hour ? Math.min(hour + 1, CLOSE_HOUR) : endHour });
}
export const setEndHour = (hour) => bookingState.set({ endHour: hour });
export const setPhotographer = (on) => bookingState.set({ photographer: on });
export const setPaymentMethod = (method) => bookingState.set({ paymentMethod: method });
export const setStudio = (studioId) => bookingState.set({ studioId });
export const setStep = (step) => bookingState.set({ step });

export function calculatePricing(studio, b = bookingState.get()) {
  const hours = getSessionHours(b) * getDayCount(b);
  const studioCost = studio.ratePerHour * hours;
  const photographerCost = b.photographer ? PHOTOGRAPHER_RATE * hours : 0;
  const subtotal = studioCost + photographerCost;
  const tax = roundMoney(subtotal * TAX_RATE);
  return { hours, studioCost, photographerCost, tax, total: roundMoney(subtotal + tax) };
}

// Shape sent to bookingService.createBooking().
export function toBookingPayload(studio, b = bookingState.get()) {
  return { studioId: studio.id, dateStart: b.dateStart, dateEnd: b.dateEnd, startHour: b.startHour, endHour: b.endHour,
    photographer: b.photographer, paymentMethod: b.paymentMethod, total: calculatePricing(studio, b).total };
}
