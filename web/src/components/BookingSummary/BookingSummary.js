import { PHOTOGRAPHER_RATE } from '../../data/config.js';
import { t } from '../../i18n/i18n.js';
import { money } from '../../utils/formatting.js';

const totalRow = (total) => `<div class="tt"><span>${t('booking.total')}</span><span>${money(total)}</span></div>`;

// Price rows. pricing = calculatePricing(); totalOnly renders just the total line (success page).
export function renderBookingSummary({ studio, pricing, photographer, totalOnly = false }) {
  if (totalOnly) return `<div class="pr">${totalRow(pricing.total)}</div>`;
  const h = pricing.hours;
  return `<div class="pr" id="pr">${summaryRows(studio, pricing, photographer, h)}</div>`;
}
const summaryRows = (studio, pricing, photographer, h) =>
  `<div><span>${t('booking.studio')} &nbsp; ${studio.ratePerHour} × ${h} ${t('booking.hoursWord')}</span><span>${money(pricing.studioCost)}</span></div>${photographer ? `<div><span>${t('booking.photographer')} &nbsp; ${PHOTOGRAPHER_RATE} × ${h}</span><span>${money(pricing.photographerCost)}</span></div>` : ''}<div><span>${t('booking.tax')}</span><span>${money(pricing.tax)}</span></div>${totalRow(pricing.total)}`;

export function updateBookingSummary(root, props) {
  root.querySelector('#pr').innerHTML = summaryRows(props.studio, props.pricing, props.photographer, props.pricing.hours);
}
