import { t } from '../../i18n/i18n.js';
import { money } from '../../utils/formatting.js';

// The rate is filled from the server's quote (photographerRatePerHour), never hard-coded here.
export const renderPhotographerOption = (checked) => `<label class="opt"><input type="checkbox" id="ph" ${checked ? 'checked' : ''}><span class="i"><b>${t('booking.photographer')}</b><small id="phs"></small></span><span class="p" dir="ltr" id="phr"></span></label>`;

export function mountPhotographerOption(root, { onChange }) {
  root.querySelector('#ph').onchange = (e) => onChange(e.target.checked);
  return {
    setHours: (h) => (root.querySelector('#phs').textContent = h ? t('booking.photographerNote', { h }) : ''),
    setRate: (rate) => (root.querySelector('#phr').innerHTML = `${money(rate)} ${t('common.perHour')}`),
  };
}
