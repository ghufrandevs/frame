// Pill button/link. variant: 'primary' | 'light' | 'cta' (full-width booking button with trailing icon).
import { arrowRight } from '../../assets/icons/index.js';

export function Button({ label, variant = 'primary', large = false, href, id, style, extra = '' }) {
  const attrs = `${id ? ` id="${id}"` : ''}${style ? ` style="${style}"` : ''} ${extra}`;
  if (variant === 'cta') return `<button class="go"${attrs}>${label}${arrowRight}</button>`;
  const cls = ['pill', large && 't', variant === 'light' && 'l'].filter(Boolean).join(' ');
  return href ? `<a class="${cls}" href="${href}"${attrs}>${label}</a>` : `<button type="button" class="${cls}"${attrs}>${label}</button>`;
}
