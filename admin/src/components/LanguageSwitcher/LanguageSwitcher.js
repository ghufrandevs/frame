import { chevronDown } from '../../assets/icons.js';
import { languages, getLanguage, setLanguage } from '../../i18n/i18n.js';

// "EN ⌄" dropdown, same as the customer site.
export const renderLanguageSwitcher = () => `<div class="lang" id="lang">
  <button type="button" id="langButton" aria-haspopup="true" aria-expanded="false" aria-label="Language"><span>${getLanguage().toUpperCase()}</span>${chevronDown}</button>
  <ul role="menu">${languages.map((l) => `<li><button type="button" role="menuitem" data-lang="${l.code}" lang="${l.code}">${l.label}</button></li>`).join('')}</ul>
</div>`;

// The header is re-rendered on every page, so the listeners live on the document and are added once.
let mounted = false;
export function mountLanguageSwitcher() {
  if (mounted) return;
  mounted = true;
  const close = () => {
    document.getElementById('lang')?.classList.remove('o');
    document.getElementById('langButton')?.setAttribute('aria-expanded', 'false');
  };
  document.addEventListener('click', (e) => {
    const toggle = e.target.closest('#langButton');
    const box = document.getElementById('lang');
    if (toggle && box) {
      const open = !box.classList.contains('o');
      box.classList.toggle('o', open);
      toggle.setAttribute('aria-expanded', String(open));
      return;
    }
    const choice = e.target.closest('[data-lang]');
    close();
    if (choice) setLanguage(choice.dataset.lang);
  });
  document.addEventListener('keydown', (e) => e.key === 'Escape' && close());
}
