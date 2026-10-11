import { cardBrand } from '../services/paymentService.js';

// Same minimum as the server's name rule (3 letters).
export const validateCardName = (v) => v.trim().length >= 3;

// 16 digits, a real card number (Luhn check) and a brand we accept (Visa or Mastercard).
export function validateCardNumber(v) {
  const digits = v.replace(/\s/g, '');
  return /^\d{16}$/.test(digits) && passesLuhn(digits) && cardBrand(digits) !== null;
}

export const validateCVV = (v) => /^\d{3}$/.test(v);

// Expects "MM / YY"; the card must not be expired this month.
export function validateExpiryDate(v) {
  const m = /^(\d\d) \/ (\d\d)$/.exec(v);
  if (!m) return false;
  const now = new Date();
  return +m[1] >= 1 && +m[1] <= 12 && (2000 + +m[2]) * 12 + +m[1] >= now.getFullYear() * 12 + now.getMonth() + 1;
}

// Luhn checksum: catches mistyped card numbers before anything is sent.
function passesLuhn(digits) {
  let sum = 0;
  for (let i = 0; i < digits.length; i++) {
    let d = Number(digits[digits.length - 1 - i]);
    if (i % 2 === 1) { d *= 2; if (d > 9) d -= 9; }
    sum += d;
  }
  return sum % 10 === 0;
}
