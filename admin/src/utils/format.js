import { CURRENCY } from '../data/config.js';
import { getLanguage } from '../i18n/i18n.js';

// Arabic text with Western digits, like the rest of the app.
const locale = () => (getLanguage() === 'ar' ? 'ar-OM-u-nu-latn' : 'en-GB');

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
  return new Date(year, m - 1, 1).toLocaleDateString(locale(), { month: 'long', year: 'numeric' });
}

// '2026-10-15' -> Thu, 15 Oct 2026. Built from the parts so the day never shifts with the time zone.
export function dateLabel(date) {
  const [year, m, d] = date.split('-').map(Number);
  return new Date(year, m - 1, d).toLocaleDateString(locale(), { weekday: 'short', day: 'numeric', month: 'short', year: 'numeric' });
}

// '2026-10-11T18:20:00+04:00' -> 11 Oct 2026, 18:20 in Muscat time (the studio's time, wherever the admin is).
export const dateTimeLabel = (iso) => new Date(iso).toLocaleString(locale(), {
  day: 'numeric', month: 'short', year: 'numeric', hour: '2-digit', minute: '2-digit', hour12: false, timeZone: 'Asia/Muscat',
});

// Today's date in Muscat as 'YYYY-MM-DD' (the studio's day, wherever the admin is).
export const muscatToday = () => new Date().toLocaleDateString('en-CA', { timeZone: 'Asia/Muscat' });

// Text from the server is escaped before it goes into HTML: a name like "<b>" shows as text.
export const escapeHtml = (value) => String(value ?? '').replace(/[&<>"']/g, (ch) => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' })[ch]);
