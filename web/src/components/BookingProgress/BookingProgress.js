import { t } from '../../i18n/i18n.js';

const STEPS = ['reservation', 'payment', 'complete'];

// step: 1 Reservation, 2 Payment, 3 Booking complete (everything done).
export function BookingProgress({ step }) {
  return `<div class="steps" aria-label="${STEPS.map((s) => t(`progress.${s}`)).join(' › ')}">${STEPS.map((s, i) => {
    const n = i + 1, done = n < step || step === 3;
    return `${i ? '<div class="bar"><i></i></div>' : ''}<div class="stp ${done ? 'dn' : n === step ? 'cu' : ''}"${n === step ? ' aria-current="step"' : ''}><b>${done ? '✓' : n}</b>${t(`progress.${s}`)}</div>`;
  }).join('')}</div>`;
}

// Fills the connector bars after paint so the CSS width transition plays.
export function mountBookingProgress(root, { step }) {
  const bars = root.querySelectorAll('.bar');
  if (step === 2) requestAnimationFrame(() => requestAnimationFrame(() => bars[0]?.classList.add('f')));
  if (step === 3) setTimeout(() => bars.forEach((b, i) => setTimeout(() => b.classList.add('f'), i * 500)), 50);
}
