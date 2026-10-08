import { routes } from './routes.js';
import { startRouter, refresh } from './router.js';
import { appState } from './state/appState.js';
import { bookingState } from './state/bookingState.js';
import { loadUser } from './state/userState.js';
import { applyDirection } from './i18n/i18n.js';
import { renderHeader, mountHeader } from './components/Header/Header.js';
import { renderFooter } from './components/Footer/Footer.js';

const header = document.getElementById('header'), footer = document.getElementById('footer');

// Header and footer are shared by every page; they re-render after each navigation / language change.
function renderChrome(route) {
  applyDirection();
  header.innerHTML = renderHeader({ activeNavId: route.navId, studioId: bookingState.get().studioId });
  footer.innerHTML = renderFooter();
  mountHeader(header);
}

async function boot() {
  await loadUser();
  applyDirection();
  appState.subscribe(refresh); // language change -> re-render current page (+ chrome)
  startRouter({ routes, outlet: document.getElementById('app'), onRender: renderChrome });
}
boot();
