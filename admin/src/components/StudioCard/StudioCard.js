import { t } from '../../i18n/i18n.js';
import { frameSymbol } from '../../assets/icons.js';
import { studioPhoto } from '../../data/studioMedia.js';
import { studioName, studioDescription } from '../../services/studioService.js';
import { money, hourRange, escapeHtml } from '../../utils/format.js';

// One studio in the admin list: photo, status, name, description, price, hours, Edit and Pause/Resume.
// Same card shape as the customer site. A paused studio is faded and offers Resume instead of Pause.
export function StudioCard(studio) {
  const name = escapeHtml(studioName(studio));
  const photo = studioPhoto(studio);
  const media = photo
    ? `<img src="${escapeHtml(photo)}" alt="" loading="lazy">`
    : `<span class="sc-placeholder" aria-hidden="true">${frameSymbol}</span>`;
  const status = studio.isActive
    ? `<span class="badge">${t('studios.active')}</span>`
    : `<span class="badge badge-Paused">${t('studios.paused')}</span>`;
  return `<article class="sc${studio.isActive ? '' : ' sc-paused'}">
    <div class="sc-media">${media}</div>
    <div class="sc-body">
      <div>
        ${status}
        <h2 class="sc-name">${name}</h2>
        <p class="sc-desc">${escapeHtml(studioDescription(studio))}</p>
      </div>
      <dl class="sc-facts">
        <div><dt>${t('studios.price')}</dt><dd>${money(studio.pricePerHour)} <span>${t('studios.perHour')}</span></dd></div>
        <div><dt>${t('studios.hours')}</dt><dd dir="ltr">${hourRange(studio.openHour, studio.closeHour)}</dd></div>
      </dl>
      <div class="sc-actions">
        <a class="btn btn-light" href="#/studios/${studio.id}/edit" aria-label="${t('studios.edit')}: ${name}">${t('studios.edit')}</a>
        <button type="button" class="btn ${studio.isActive ? 'btn-light' : ''}" data-toggle="${studio.id}" aria-label="${t(studio.isActive ? 'studios.pause' : 'studios.resume')}: ${name}">${t(studio.isActive ? 'studios.pause' : 'studios.resume')}</button>
      </div>
    </div>
  </article>`;
}
