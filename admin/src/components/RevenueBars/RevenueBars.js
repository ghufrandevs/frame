import { money, escapeHtml } from '../../utils/format.js';

// rows: [{ studioId, name, revenue }], already sorted by the server (highest first).
// The highest studio fills the whole bar; the others are a share of it.
export function RevenueBars(rows) {
  const max = Math.max(0, ...rows.map((r) => r.revenue));
  const items = rows.map((r) => {
    const width = max > 0 ? Math.max(0, (r.revenue / max) * 100) : 0;
    return `<li class="rb-row">
      <span class="rb-name">${escapeHtml(r.name)}</span>
      <span class="rb-value">${money(r.revenue)}</span>
      <span class="rb-track" aria-hidden="true"><span class="rb-fill" style="width:${width.toFixed(1)}%"></span></span>
    </li>`;
  }).join('');
  return `<ul class="rb">${items}</ul>`;
}
