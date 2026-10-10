import { t } from '../../i18n/i18n.js';
import { formatCardNumber, formatExpiry, formatCVV, escapeHtml } from '../../utils/formatting.js';
import { validateCardName, validateCardNumber, validateExpiryDate, validateCVV } from '../../utils/validation.js';

const field = (id, label, attrs) => `<div class="ff"><label for="${id}">${label}</label><input id="${id}" ${attrs}><div class="er"></div></div>`;

export const renderCardPaymentForm = ({ name }) => `<div id="cf" style="border-top:1px solid var(--b);padding-top:18px">${field('cn', t('payment.cardName'), `autocomplete="cc-name" value="${escapeHtml(name)}"`)}${field('cc', t('payment.cardNumber'), 'dir="ltr" inputmode="numeric" autocomplete="cc-number" placeholder="0000 0000 0000 0000" maxlength="19"')}<div class="rw">${field('ce', t('payment.expiry'), 'dir="ltr" inputmode="numeric" autocomplete="cc-exp" placeholder="MM / YY" maxlength="7"')}${field('cv', t('payment.cvv'), 'dir="ltr" inputmode="numeric" autocomplete="cc-csc" placeholder="000" maxlength="3"')}</div></div>`;

// Input formatting + inline validation. Returns { validate(), takeCardNumber(), setDisabled(bool) }.
export function mountCardPaymentForm(root) {
  const q = (id) => root.querySelector(id);
  const inputs = ['#cn', '#cc', '#ce', '#cv'].map(q);
  const checks = [
    [q('#cn'), validateCardName, 'payment.errors.name'],
    [q('#cc'), validateCardNumber, 'payment.errors.number'],
    [q('#ce'), validateExpiryDate, 'payment.errors.expiry'],
    [q('#cv'), validateCVV, 'payment.errors.cvv'],
  ];
  function check([el, isValid, errorKey], showEmpty) {
    const ok = isValid(el.value);
    if (showEmpty || el.value) {
      el.classList.toggle('bad', !ok);
      el.classList.toggle('good', ok);
      el.setAttribute('aria-invalid', !ok);
      el.nextElementSibling.textContent = ok ? '' : t(errorKey);
    }
    return ok;
  }
  q('#cc').oninput = (e) => (e.target.value = formatCardNumber(e.target.value));
  q('#ce').oninput = (e) => (e.target.value = formatExpiry(e.target.value, e.target.dataset.deleting === '1'));
  q('#ce').onkeydown = (e) => (e.target.dataset.deleting = e.key === 'Backspace' ? '1' : '');
  q('#cv').oninput = (e) => (e.target.value = formatCVV(e.target.value));
  checks.forEach((c) => {
    c[0].onblur = () => check(c, true);
    c[0].addEventListener('input', () => c[0].classList.contains('bad') && check(c, true));
  });
  return {
    validate() {
      const results = checks.map((c) => check(c, true));
      checks[results.indexOf(false)]?.[0].focus();
      return results.every(Boolean);
    },
    // Hands the card number over once and clears the number, expiry and CVV from the page,
    // so card data never stays in the browser after the token is made.
    takeCardNumber() {
      const number = q('#cc').value;
      ['#cc', '#ce', '#cv'].forEach((id) => { q(id).value = ''; q(id).classList.remove('good', 'bad'); });
      return number;
    },
    setDisabled: (disabled) => inputs.forEach((el) => (el.disabled = disabled)),
  };
}
