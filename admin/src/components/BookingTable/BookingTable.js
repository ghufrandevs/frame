import { t } from '../../i18n/i18n.js';
import { money, hourRange, escapeHtml } from '../../utils/format.js';

// rows: admin booking rows { id, bookingNumber, customerName, studioName, date, startHour, endHour, total, status }.
// Reused later by the Bookings page.
export function BookingTable(rows, emptyText) {
  if (!rows.length) return `<p class="bt-empty">${emptyText}</p>`;
  const body = rows.map((b) => `<tr>
    <td class="bt-num">${escapeHtml(b.bookingNumber)}</td>
    <td>${escapeHtml(b.customerName)}</td>
    <td>${escapeHtml(b.studioName)}</td>
    <td dir="ltr">${hourRange(b.startHour, b.endHour)}</td>
    <td>${money(b.total)}</td>
    <td><span class="badge badge-${b.status}">${t(`status.${b.status}`)}</span></td>
  </tr>`).join('');
  return `<div class="bt-wrap"><table class="bt">
    <thead><tr>
      <th scope="col">${t('table.number')}</th><th scope="col">${t('table.customer')}</th><th scope="col">${t('table.studio')}</th>
      <th scope="col">${t('table.time')}</th><th scope="col">${t('table.total')}</th><th scope="col">${t('table.status')}</th>
    </tr></thead>
    <tbody>${body}</tbody>
  </table></div>`;
}
