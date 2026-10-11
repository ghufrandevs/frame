import { routes } from './routes.js';
import { startRouter, refresh } from './router.js';
import { appState } from './state/appState.js';
import { bookingState } from './state/bookingState.js';
import { loadUser } from './state/userState.js';
import { isLoggedIn, rememberReturnPath } from './services/authService.js';
import { applyDirection } from './i18n/i18n.js';
import { errorMessage } from './utils/errors.js';
import { renderHeader, mountHeader } from './components/Header/Header.js';
import { renderFooter } from './components/Footer/Footer.js';

const header = document.getElementById('header'), footer = document.getElementById('footer');
const LOGIN_PATH = '/login';

// Header and footer are shared by every page; they re-render after each navigation / language change.
function renderChrome(route) {
  applyDirection();
  header.innerHTML = renderHeader({ activeNavId: route.navId, studioId: bookingState.get().studioId, loggedIn: isLoggedIn() });
  footer.innerHTML = renderFooter();
  mountHeader(header);
}

// Pages marked auth: true send a visitor to the login page, which brings them back afterwards.
function guard(route, path) {
  if (!route.auth || isLoggedIn()) return null;
  rememberReturnPath(path);
  return LOGIN_PATH;
}

// A page that could not load: an expired session goes to login, anything else shows a message.
function onError(error) {
  if (error?.status === 401) {
    rememberReturnPath(location.hash.slice(1) || '/');
    return { redirect: LOGIN_PATH };
  }
  return `<section class="pg"><p class="sub">${errorMessage(error)}</p></section>`;
}

async function boot() {
  await loadUser();
  applyDirection();
  appState.subscribe(refresh); // language change -> re-render current page (+ chrome)
  startRouter({ routes, outlet: document.getElementById('app'), onRender: renderChrome, guard, onError });
}
boot();
