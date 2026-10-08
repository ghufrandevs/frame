import { PHOTOGRAPHER_RATE } from '../../data/config.js';
import { t } from '../../i18n/i18n.js';
import { money } from '../../utils/formatting.js';

export const renderPhotographerOption = (checked) => `<label class="opt"><input type="checkbox" id="ph" ${checked ? 'checked' : ''}><span class="i"><b>${t('booking.photographer')}</b><small id="phs"></small></span><span class="p" dir="ltr">${money(PHOTOGRAPHER_RATE)} ${t('common.perHour')}</span></label>`;

export function mountPhotographerOption(root, { onChange }) {
  root.querySelector('#ph').onchange = (e) => onChange(e.target.checked);
  return { setHours: (h) => (root.querySelector('#phs').textContent = t('booking.photographerNote', { h })) };
}
