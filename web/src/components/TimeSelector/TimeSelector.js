import { chevronDown } from '../../assets/icons/index.js';
import { t } from '../../i18n/i18n.js';

const field = (id, label) => `<div class="fld"><label for="${id}">${label}</label><select id="${id}"></select>${chevronDown}</div>`;
export const renderTimeSelector = () => `<div class="tm">${field('st', t('booking.startTime'))}${field('en', t('booking.endTime'))}</div>`;

const options = (hours, selected) => hours.map((h) => `<option value="${h}" ${h === selected ? 'selected' : ''}>${h}:00</option>`).join('');

// Two selects filled with the hours the server says are free. Returns { show(startHours, endHours, start, end) }.
// With no free hours both selects are disabled and empty.
export function mountTimeSelector(root, { onStart, onEnd }) {
  const start = root.querySelector('#st'), end = root.querySelector('#en');
  start.onchange = () => onStart(Number(start.value));
  end.onchange = () => onEnd(Number(end.value));
  return {
    show(startHours, endHours, selectedStart, selectedEnd) {
      start.innerHTML = options(startHours, selectedStart);
      end.innerHTML = options(endHours, selectedEnd);
      start.disabled = startHours.length === 0;
      end.disabled = endHours.length === 0;
    },
  };
}
