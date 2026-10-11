import { frameSymbol } from '../../assets/icons.js';
import { navItems } from '../../data/navigation.js';
import { t, otherLanguage, setLanguage } from '../../i18n/i18n.js';
import { logout } from '../../services/authService.js';
import { navigate } from '../../router.js';

// loggedIn = false on the login page: only the logo and the language button.
export function renderAdminHeader({ activeNavId, loggedIn }) {
  const links = navItems.map((item) =>
    `<a href="#${item.path}"${item.id === activeNavId ? ' class="on" aria-current="page"' : ''}>${t(item.labelKey)}</a>`).join('');
  const other = otherLanguage();
  return `<header class="ah">
    <a class="ah-brand" href="#/">${frameSymbol}FRAME<span class="ah-badge">Admin</span></a>
    ${loggedIn ? `<nav class="ah-nav" aria-label="Admin">${links}</nav>` : ''}
    <div class="ah-act">
      <button type="button" class="ah-lang" id="adminLang" lang="${other}">${other === 'ar' ? 'العربية' : 'English'}</button>
      ${loggedIn ? `<button type="button" class="btn" id="adminLogout">${t('nav.logout')}</button>` : ''}
    </div>
  </header>`;
}

export function mountAdminHeader(root) {
  root.querySelector('#adminLang').onclick = () => setLanguage(otherLanguage());
  const logoutButton = root.querySelector('#adminLogout');
  if (logoutButton) logoutButton.onclick = () => { logout(); navigate('/login'); };
}
