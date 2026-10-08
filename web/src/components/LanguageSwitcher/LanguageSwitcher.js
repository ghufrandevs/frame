import { chevronDownSmall } from '../../assets/icons/index.js';
import { languages, getLanguage, setLanguage } from '../../i18n/i18n.js';

export const renderLanguageSwitcher = () => `<div class="lang" id="lang"><button aria-haspopup="true" aria-expanded="false" id="lb"><span>${getLanguage().toUpperCase()}</span>${chevronDownSmall}</button><ul role="menu">${languages.map((l) => `<li><button data-lang="${l.code}">${l.label}</button></li>`).join('')}</ul></div>`;

// Global listeners, attached once (the switcher markup is re-rendered with the header).
let mounted = false;
export function mountLanguageSwitcher() {
  if (mounted) return;
  mounted = true;
  document.addEventListener('click', (e) => {
    const box = document.getElementById('lang');
    const toggle = e.target.closest('#lb');
    box.classList.toggle('o', !!toggle && !box.classList.contains('o'));
    toggle?.setAttribute('aria-expanded', box.classList.contains('o'));
    const choice = e.target.closest('[data-lang]');
    if (choice) setLanguage(choice.dataset.lang);
  });
  document.addEventListener('keydown', (e) => e.key === 'Escape' && document.getElementById('lang')?.classList.remove('o'));
}
