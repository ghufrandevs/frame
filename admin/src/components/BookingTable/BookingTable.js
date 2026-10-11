import { t } from '../../i18n/i18n.js';
import { navigate } from '../../router.js';
import { money, hourRange, dateLabel, escapeHtml } from '../../utils/format.js';

// rows: admin booking rows { id, bookingNumber, customerName, studioName, date, startHour, endHour, total, status }.
// The whole row opens the booking; the booking number is a real link for the keyboard.
// showDate adds a Date column (the dashboard lists today only, so it hides it).
// On phones each row becomes a card; data-label gives every value its title there.
export function BookingTable(rows, { emptyText = '', showDate = false }) {
  if (!rows.length) return `<div class="bt-empty"><p>${emptyText}</p></div>`;
  const cell = (key, html, extra = '') => `<td data-label="${t(`table.${key}`)}"${extra}>${html}</td>`;
  const body = rows.map((b) => `<tr data-href="/bookings/${b.id}">
    ${cell('number', `<a class="bt-num" href="#/bookings/${b.id}">${escapeHtml(b.bookingNumber)}</a>`)}
    ${cell('customer', escapeHtml(b.customerName))}
    ${cell('studio', escapeHtml(b.studioName))}
    ${showDate ? cell('date', dateLabel(b.date)) : ''}
    ${cell('time', `<span dir="ltr">${hourRange(b.startHour, b.endHour)}</span>`)}
    ${cell('total', money(b.total))}
    ${cell('status', StatusBadge(b.status))}
  </tr>`).join('');
  const head = ['number', 'customer', 'studio', ...(showDate ? ['date'] : []), 'time', 'total', 'status']
    .map((key) => `<th scope="col">${t(`table.${key}`)}</th>`).join('');
  return `<div class="bt-wrap"><table class="bt"><thead><tr>${head}</tr></thead><tbody>${body}</tbody></table></div>`;
}

// Clicking anywhere on a row opens the booking (call once per page, on any element that contains tables).
export function mountBookingTable(container) {
  container.addEventListener('click', (e) => {
    const row = e.target.closest('tr[data-href]');
    if (!row || e.target.closest('a')) return; // the link handles itself
    navigate(row.dataset.href);
  });
}

// Upcoming / InProgress / Completed / Cancelled, coloured by status.
export const StatusBadge = (status) => `<span class="badge badge-${status}">${t(`status.${status}`)}</span>`;
