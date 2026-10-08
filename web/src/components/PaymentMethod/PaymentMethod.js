import { t } from '../../i18n/i18n.js';

export const renderPaymentMethod = (method) => `<div class="meth" role="group"><button type="button" data-method="card" aria-pressed="${method === 'card'}">💳 ${t('payment.card')}</button><button type="button" data-method="apple" aria-pressed="${method === 'apple'}">${t('payment.applePay')}</button></div>`;

export function mountPaymentMethod(root, { onChange }) {
  const buttons = root.querySelectorAll('[data-method]');
  buttons.forEach((b) => (b.onclick = () => {
    buttons.forEach((x) => x.setAttribute('aria-pressed', x === b));
    onChange(b.dataset.method);
  }));
}
