import { t } from '../../i18n/i18n.js';
import { getStudioById } from '../../services/studioService.js';
import { bookingState, setStep, calculatePricing, getDayCount, getSessionHours } from '../../state/bookingState.js';
import { userState } from '../../state/userState.js';
import { formatLongDate } from '../../utils/date.js';
import { checkBig } from '../../assets/icons/index.js';
import { Button } from '../../components/Button/Button.js';
import { BookingProgress, mountBookingProgress } from '../../components/BookingProgress/BookingProgress.js';
import { renderBookingSummary } from '../../components/BookingSummary/BookingSummary.js';
import { DetailList } from '../../components/DetailList/DetailList.js';

export const BookingSuccessPage = {
  async render() {
    const b = bookingState.get(), user = userState.get();
    const studio = await getStudioById(b.studioId);
    if (!studio) return { redirect: '/' };
    setStep(3);
    const days = getDayCount(b), hours = t('common.hours', { h: getSessionHours(b) });
    const booking = DetailList([
      { label: t('common.date'), value: formatLongDate(b.dateStart) + (b.dateEnd ? ' → ' + formatLongDate(b.dateEnd, false) : '') },
      { label: t('common.time'), value: `${b.startHour}:00–${b.endHour}:00`, ltr: true },
      { label: t('common.duration'), value: (days > 1 ? t('common.days', { d: days }) + ' × ' : '') + hours },
      { label: t('common.location'), value: t('location.address').join(', ') },
    ]);
    const details = DetailList([
      { label: t('common.name'), value: user.name },
      { label: t('common.phone'), value: user.phone, ltr: true },
      { label: t('common.email'), value: user.email },
    ]);
    return `<section class="pg"><div style="display:flex;gap:16px;align-items:center"><div class="chk">${checkBig}</div><div><h1 style="font-size:40px">${t('success.thanks', { n: user.name })}</h1><p class="sub" style="margin-top:6px">${t('success.message')}</p></div></div>${BookingProgress({ step: 3 })}<div class="split"><div class="cd" style="max-width:640px"><div class="thm"><img src="${studio.image}" alt=""></div><h3>${t(`studios.${studio.id}.name`)}</h3><span class="bdg">${t('success.received')}</span>${booking}${renderBookingSummary({ pricing: calculatePricing(studio), totalOnly: true })}</div><div class="cd" style="max-width:480px"><h3>${t('success.yourDetails')}</h3>${details}<div class="apn"><b style="color:var(--g)">${t('success.checkEmail')}</b><br>${t('success.checkEmailBody', { e: user.email })}</div><div class="ctl">${Button({ label: t('success.backHome'), href: '#/', large: true })}${Button({ label: t('success.viewProfile'), href: '#/profile', variant: 'light', large: true })}</div></div></div></section>`;
  },
  mount(root) { mountBookingProgress(root, { step: 3 }); },
};
