// Short message at the bottom of the screen, same as the customer site.
// A new message replaces the one on screen instead of stacking on top of it.
export function showToast(message) {
  document.querySelectorAll('.toast').forEach((old) => old.remove());
  const el = document.createElement('div');
  el.className = 'toast';
  el.setAttribute('role', 'status');
  el.textContent = message;
  document.body.append(el);
  setTimeout(() => el.remove(), 3500);
}
