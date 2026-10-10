import { t } from '../../i18n/i18n.js';

// Card is the only payment method the server accepts; Apple Pay is shown as unavailable.
export const renderPaymentMethod = () => `<div class="meth" role="group"><button type="button" data-method="card" aria-pressed="true">💳 ${t('payment.card')}</button><button type="button" data-method="apple" aria-pressed="false" disabled>${t('payment.applePay')}</button></div>`;
