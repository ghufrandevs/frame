// Label/value grid. rows: [{ label, value, ltr? }]
export const DetailList = (rows) =>
  `<div class="kv">${rows.map((r) => `<span>${r.label}</span><span${r.ltr ? ' dir="ltr"' : ''}>${r.value}</span>`).join('')}</div>`;
