import { frameSymbol, phone, social } from '../../assets/icons/index.js';
import { t } from '../../i18n/i18n.js';

export const renderFooter = () => `<div class="ftw"><footer class="ft" id="ft"><div class="r"><div><div class="br">${frameSymbol}FRAME</div><p class="tx">${t('footer.tagline')[0]}<br>${t('footer.tagline')[1]}</p></div><div><p class="ph">${phone}<span dir="ltr">+698 00000000</span></p><div class="soc">${social.map((s) => `<a href="#" aria-label="${s.name}">${s.svg}</a>`).join('')}</div></div></div><p class="lg">${t('footer.legal')}</p></footer></div>`;

export function scrollToFooter() {
  const footer = document.getElementById('ft');
  footer.scrollIntoView({ behavior: 'smooth' });
  footer.classList.remove('fl');
  void footer.offsetWidth; // restart the highlight animation
  footer.classList.add('fl');
}
