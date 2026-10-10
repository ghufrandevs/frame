// App-wide settings. Prices, tax and opening hours are NOT here: they come from the server.
export const CURRENCY = 'OMR';

// Backend. Every request goes to API_BASE_URL + path (e.g. /studios).
export const API_BASE_URL = 'http://localhost:8080/api';
// Services still on src/data/mock read this; each one drops it once it is wired to the API.
export const USE_MOCK = true;

// Starting state of the booking flow. Date and hours are picked from the studio's real availability.
export const DEFAULT_BOOKING = {
  studioId: 'daylight-loft',
  date: null, // 'YYYY-MM-DD', one day per booking
  startHour: null,
  endHour: null,
  photographer: false,
  paymentMethod: 'card', // 'card' | 'apple'
  step: 1, // 1 Reservation, 2 Payment, 3 Booking complete
  quote: null, // last price from POST /bookings/quote for exactly the fields above
};
