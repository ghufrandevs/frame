import { frameSymbol, phone, social } from '../../assets/icons.js';
import { CONTACT_PHONE } from '../../data/config.js';
import { t } from '../../i18n/i18n.js';

// Same footer as the customer site.
export const renderAdminFooter = () => `<footer class="af">
  <div class="af-row">
    <div>
      <div class="af-brand">${frameSymbol}FRAME</div>
      <p class="af-tagline">${t('footer.tagline1')}<br>${t('footer.tagline2')}</p>
    </div>
    <div>
      <p class="af-phone">${phone}<span dir="ltr">${CONTACT_PHONE}</span></p>
      <div class="af-social">${social.map((s) => `<a href="#" aria-label="${s.name}">${s.svg}</a>`).join('')}</div>
    </div>
  </div>
  <p class="af-legal">${t('footer.legal')}</p>
</footer>`;
