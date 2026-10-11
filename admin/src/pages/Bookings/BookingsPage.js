import { t } from '../../i18n/i18n.js';
import { getBookings } from '../../services/bookingService.js';
import { getStudios, studioName } from '../../services/studioService.js';
import { BookingTable, mountBookingTable } from '../../components/BookingTable/BookingTable.js';
import { errorMessage } from '../../utils/errors.js';
import { escapeHtml, muscatToday } from '../../utils/format.js';

// Kept while the app is open, so "Back to bookings" returns to the same filters and page.
// date: null = first visit (opens on today), '' = all dates, 'YYYY-MM-DD' = one day.
const filters = { studioId: '', date: null, page: 1 };
let requestId = 0;

const countText = (n) => (n === 1 ? t('bookings.countOne') : t('bookings.count').replace('{n}', n));
const isToday = () => filters.date === muscatToday();

function emptyState() {
  const text = isToday() ? t('bookings.emptyToday') : t(filters.date || filters.studioId ? 'bookings.emptyFiltered' : 'bookings.empty');
  const showAll = filters.date ? `<button type="button" class="btn btn-light" data-all-dates>${t('bookings.showAll')}</button>` : '';
  return `<div class="bt-empty"><p>${text}</p>${showAll}</div>`;
}

function renderResults(result) {
  if (!result.items.length) return emptyState();
  const table = BookingTable(result.items, { showDate: true });
  const pager = result.totalPages <= 1 ? '' : `<nav class="pager" aria-label="${t('bookings.pages')}">
      <button type="button" class="btn btn-light" data-page="${result.page - 1}"${result.page <= 1 ? ' disabled' : ''}>${t('bookings.previous')}</button>
      <span>${t('bookings.pageOf').replace('{p}', result.page).replace('{n}', result.totalPages)}</span>
      <button type="button" class="btn btn-light" data-page="${result.page + 1}"${result.page >= result.totalPages ? ' disabled' : ''}>${t('bookings.next')}</button>
    </nav>`;
  return `<p class="muted results-count">${countText(result.totalCount)}</p>${table}${pager}`;
}

// Reloads only the results; a slower old answer never replaces a newer one.
// Highlights the active choice: Today, All dates, or neither when one day is picked.
function updateDateButtons(root) {
  root.querySelector('#todayFilter').setAttribute('aria-pressed', String(isToday()));
  root.querySelector('#allDates').setAttribute('aria-pressed', String(filters.date === ''));
}

async function loadResults(root) {
  const box = root.querySelector('#bookingsResults');
  updateDateButtons(root);
  const id = ++requestId;
  box.setAttribute('aria-busy', 'true');
  try {
    const result = await getBookings(filters);
    if (id === requestId) box.innerHTML = renderResults(result);
  } catch (error) {
    if (id === requestId) box.innerHTML = `<p class="form-error" role="alert">${errorMessage(error)}</p>`;
  } finally {
    if (id === requestId) box.removeAttribute('aria-busy');
  }
}

export const BookingsPage = {
  async render() {
    if (filters.date === null) filters.date = muscatToday();
    const [studios, result] = await Promise.all([getStudios(), getBookings(filters)]);
    const options = studios.map((s) =>
      `<option value="${s.id}"${String(s.id) === filters.studioId ? ' selected' : ''}>${escapeHtml(studioName(s))}</option>`).join('');
    return `<section class="page">
      <header class="page-head">
        <h1>${t('bookings.title')}</h1>
        <p class="page-sub">${t('bookings.subtitle')}</p>
      </header>
      <form class="filters" id="bookingFilters" role="search">
        <div class="field">
          <label for="filterStudio">${t('table.studio')}</label>
          <select id="filterStudio"><option value="">${t('bookings.allStudios')}</option>${options}</select>
        </div>
        <div class="field">
          <label for="filterDate">${t('table.date')}</label>
          <input id="filterDate" type="date" value="${filters.date}">
        </div>
        <div class="seg" role="group" aria-label="${t('table.date')}">
          <button type="button" class="btn btn-light btn-tall" id="todayFilter" aria-pressed="${isToday()}">${t('bookings.today')}</button>
          <button type="button" class="btn btn-light btn-tall" id="allDates" aria-pressed="${filters.date === ''}">${t('bookings.allDates')}</button>
        </div>
      </form>
      <section class="panel panel-list" id="bookingsResults" aria-live="polite">${renderResults(result)}</section>
    </section>`;
  },

  mount(root) {
    const studio = root.querySelector('#filterStudio');
    const date = root.querySelector('#filterDate');
    const results = root.querySelector('#bookingsResults');
    const apply = () => {
      Object.assign(filters, { studioId: studio.value, date: date.value, page: 1 });
      loadResults(root);
    };
    root.querySelector('#bookingFilters').onsubmit = (e) => e.preventDefault();
    studio.onchange = apply;
    date.onchange = apply;
    const showAllDates = () => { date.value = ''; apply(); };
    root.querySelector('#todayFilter').onclick = () => { date.value = muscatToday(); apply(); };
    root.querySelector('#allDates').onclick = showAllDates;
    mountBookingTable(results);
    results.addEventListener('click', (e) => {
      if (e.target.closest('[data-all-dates]')) return showAllDates();
      const button = e.target.closest('[data-page]');
      if (!button) return;
      filters.page = Number(button.dataset.page);
      loadResults(root);
      results.scrollIntoView({ behavior: 'smooth', block: 'start' });
    });
  },
};
