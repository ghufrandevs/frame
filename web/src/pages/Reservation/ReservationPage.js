import { t } from '../../i18n/i18n.js';
import { navigate } from '../../router.js';
import { getStudios } from '../../services/studioService.js';
import { bookingState, selectDate, setStartHour, setEndHour, setPhotographer, setStudio, setStep, calculatePricing, getDayCount, getSessionHours } from '../../state/bookingState.js';
import { money } from '../../utils/formatting.js';
import { Button } from '../../components/Button/Button.js';
import { renderDatePicker, mountDatePicker } from '../../components/DatePicker/DatePicker.js';
import { renderTimeSelector, mountTimeSelector } from '../../components/TimeSelector/TimeSelector.js';
import { renderPhotographerOption, mountPhotographerOption } from '../../components/PhotographerOption/PhotographerOption.js';
import { renderBookingSummary, updateBookingSummary } from '../../components/BookingSummary/BookingSummary.js';
import { renderStudioRotation, mountStudioRotation } from '../../components/StudioRotation/StudioRotation.js';

// One page for every studio: /reservation/:studioId picks the studio from data.
let studios = [], parts = null;
const currentStudio = () => studios.find((s) => s.id === bookingState.get().studioId);

export const ReservationPage = {
  async render({ studioId }) {
    studios = await getStudios();
    const studio = studios.find((s) => s.id === studioId);
    if (!studio) return { redirect: '/' };
    setStudio(studio.id);
    setStep(1);
    const b = bookingState.get();
    return `<section class="pg"><div><h1 id="stt" class="fade"></h1><p class="sub fade" id="std"></p></div><div class="split"><div class="panel"><div><p class="eb">${t('booking.session')}</p><div class="ph1"><h2>${t('booking.reserve')}</h2><span id="rt" class="fade" style="font-size:12px;color:var(--m)"></span></div></div><div class="hr"></div>${renderDatePicker()}${renderTimeSelector()}<p class="note" id="nt"></p>${renderPhotographerOption(b.photographer)}${renderBookingSummary({ studio, pricing: calculatePricing(studio), photographer: b.photographer })}<div style="display:flex;flex-direction:column;gap:12px">${Button({ label: t('booking.continue'), variant: 'cta', id: 'cb' })}<p class="fn">${t('booking.review')}</p></div></div><div class="rt">${renderStudioRotation(studios)}</div></div></section>`;
  },

  mount(root) {
    const b = () => bookingState.get();
    const refresh = () => {
      const studio = currentStudio(), pricing = calculatePricing(studio);
      root.querySelector('#nt').textContent = b().dateEnd
        ? t('booking.durationMulti', { d: getDayCount(), h: getSessionHours() })
        : t('booking.durationSingle', { h: getSessionHours() });
      parts.photographer.setHours(pricing.hours);
      updateBookingSummary(root, { studio, pricing, photographer: b().photographer });
    };
    parts = {
      picker: mountDatePicker(root.querySelector('#cal'), {
        getValue: () => ({ start: b().dateStart, end: b().dateEnd }),
        onSelect: (iso) => { selectDate(iso); parts.picker.redraw(); refresh(); },
      }),
      time: mountTimeSelector(root, {
        getValue: b,
        onStart: (h) => { setStartHour(h); parts.time.refresh(); refresh(); },
        onEnd: (h) => { setEndHour(h); refresh(); },
      }),
      photographer: mountPhotographerOption(root, { onChange: (on) => { setPhotographer(on); refresh(); } }),
      rotation: mountStudioRotation(root, {
        studios, currentId: b().studioId,
        onSelect: (id) => navigate(`/reservation/${id}`, { replace: true }), // replace: studio switches don't pile up history
      }),
      refresh,
    };
    root.querySelector('#cb').onclick = () => navigate('/payment');
    ReservationPage.applyStudio(false);
  },

  // Text that changes with the studio. With animate=true it fades out while the prism turns.
  applyStudio(animate) {
    const root = document.getElementById('app'), studio = currentStudio(), id = studio.id;
    const fades = root.querySelectorAll('.fade');
    fades.forEach((e) => e.classList.add('x'));
    const update = () => {
      root.querySelector('#stt').textContent = studio.name;
      root.querySelector('#std').textContent = studio.description;
      root.querySelector('#rt').innerHTML = `${money(studio.ratePerHour)} ${t('common.perHour')}`;
      parts.rotation.renderThumbs(id);
      fades.forEach((e) => e.classList.remove('x'));
      parts.refresh();
      document.title = `FRAME — ${studio.name}`;
    };
    animate ? setTimeout(update, 380) : update();
  },

  // Called by the router when only :studioId changed (no full re-render, so the prism can animate).
  onParams({ studioId }) {
    if (studioId === bookingState.get().studioId) return;
    if (!studios.some((s) => s.id === studioId)) return navigate('/', { replace: true });
    setStudio(studioId);
    parts.rotation.rotateTo(studioId);
    ReservationPage.applyStudio(true);
  },

  unmount() { parts?.rotation.destroy(); parts = null; },
};
