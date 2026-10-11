import { routes } from './routes.js';
import { startRouter, refresh } from './router.js';
import { applyDirection, onLanguageChange } from './i18n/i18n.js';
import { isLoggedIn } from './services/authService.js';
import { errorMessage } from './utils/errors.js';
import { renderAdminHeader, mountAdminHeader } from './components/AdminHeader/AdminHeader.js';
import { renderAdminFooter } from './components/AdminFooter/AdminFooter.js';

const header = document.getElementById('header');
const footer = document.getElementById('footer');
const LOGIN_PATH = '/login';

function renderChrome(route) {
  applyDirection();
  header.innerHTML = renderAdminHeader({ activeNavId: route.navId, loggedIn: isLoggedIn() });
  mountAdminHeader(header);
  footer.innerHTML = renderAdminFooter();
}

// Every page except login needs an admin token.
const guard = (route) => (route.public || isLoggedIn() ? null : LOGIN_PATH);

// Expired session -> login again. Anything else -> a message in place of the page.
function onError(error) {
  if (error?.status === 401 || error?.status === 403) return { redirect: LOGIN_PATH };
  return `<section class="page"><p class="form-error" role="alert">${errorMessage(error)}</p></section>`;
}

applyDirection();
onLanguageChange(refresh); // language change -> re-render page and header
startRouter({ routes, outlet: document.getElementById('app'), onRender: renderChrome, guard, onError });
