// Pill button/link. variant: 'primary' | 'light' | 'cta' (full-width booking button with trailing icon).
// type: 'button' (default) or 'submit' for the main button of a form, so Enter submits it.
import { arrowRight } from '../../assets/icons/index.js';

export function Button({ label, variant = 'primary', large = false, href, id, style, type = 'button', extra = '' }) {
  const attrs = `${id ? ` id="${id}"` : ''}${style ? ` style="${style}"` : ''} ${extra}`;
  if (variant === 'cta') return `<button class="go"${attrs}>${label}${arrowRight}</button>`;
  const cls = ['pill', large && 't', variant === 'light' && 'l'].filter(Boolean).join(' ');
  return href ? `<a class="${cls}" href="${href}"${attrs}>${label}</a>` : `<button type="${type}" class="${cls}"${attrs}>${label}</button>`;
}
