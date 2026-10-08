import { frameSymbol } from '../../assets/icons/index.js';
import { navItems } from '../../data/navigation.js';
import { t } from '../../i18n/i18n.js';
import { Button } from '../Button/Button.js';
import { renderLanguageSwitcher, mountLanguageSwitcher } from '../LanguageSwitcher/LanguageSwitcher.js';
import { scrollToFooter } from '../Footer/Footer.js';

// activeNavId: navItems id to underline; studioId: studio the "Discover studios" link opens.
export function renderHeader({ activeNavId, studioId }) {
  const links = navItems.map((item) => {
    const href = item.anchor ? `#${item.anchor}` : `#${item.path.replace('{studioId}', studioId)}`;
    return `<a href="${href}" class="${item.id === activeNavId ? 'on' : ''}"${item.anchor ? ` data-anchor="${item.anchor}"` : ''}>${t(item.labelKey)}</a>`;
  }).join('');
  return `<header class="nav"><a class="brand" href="#/" aria-label="FRAME"><span>${frameSymbol}</span>FRAME</a><nav class="mn" aria-label="Main">${links}</nav><div class="act">${renderLanguageSwitcher()}${Button({ label: t('nav.profile'), href: '#/profile', id: 'pf', style: 'min-width:88px' })}</div></header><div class="mnav">${links}</div>`;
}

export function mountHeader(root) {
  mountLanguageSwitcher();
  root.querySelectorAll('[data-anchor]').forEach((a) => (a.onclick = (e) => { e.preventDefault(); scrollToFooter(); }));
}
