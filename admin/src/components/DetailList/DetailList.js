// A titled card of label/value pairs. rows: [[label, valueHtml], ...]; values must already be escaped.
// total: true makes the last row bold with a line above it (invoice total).
export const DetailCard = (title, rows, { total = false } = {}) => `<section class="panel dc${total ? ' dc-total' : ''}">
  <h2 class="panel-title">${title}</h2>
  <dl class="dc-list">${rows.map(([label, value]) => `<dt>${label}</dt><dd>${value}</dd>`).join('')}</dl>
</section>`;
