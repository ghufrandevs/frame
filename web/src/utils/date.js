import { getLanguage } from '../i18n/i18n.js';

const locale = () => (getLanguage() === 'ar' ? 'ar-u-nu-latn' : 'en-GB');
const pad = (n) => String(n).padStart(2, '0');

export const toISO = (d) => `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}`;
export const parseISO = (s) => { const [y, m, d] = s.split('-'); return new Date(+y, +m - 1, +d); };
export const daysBetween = (a, b) => Math.round((parseISO(b) - parseISO(a)) / 864e5);
export const isDateRangeValid = (start, end) => !!start && (end == null || end >= start);

// "Thursday, 15 October 2026" (weekday optional)
export function formatLongDate(iso, withWeekday = true) {
  const d = parseISO(iso);
  const weekday = new Intl.DateTimeFormat(locale(), { weekday: 'long' }).format(d);
  const month = new Intl.DateTimeFormat(locale(), { month: 'long' }).format(d);
  return `${withWeekday ? weekday + ', ' : ''}${d.getDate()} ${month} ${d.getFullYear()}`;
}
export const formatMonthYear = (year, month) =>
  new Intl.DateTimeFormat(locale(), { month: 'long', year: 'numeric' }).format(new Date(year, month, 1));

// Monday-first grid of whole weeks covering the month. Cells outside the month have outside=true.
export function buildMonthGrid(year, month) {
  const lead = (new Date(year, month, 1).getDay() + 6) % 7;
  const days = new Date(year, month + 1, 0).getDate();
  const total = Math.ceil((lead + days) / 7) * 7;
  return Array.from({ length: total }, (_, i) => {
    const d = new Date(year, month, 1 - lead + i);
    return { iso: toISO(d), day: d.getDate(), outside: d.getMonth() !== month };
  });
}
