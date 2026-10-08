import { arrowUpRight, scan } from '../../assets/icons/index.js';
import { t } from '../../i18n/i18n.js';
import { money } from '../../utils/formatting.js';

// One studio card. Pure presentation: receives a studio object.
export function StudioCard(studio) {
  const name = t(`studios.${studio.id}.name`);
  return `<article class="card" tabindex="0" data-studio-id="${studio.id}"><div class="shw"><img src="${studio.image}" alt="${name}"></div><div class="det"><div><p class="eb">${t(`studios.${studio.id}.category`)}</p><h3>${name}</h3><p class="d">${t(`studios.${studio.id}.description`)}</p></div><p class="cap">${scan}<span>${studio.areaM2} m² &nbsp;·&nbsp; ${t('studio.capacity', { n: studio.maxPeople })}</span></p><div class="rate">${t('studio.from')}<b>${money(studio.ratePerHour)}</b>${t('common.perHour')}</div><div class="bk"><span>${t('studio.book')}</span>${arrowUpRight}</div></div></article>`;
}
