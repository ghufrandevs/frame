import { t } from '../../i18n/i18n.js';
import { getStudios } from '../../services/studioService.js';
import { bookingState, setStep } from '../../state/bookingState.js';
import { userState } from '../../state/userState.js';
import { formatLongDate } from '../../utils/date.js';
import { escapeHtml } from '../../utils/formatting.js';
import { defaultStudioImage } from '../../data/studioMedia.js';
import { checkBig } from '../../assets/icons/index.js';
import { Button } from '../../components/Button/Button.js';
import { BookingProgress, mountBookingProgress } from '../../components/BookingProgress/BookingProgress.js';
import { DetailList } from '../../components/DetailList/DetailList.js';
import { renderTotal } from '../../components/BookingSummary/BookingSummary.js';

// Shows the booking exactly as the server saved it (the answer to POST /bookings):
// booking number, studio, time, invoice total and the card that paid.
export const BookingSuccessPage = {
  async render() {
    const booking = bookingState.get().lastBooking, user = userState.get();
    if (!booking) return { redirect: '/' };
    setStep(3);
    const studio = (await getStudios()).find((s) => s.apiId === booking.studio.id);
    const { invoice, payment } = booking;

    const details = DetailList([
      { label: t('success.bookingNumber'), value: escapeHtml(booking.bookingNumber), ltr: true },
      { label: t('common.date'), value: formatLongDate(booking.date) },
      { label: t('common.time'), value: `${booking.startHour}:00–${booking.endHour}:00`, ltr: true },
      { label: t('common.duration'), value: t('common.hours', { h: invoice.hours }) },
      { label: t('common.location'), value: t('location.address').join(', ') },
      { label: t('success.paidWith'), value: `${escapeHtml(payment.cardBrand)} •••• ${escapeHtml(payment.cardLast4)}`, ltr: true },
    ]);
    const customer = DetailList([
      { label: t('common.name'), value: escapeHtml(user.name) },
      { label: t('common.phone'), value: escapeHtml(user.phone), ltr: true },
      { label: t('common.email'), value: escapeHtml(user.email) },
    ]);

    return `<section class="pg"><div style="display:flex;gap:16px;align-items:center"><div class="chk">${checkBig}</div><div><h1 style="font-size:40px">${t('success.thanks', { n: escapeHtml(user.name) })}</h1><p class="sub" style="margin-top:6px">${t('success.message')}</p></div></div>${BookingProgress({ step: 3 })}<div class="split"><div class="cd" style="max-width:640px"><div class="thm"><img src="${studio?.image ?? defaultStudioImage}" alt=""></div><h3>${escapeHtml(booking.studio.name)}</h3><span class="bdg">${t('success.received')}</span>${details}${renderTotal(invoice.total)}</div><div class="cd" style="max-width:480px"><h3>${t('success.yourDetails')}</h3>${customer}<div class="apn"><b style="color:var(--g)">${t('success.checkEmail')}</b><br>${t('success.checkEmailBody', { e: escapeHtml(user.email) })}</div><div class="ctl">${Button({ label: t('success.backHome'), href: '#/', large: true })}${Button({ label: t('success.viewProfile'), href: '#/profile', variant: 'light', large: true })}</div></div></div></section>`;
  },
  mount(root) { mountBookingProgress(root, { step: 3 }); },
};
