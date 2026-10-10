import { t } from '../../i18n/i18n.js';
import { navigate } from '../../router.js';
import { getStudios } from '../../services/studioService.js';
import { getStudioDays, getAvailability, getQuote } from '../../services/bookingService.js';
import { bookingState, selectDate, setHours, setPhotographer, setStudio, setStep, setQuote, getSessionHours, toQuoteRequest } from '../../state/bookingState.js';
import { money } from '../../utils/formatting.js';
import { freeStartHours, endHoursFor, pickHours } from '../../utils/schedule.js';
import { errorMessage } from '../../utils/errors.js';
import { Button } from '../../components/Button/Button.js';
import { renderDatePicker, mountDatePicker } from '../../components/DatePicker/DatePicker.js';
import { renderTimeSelector, mountTimeSelector } from '../../components/TimeSelector/TimeSelector.js';
import { renderPhotographerOption, mountPhotographerOption } from '../../components/PhotographerOption/PhotographerOption.js';
import { renderBookingSummary, updateBookingSummary } from '../../components/BookingSummary/BookingSummary.js';
import { renderStudioRotation, mountStudioRotation } from '../../components/StudioRotation/StudioRotation.js';

// One page for every studio: /reservation/:studioId picks the studio from data.
// Everything the customer can pick comes from the server: bookable days (GET .../days),
// free hours of the chosen day (GET .../availability) and the price (POST /bookings/quote).
let studios = [], parts = null, root = null;
let days = new Map();   // 'YYYY-MM-DD' -> hasAvailability, for the month on screen
let slots = [];         // free/booked hours of the selected day
let request = 0;        // increases with every load; an answer for an older request is ignored
const currentStudio = () => studios.find((s) => s.id === bookingState.get().studioId);
const monthKey = (year, month) => `${year}-${String(month + 1).padStart(2, '0')}`;

// ===== Loading from the server =====

// Bookable days of a month. A month outside the booking window simply has none.
async function loadMonth(year, month) {
  try {
    const result = await getStudioDays(currentStudio().apiId, monthKey(year, month));
    days = new Map(result.days.map((d) => [d.date, d.hasAvailability]));
  } catch {
    days = new Map();
  }
}

// Shows the month of the selected day, or the first month with a free day (this month or the next).
async function loadCalendar(ticket) {
  const selected = bookingState.get().date;
  const view = selected ? new Date(`${selected}T00:00`) : new Date();
  await loadMonth(view.getFullYear(), view.getMonth());
  if (ticket !== request) return false;

  if (!selected || !days.get(selected)) {
    let firstFree = [...days].find(([, free]) => free)?.[0];
    if (!firstFree) {
      await loadMonth(view.getFullYear(), view.getMonth() + 1);
      if (ticket !== request) return false;
      firstFree = [...days].find(([, free]) => free)?.[0];
      parts.picker.showMonth(view.getFullYear(), view.getMonth() + 1);
    }
    selectDate(firstFree ?? null);
  }
  parts.picker.redraw();
  return true;
}

// Free hours of the selected day; keeps the customer's hours when they are still free.
async function loadDay(ticket) {
  const { date, startHour, endHour } = bookingState.get();
  slots = date ? (await getAvailability(currentStudio().apiId, date)).slots : [];
  if (ticket !== request) return false;
  const hours = pickHours(slots, startHour, endHour);
  setHours(hours.startHour, hours.endHour);
  return true;
}

// Asks the server for the price of exactly what is selected now.
async function loadQuote(ticket) {
  const b = bookingState.get();
  if (b.startHour == null) return showQuote(null, t('booking.noHours'));
  const quote = await getQuote(toQuoteRequest(currentStudio()));
  if (ticket !== request) return;
  setQuote(quote);
  showQuote(quote);
}

// Runs the loads that a change needs, in order; any failure is shown under the hours.
async function reload({ calendar = false, day = false } = {}) {
  const ticket = ++request;
  showQuote(null);
  try {
    if (calendar && !(await loadCalendar(ticket))) return;
    if ((calendar || day) && !(await loadDay(ticket))) return;
    showTimes();
    await loadQuote(ticket);
  } catch (error) {
    if (ticket === request) showQuote(null, errorMessage(error));
  }
}

// ===== Showing state on the page =====

function showTimes() {
  const { startHour, endHour } = bookingState.get();
  parts.time.show(freeStartHours(slots), startHour == null ? [] : endHoursFor(slots, startHour), startHour, endHour);
}

// note: a message instead of the session length (no free hours, or an error).
function showQuote(quote, note = '') {
  const hours = getSessionHours();
  root.querySelector('#nt').textContent = note || (quote ? t('booking.durationSingle', { h: hours }) : '');
  parts.photographer.setHours(quote ? hours : 0);
  if (quote) parts.photographer.setRate(quote.photographerRatePerHour);
  updateBookingSummary(root, quote);
  root.querySelector('#cb').disabled = !quote;
}

export const ReservationPage = {
  async render({ studioId }) {
    studios = await getStudios();
    const studio = studios.find((s) => s.id === studioId);
    if (!studio) return { redirect: '/' };
    setStudio(studio.id);
    setStep(1);
    const b = bookingState.get();
    return `<section class="pg"><div><h1 id="stt" class="fade"></h1><p class="sub fade" id="std"></p></div><div class="split"><div class="panel"><div><p class="eb">${t('booking.session')}</p><div class="ph1"><h2>${t('booking.reserve')}</h2><span id="rt" class="fade" style="font-size:12px;color:var(--m)"></span></div></div><div class="hr"></div>${renderDatePicker()}${renderTimeSelector()}<p class="note" id="nt"></p>${renderPhotographerOption(b.photographer)}${renderBookingSummary({ quote: null })}<div style="display:flex;flex-direction:column;gap:12px">${Button({ label: t('booking.continue'), variant: 'cta', id: 'cb' })}<p class="fn">${t('booking.review')}</p></div></div><div class="rt">${renderStudioRotation(studios)}</div></div></section>`;
  },

  mount(pageRoot) {
    root = pageRoot;
    const b = () => bookingState.get();
    parts = {
      picker: mountDatePicker(root.querySelector('#cal'), {
        getValue: () => b().date,
        isSelectable: (iso) => days.get(iso) === true,
        onSelect: (iso) => { selectDate(iso); parts.picker.redraw(); reload({ day: true }); },
        onMonthChange: async (year, month) => { await loadMonth(year, month); parts.picker.redraw(); },
      }),
      time: mountTimeSelector(root, {
        onStart: (h) => { setHours(h, endHoursFor(slots, h)[0]); showTimes(); reload(); },
        onEnd: (h) => { setHours(b().startHour, h); reload(); },
      }),
      photographer: mountPhotographerOption(root, { onChange: (on) => { setPhotographer(on); reload(); } }),
      rotation: mountStudioRotation(root, {
        studios, currentId: b().studioId,
        onSelect: (id) => navigate(`/reservation/${id}`, { replace: true }), // replace: studio switches don't pile up history
      }),
    };
    root.querySelector('#cb').onclick = () => b().quote && navigate('/payment');
    ReservationPage.applyStudio(false);
    reload({ calendar: true });
  },

  // Text that changes with the studio. With animate=true it fades out while the prism turns.
  applyStudio(animate) {
    const studio = currentStudio();
    const fades = root.querySelectorAll('.fade');
    fades.forEach((e) => e.classList.add('x'));
    const update = () => {
      root.querySelector('#stt').textContent = studio.name;
      root.querySelector('#std').textContent = studio.description;
      root.querySelector('#rt').innerHTML = `${money(studio.ratePerHour)} ${t('common.perHour')}`;
      parts.rotation.renderThumbs(studio.id);
      fades.forEach((e) => e.classList.remove('x'));
      document.title = `FRAME — ${studio.name}`;
    };
    animate ? setTimeout(update, 380) : update();
  },

  // Called by the router when only :studioId changed (no full re-render, so the prism can animate).
  // Another studio has its own bookings, so its calendar and hours are loaded again.
  onParams({ studioId }) {
    if (studioId === bookingState.get().studioId) return;
    if (!studios.some((s) => s.id === studioId)) return navigate('/', { replace: true });
    setStudio(studioId);
    parts.rotation.rotateTo(studioId);
    ReservationPage.applyStudio(true);
    reload({ calendar: true });
  },

  unmount() { parts?.rotation.destroy(); parts = null; request++; },
};
