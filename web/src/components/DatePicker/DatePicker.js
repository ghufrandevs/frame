import { calendarDays, chevronLeft, chevronRight } from '../../assets/icons/index.js';
import { CALENDAR_START, MIN_DATE } from '../../data/config.js';
import { t } from '../../i18n/i18n.js';
import { buildMonthGrid, formatLongDate, formatMonthYear, daysBetween } from '../../utils/date.js';

export const renderDatePicker = () => '<div id="cal"></div>';

// Month grid with single-day and consecutive-range selection.
// getValue() -> { start, end|null }; onSelect(iso) is called on click (the owner updates booking state, then calls redraw()).
export function mountDatePicker(el, { getValue, onSelect }) {
  const view = { year: CALENDAR_START.year, month: CALENDAR_START.month };
  const [minY, minM] = MIN_DATE.split('-').map(Number);

  function draw() {
    const { start, end } = getValue();
    const cells = buildMonthGrid(view.year, view.month).map(({ iso, day, outside }) => {
      const disabled = iso < MIN_DATE, isStart = iso === start, isEnd = end ? iso === end : isStart;
      const mid = end && iso > start && iso < end, selected = isStart || isEnd;
      return `<button type="button" class="dt ${outside || disabled ? 'o' : ''} ${selected ? 's' : ''} ${isStart ? 'a' : ''} ${isEnd ? 'z' : ''} ${mid ? 'n' : ''}" data-date="${iso}" ${disabled ? 'disabled' : ''} aria-pressed="${!!(selected || mid)}" aria-label="${formatLongDate(iso)}">${day}</button>`;
    }).join('');
    const days = end ? ` · ${t('common.days', { d: daysBetween(start, end) + 1 })}` : '';
    el.innerHTML = `<div style="display:flex;flex-direction:column;gap:12px"><div class="cm"><span>${formatMonthYear(view.year, view.month)}</span><div><button type="button" data-nav="-1" aria-label="Previous month" ${view.year === minY && view.month <= minM - 1 ? 'disabled' : ''}>${chevronLeft}</button><button type="button" data-nav="1" aria-label="Next month">${chevronRight}</button></div></div><div class="wk">${t('booking.weekdays').map((d) => `<span>${d}</span>`).join('')}</div><div class="dts">${cells}</div><div class="sd">${calendarDays}<span>${formatLongDate(start)}${end ? ' → ' + formatLongDate(end) : ''}${days}</span></div></div>`;
  }

  el.addEventListener('click', (e) => {
    const nav = e.target.closest('[data-nav]'), day = e.target.closest('[data-date]');
    if (nav) {
      const d = new Date(view.year, view.month + Number(nav.dataset.nav), 1);
      view.year = d.getFullYear(); view.month = d.getMonth(); draw();
    } else if (day) onSelect(day.dataset.date);
  });
  draw();
  return { redraw: draw };
}
