export const validateCardName = (v) => v.trim().length > 1;
export const validateCardNumber = (v) => v.replace(/\s/g, '').length === 16;
export const validateCVV = (v) => /^\d{3}$/.test(v);
// Expects "MM / YY"; the card must not be expired this month.
export function validateExpiryDate(v) {
  const m = /^(\d\d) \/ (\d\d)$/.exec(v);
  if (!m) return false;
  const now = new Date();
  return +m[1] >= 1 && +m[1] <= 12 && (2000 + +m[2]) * 12 + +m[1] >= now.getFullYear() * 12 + now.getMonth() + 1;
}
