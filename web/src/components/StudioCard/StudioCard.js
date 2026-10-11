import { arrowUpRight, scan } from '../../assets/icons/index.js';
import { t } from '../../i18n/i18n.js';
import { money, escapeHtml } from '../../utils/formatting.js';

// One studio card. Pure presentation: receives a studio from studioService.
// Size and capacity are shown only for studios that have them (see data/studioMedia.js).
export function StudioCard(studio) {
  const name = escapeHtml(studio.name);
  const size = studio.areaM2 ? `<p class="cap">${scan}<span>${studio.areaM2} m² &nbsp;·&nbsp; ${t('studio.capacity', { n: studio.maxPeople })}</span></p>` : '';
  return `<article class="card" tabindex="0" data-studio-id="${studio.id}"><div class="shw"><img src="${studio.image}" alt="${name}"></div><div class="det"><div><p class="eb">${studio.category}</p><h3>${name}</h3><p class="d">${escapeHtml(studio.description)}</p></div>${size}<div class="rate">${t('studio.from')}<b>${money(studio.ratePerHour)}</b>${t('common.perHour')}</div><div class="bk"><span>${t('studio.book')}</span>${arrowUpRight}</div></div></article>`;
}