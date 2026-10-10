import { t } from '../../i18n/i18n.js';
import { money } from '../../utils/formatting.js';

// Price rows, straight from the server's quote (POST /bookings/quote). Nothing is calculated here.
// quote = { hours, hourlyRate, photographerRatePerHour, photographerFee, subtotal, vatRate, vatAmount, total }
const totalRow = (total) => `<div class="tt"><span>${t('booking.total')}</span><span>${money(total)}</span></div>`;

function summaryRows(q) {
  const studio = `<div><span>${t('booking.studio')} &nbsp; ${q.hourlyRate} × ${q.hours} ${t('booking.hoursWord')}</span><span>${money(q.subtotal - q.photographerFee)}</span></div>`;
  const photographer = q.photographerFee > 0 ? `<div><span>${t('booking.photographer')} &nbsp; ${q.photographerRatePerHour} × ${q.hours}</span><span>${money(q.photographerFee)}</span></div>` : '';
  const tax = `<div><span>${t('booking.tax', { p: q.vatRate * 100 })}</span><span>${money(q.vatAmount)}</span></div>`;
  return studio + photographer + tax + totalRow(q.total);
}

// With no quote yet the box stays empty.
export const renderBookingSummary = ({ quote }) => `<div class="pr" id="pr">${quote ? summaryRows(quote) : ''}</div>`;

// Just the total line, e.g. the invoice total on the success page.
export const renderTotal = (total) => `<div class="pr">${totalRow(total)}</div>`;

export function updateBookingSummary(root, quote) {
  root.querySelector('#pr').innerHTML = quote ? summaryRows(quote) : '';
}
