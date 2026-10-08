import { chevronDown } from '../../assets/icons/index.js';
import { OPEN_HOUR, CLOSE_HOUR } from '../../data/config.js';
import { t } from '../../i18n/i18n.js';

const field = (id, label) => `<div class="fld"><label for="${id}">${label}</label><select id="${id}"></select>${chevronDown}</div>`;
export const renderTimeSelector = () => `<div class="tm">${field('st', t('booking.startTime'))}${field('en', t('booking.endTime'))}</div>`;

const options = (from, to, selected) => Array.from({ length: to - from + 1 }, (_, i) => from + i)
  .map((h) => `<option value="${h}" ${h === selected ? 'selected' : ''}>${h}:00</option>`).join('');

// getValue() -> { startHour, endHour }. Call the returned refresh() after the state changes.
export function mountTimeSelector(root, { getValue, onStart, onEnd }) {
  const start = root.querySelector('#st'), end = root.querySelector('#en');
  const refresh = () => {
    const { startHour, endHour } = getValue();
    start.innerHTML = options(OPEN_HOUR, CLOSE_HOUR - 1, startHour);
    end.innerHTML = options(startHour + 1, CLOSE_HOUR, endHour);
  };
  start.onchange = () => onStart(+start.value);
  end.onchange = () => onEnd(+end.value);
  refresh();
  return { refresh };
}
