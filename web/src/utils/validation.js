import { MIN_DATE } from '../data/config.js';

export const validateCardName = (v) => v.trim().length > 1;
export const validateCardNumber = (v) => v.replace(/\s/g, '').length === 16;
export const validateCVV = (v) => /^\d{3}$/.test(v);
// Expects "MM / YY"; the card must not be expired relative to MIN_DATE (prototype "today").
export function validateExpiryDate(v) {
  const m = /^(\d\d) \/ (\d\d)$/.exec(v);
  if (!m) return false;
  const [year, month] = MIN_DATE.split('-').map(Number);
  return +m[1] >= 1 && +m[1] <= 12 && (2000 + +m[2]) * 12 + +m[1] >= year * 12 + month;
}
