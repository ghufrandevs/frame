// One number on the dashboard: label on top, big value, short note under it.
export const StatCard = ({ label, value, note }) => `<article class="stat">
  <h2 class="stat-label">${label}</h2>
  <p class="stat-value">${value}</p>
  <p class="stat-note">${note}</p>
</article>`;
