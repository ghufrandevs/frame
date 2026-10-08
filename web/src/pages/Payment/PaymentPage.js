import { t } from '../../i18n/i18n.js';
import { navigate } from '../../router.js';
import { getStudioById } from '../../services/studioService.js';
import * as bookingService from '../../services/bookingService.js';
import * as paymentService from '../../services/paymentService.js';
import { bookingState, setPaymentMethod, setStep, calculatePricing, toBookingPayload } from '../../state/bookingState.js';
import { userState } from '../../state/userState.js';
import { money } from '../../utils/formatting.js';
import { Button } from '../../components/Button/Button.js';
import { BookingProgress, mountBookingProgress } from '../../components/BookingProgress/BookingProgress.js';
import { renderPaymentMethod, mountPaymentMethod } from '../../components/PaymentMethod/PaymentMethod.js';
import { renderCardPaymentForm, mountCardPaymentForm } from '../../components/CardPaymentForm/CardPaymentForm.js';

let studio;

export const PaymentPage = {
  async render() {
    const b = bookingState.get();
    studio = await getStudioById(b.studioId);
    if (!studio) return { redirect: '/' };
    setStep(2);
    return `<section class="pg"><h1>${t('payment.title')}</h1>${BookingProgress({ step: 2 })}<form class="cardf" novalidate>${renderPaymentMethod(b.paymentMethod)}${renderCardPaymentForm({ name: userState.get().name })}<p class="apn" id="ap" hidden>${t('payment.applePayNote')}</p><div class="ctl">${Button({ label: `${t('payment.payNow')} · ${money(calculatePricing(studio).total)}`, large: true, id: 'pn' })}${Button({ label: t('common.cancel'), variant: 'light', large: true, id: 'pc' })}</div><p class="note">${t('payment.prototype')}</p></form></section>`;
  },

  mount(root) {
    mountBookingProgress(root, { step: 2 });
    const card = mountCardPaymentForm(root), note = root.querySelector('#ap');
    const showMethod = (method) => { card.setVisible(method === 'card'); note.hidden = method === 'card'; };
    showMethod(bookingState.get().paymentMethod);
    mountPaymentMethod(root, { onChange: (m) => { setPaymentMethod(m); showMethod(m); } });
    root.querySelector('form').onsubmit = (e) => e.preventDefault();
    root.querySelector('#pc').onclick = () => navigate(`/reservation/${studio.id}`);
    root.querySelector('#pn').onclick = async (e) => {
      const method = bookingState.get().paymentMethod;
      if (method === 'card' && !card.validate()) return;
      const button = e.currentTarget;
      button.disabled = true;
      button.textContent = t('payment.processing');
      const payment = await paymentService.processPayment({ method, amount: calculatePricing(studio).total, ...(method === 'card' ? card.getSummary() : {}) });
      if (payment.status === 'succeeded') {
        await bookingService.createBooking(toBookingPayload(studio));
        navigate('/booking-success');
      }
    };
  },
};
