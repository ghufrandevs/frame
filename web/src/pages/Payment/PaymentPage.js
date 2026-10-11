import { t } from '../../i18n/i18n.js';
import { navigate } from '../../router.js';
import { getStudioById } from '../../services/studioService.js';
import { createBooking } from '../../services/bookingService.js';
import { toPaymentToken } from '../../services/paymentService.js';
import { rememberReturnPath } from '../../services/authService.js';
import { bookingState, setStep, completeBooking, toQuoteRequest } from '../../state/bookingState.js';
import { userState } from '../../state/userState.js';
import { money } from '../../utils/formatting.js';
import { errorMessage } from '../../utils/errors.js';
import { Button } from '../../components/Button/Button.js';
import { BookingProgress, mountBookingProgress } from '../../components/BookingProgress/BookingProgress.js';
import { renderPaymentMethod } from '../../components/PaymentMethod/PaymentMethod.js';
import { renderCardPaymentForm, mountCardPaymentForm } from '../../components/CardPaymentForm/CardPaymentForm.js';

// Errors that mean the chosen hours are gone: the customer must pick a new time, not retry the card.
const SESSION_ERRORS = ['SLOT_TAKEN', 'PAST_TIME', 'STUDIO_NOT_FOUND', 'STUDIO_INACTIVE'];

let studio;

export const PaymentPage = {
  async render() {
    const b = bookingState.get();
    studio = await getStudioById(b.studioId);
    if (!studio) return { redirect: '/' };
    if (!b.quote) return { redirect: `/reservation/${studio.id}` }; // no price yet: pick a session first
    setStep(2);
    const payLabel = `${t('payment.payNow')} · ${money(b.quote.total)}`;
    return `<section class="pg"><h1>${t('payment.title')}</h1>${BookingProgress({ step: 2 })}<form class="cardf" novalidate>${renderPaymentMethod()}${renderCardPaymentForm({ name: userState.get().name })}<p class="note" id="pe" role="alert"></p><div class="ctl">${Button({ label: payLabel, large: true, id: 'pn' })}${Button({ label: t('common.cancel'), variant: 'light', large: true, id: 'pc' })}</div></form></section>`;
  },

  mount(root) {
    mountBookingProgress(root, { step: 2 });
    const card = mountCardPaymentForm(root);
    const payButton = root.querySelector('#pn'), payLabel = payButton.innerHTML, errorBox = root.querySelector('#pe');
    root.querySelector('form').onsubmit = (e) => e.preventDefault();
    root.querySelector('#pc').onclick = () => navigate(`/reservation/${studio.id}`);

    const setBusy = (busy) => {
      payButton.disabled = busy;
      payButton.innerHTML = busy ? t('payment.processing') : payLabel;
      card.setDisabled(busy);
    };

    // One request does it all on the server: re-checks the time, charges the token, saves the booking.
    payButton.onclick = async () => {
      errorBox.textContent = '';
      if (!card.validate()) return;
      const paymentToken = toPaymentToken(card.takeCardNumber());
      setBusy(true);
      try {
        const booking = await createBooking({ ...toQuoteRequest(studio), paymentToken });
        completeBooking(booking);
        navigate('/booking-success');
      } catch (error) {
        setBusy(false);
        if (error.status === 401) { rememberReturnPath('/payment'); return navigate('/login'); } // session expired
        errorBox.textContent = errorMessage(error);
        if (SESSION_ERRORS.includes(error.code)) payButton.disabled = true;
      }
    };
  },
};
