import { t } from '../../i18n/i18n.js';
import { Button } from '../../components/Button/Button.js';

const MAPS_URL = 'https://www.google.com/maps/search/?api=1&query=Muscat+Oman'; // replace with the real studio location
const link = { href: MAPS_URL, extra: 'target="_blank" rel="noopener"' };

export const LocationPage = {
  render() {
    return `<section class="pg"><h1 style="font-size:48px">${t('nav.location')}</h1><p class="sub">${t('location.subtitle')}</p><div class="split"><div class="map" role="img" aria-label="${t('location.name')}"><div class="pin"></div></div><div class="cd" style="max-width:380px"><p class="eb">${t('location.label')}</p><h3>${t('location.name')}</h3><p style="color:var(--m);line-height:1.7">${t('location.address').join('<br>')}</p>${Button({ label: t('location.getDirections'), ...link })}${Button({ label: t('location.openMaps'), variant: 'light', ...link })}</div></div></section>`;
  },
};
