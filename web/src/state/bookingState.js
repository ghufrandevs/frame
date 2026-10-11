import { createStore } from './store.js';
import { DEFAULT_BOOKING } from '../data/config.js';

// Single source of truth for the booking flow (studio, date, hours, photographer, step).
// Prices are never calculated in the browser: `quote` holds the server's answer for the current selection,
// and every change to the selection clears it so an old price can never be shown or paid.
export const bookingState = createStore(DEFAULT_BOOKING);

export const getSessionHours = (b = bookingState.get()) => b.endHour - b.startHour;

export const selectDate = (date) => bookingState.set({ date, quote: null });
export const setHours = (startHour, endHour) => bookingState.set({ startHour, endHour, quote: null });
export const setPhotographer = (on) => bookingState.set({ photographer: on, quote: null });
export const setStep = (step) => bookingState.set({ step });
export const setQuote = (quote) => bookingState.set({ quote });
// After a paid booking: keep the server's answer for the success page and forget the paid price,
// so going back can never pay the same quote twice.
export const completeBooking = (booking) => bookingState.set({ lastBooking: booking, quote: null, photographer: false });
export function setStudio(studioId) {
  if (bookingState.get().studioId !== studioId) bookingState.set({ studioId, quote: null });
}

// Body of POST /bookings/quote. POST /bookings sends the same fields plus paymentToken.
export const toQuoteRequest = (studio, b = bookingState.get()) => ({
  studioId: studio.apiId,
  date: b.date,
  startHour: b.startHour,
  endHour: b.endHour,
  withPhotographer: b.photographer,
});
