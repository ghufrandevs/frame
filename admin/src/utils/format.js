import { CURRENCY } from '../data/config.js';
import { getLanguage } from '../i18n/i18n.js';

// Rials always with 3 decimals: OMR 1,000.000
export const money = (value) => `${CURRENCY} ${Number(value).toLocaleString('en-US', { minimumFractionDigits: 3, maximumFractionDigits: 3 })}`;

// 0.42 -> 42%
export const percent = (ratio) => `${Math.round(Number(ratio) * 100)}%`;

// 9, 12 -> 09:00–12:00
const hour = (h) => `${String(h).padStart(2, '0')}:00`;
export const hourRange = (start, end) => `${hour(start)}–${hour(end)}`;

// '2026-10' -> October 2026 / أكتوبر 2026
export function monthLabel(month) {
  const [year, m] = month.split('-').map(Number);
  const locale = getLanguage() === 'ar' ? 'ar-OM-u-nu-latn' : 'en-GB';
  return new Date(year, m - 1, 1).toLocaleDateString(locale, { month: 'long', year: 'numeric' });
}

// Text from the server is escaped before it goes into HTML: a name like "<b>" shows as text.
export const escapeHtml = (value) => String(value ?? '').replace(/[&<>"']/g, (ch) => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' })[ch]);
