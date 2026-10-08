import { CURRENCY } from '../data/config.js';

export const roundMoney = (x) => Math.round(x * 100) / 100;
// Currency label + amount, styled by .cur
export const money = (value) => `<span class="cur">${CURRENCY}</span>${value}`;

export const formatCardNumber = (raw) => raw.replace(/\D/g, '').slice(0, 16).replace(/(.{4})(?=.)/g, '$1 ');
export function formatExpiry(raw, deleting = false) {
  const v = raw.replace(/\D/g, '').slice(0, 4);
  if (v.length >= 3) return `${v.slice(0, 2)} / ${v.slice(2)}`;
  return v.length === 2 && !deleting ? `${v} / ` : v;
}
export const formatCVV = (raw) => raw.replace(/\D/g, '').slice(0, 3);
