// Minimal hash router. Pages are objects: { render(params) -> html | {redirect}, mount?(root, params), onParams?(params), unmount?() }.
// Hash routing works on any static host (Netlify, GitHub Pages) with no server rewrites.
// A route can be guarded: guard(route, path) returns a path to go to instead (e.g. '/login'), or null.
// If render fails (server down, session expired...), onError(error) returns html or { redirect }.
let routes = [], outlet, onRender, guard, onError, current = null, seq = 0;

const compile = (path) => {
  const keys = [];
  const re = new RegExp('^' + path.replace(/:([A-Za-z]+)/g, (_, k) => (keys.push(k), '([^/]+)')) + '/?$');
  return { re, keys };
};
function match(path) {
  for (const route of routes) {
    const { re, keys } = (route.compiled ??= compile(route.path));
    const m = re.exec(path);
    if (m) return { route, params: Object.fromEntries(keys.map((k, i) => [k, decodeURIComponent(m[i + 1])])) };
  }
  return null;
}

export function navigate(path, { replace = false } = {}) {
  if (replace) { history.replaceState(null, '', '#' + path); return handle(); }
  if (location.hash === '#' + path) return handle();
  location.hash = '#' + path;
}

async function render(page, params) {
  try {
    return await page.render(params);
  } catch (error) {
    return onError(error);
  }
}

async function handle(force = false) {
  const path = location.hash.slice(1) || '/';
  const found = match(path);
  if (!found) return navigate('/', { replace: true });
  const { route, params } = found;
  if (route.redirect) return navigate(route.redirect(params), { replace: true });
  const blockedBy = guard(route, path);
  if (blockedBy) return navigate(blockedBy, { replace: true });
  const ticket = ++seq, page = route.page;
  // Same page, new params (e.g. another studio): let the page animate instead of re-rendering.
  if (!force && current?.route === route && page.onParams) { await page.onParams(params); onRender(route, params); return; }
  current?.route.page.unmount?.();
  current = null;
  const html = await render(page, params);
  if (ticket !== seq) return; // a newer navigation won
  if (html?.redirect) return navigate(html.redirect, { replace: true });
  outlet.innerHTML = `<main>${html}</main>`;
  scrollTo(0, 0);
  current = { route, params };
  page.mount?.(outlet, params);
  onRender(route, params);
}

export const refresh = () => handle(true); // full re-render of the current route (language change, saved profile)
export function startRouter(config) {
  ({ routes, outlet, onRender, guard = () => null, onError } = config);
  addEventListener('hashchange', () => handle());
  handle();
}
