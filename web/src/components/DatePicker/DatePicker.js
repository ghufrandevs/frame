import { calendarDays, chevronLeft, chevronRight } from '../../assets/icons/index.js';
import { t } from '../../i18n/i18n.js';
import { buildMonthGrid, formatLongDate, formatMonthYear, parseISO } from '../../utils/date.js';

export const renderDatePicker = () => '<div id="cal"></div>';

// Month grid for picking ONE day. The owner decides which days can be picked (from the server):
// getValue() -> selected 'YYYY-MM-DD' or null; isSelectable(iso) -> bool; onSelect(iso) on click;
// onMonthChange(year, month) when the customer pages to another month (month is 0-based).
export function mountDatePicker(el, { getValue, isSelectable, onSelect, onMonthChange }) {
  const today = new Date();
  const first = getValue() ? parseISO(getValue()) : today;
  const view = { year: first.getFullYear(), month: first.getMonth() };
  const isCurrentMonth = () => view.year === today.getFullYear() && view.month === today.getMonth();

  function draw() {
    const selected = getValue();
    const cells = buildMonthGrid(view.year, view.month).map(({ iso, day, outside }) => {
      const enabled = !outside && isSelectable(iso), isSelected = !outside && iso === selected;
      return `<button type="button" class="dt ${enabled ? '' : 'o'} ${isSelected ? 's a z' : ''}" data-date="${iso}" ${enabled ? '' : 'disabled'} aria-pressed="${isSelected}" aria-label="${formatLongDate(iso)}">${day}</button>`;
    }).join('');
    el.innerHTML = `<div style="display:flex;flex-direction:column;gap:12px"><div class="cm"><span>${formatMonthYear(view.year, view.month)}</span><div><button type="button" data-nav="-1" aria-label="${t('booking.previousMonth')}" ${isCurrentMonth() ? 'disabled' : ''}>${chevronLeft}</button><button type="button" data-nav="1" aria-label="${t('booking.nextMonth')}">${chevronRight}</button></div></div><div class="wk">${t('booking.weekdays').map((d) => `<span>${d}</span>`).join('')}</div><div class="dts">${cells}</div><div class="sd">${calendarDays}<span>${selected ? formatLongDate(selected) : t('booking.pickDay')}</span></div></div>`;
  }

  function showMonth(year, month) {
    const d = new Date(year, month, 1);
    view.year = d.getFullYear();
    view.month = d.getMonth();
    draw();
  }

  el.addEventListener('click', (e) => {
    const nav = e.target.closest('[data-nav]'), day = e.target.closest('[data-date]');
    if (nav) {
      showMonth(view.year, view.month + Number(nav.dataset.nav));
      onMonthChange(view.year, view.month);
    } else if (day && !day.disabled) onSelect(day.dataset.date);
  });
  draw();
  return { redraw: draw, showMonth, getView: () => ({ ...view }) };
}
