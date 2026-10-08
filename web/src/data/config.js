// Business constants. Change pricing rules, opening hours and prototype dates here.
export const CURRENCY = 'OMR';
export const TAX_RATE = 0.05;
export const PHOTOGRAPHER_RATE = 30; // per hour
export const OPEN_HOUR = 8;
export const CLOSE_HOUR = 22;
// Prototype dates (the design is set in October 2026). Replace with new Date() once a backend exists.
export const MIN_DATE = '2026-10-08';
export const CALENDAR_START = { year: 2026, month: 9 }; // month is 0-based
export const DEFAULT_BOOKING = {
  studioId: 'daylight-loft',
  dateStart: '2026-10-15',
  dateEnd: null, // null = single day, otherwise last day of a consecutive range
  startHour: 10,
  endHour: 14,
  photographer: false,
  paymentMethod: 'card', // 'card' | 'apple'
  step: 1, // 1 Reservation, 2 Payment, 3 Booking complete
};
