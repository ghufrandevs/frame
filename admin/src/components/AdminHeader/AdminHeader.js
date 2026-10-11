import { frameSymbol } from '../../assets/icons.js';
import { navItems } from '../../data/navigation.js';
import { t } from '../../i18n/i18n.js';
import { logout } from '../../services/authService.js';
import { navigate } from '../../router.js';
import { renderLanguageSwitcher, mountLanguageSwitcher } from '../LanguageSwitcher/LanguageSwitcher.js';

// Same header as the customer site, with "(Admin)" next to the logo.
// loggedIn = false on the login page: only the logo and the language menu.
export function renderAdminHeader({ activeNavId, loggedIn }) {
  const links = navItems.map((item) =>
    `<a href="#${item.path}"${item.id === activeNavId ? ' class="on" aria-current="page"' : ''}>${t(item.labelKey)}</a>`).join('');
  return `<header class="ah">
    <a class="ah-brand" href="#/" aria-label="FRAME Admin">${frameSymbol}FRAME<span class="ah-tag">(Admin)</span></a>
    ${loggedIn ? `<nav class="ah-nav" aria-label="Admin">${links}</nav>` : ''}
    <div class="ah-act">
      ${renderLanguageSwitcher()}
      ${loggedIn ? `<button type="button" class="btn" id="adminLogout">${t('nav.logout')}</button>` : ''}
    </div>
  </header>`;
}

export function mountAdminHeader(root) {
  mountLanguageSwitcher();
  const logoutButton = root.querySelector('#adminLogout');
  if (logoutButton) logoutButton.onclick = () => { logout(); navigate('/login'); };
}
