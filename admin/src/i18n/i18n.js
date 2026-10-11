import en from './en.js';
import ar from './ar.js';

const KEY = 'frame_admin_lang';
const dictionaries = { en, ar };
const listeners = new Set();

const read = () => { try { return sessionStorage.getItem(KEY) || 'en'; } catch { return 'en'; } };
let language = read();

export const getLanguage = () => language;
export const otherLanguage = () => (language === 'ar' ? 'en' : 'ar');
export const onLanguageChange = (fn) => listeners.add(fn);

export function setLanguage(code) {
  if (!dictionaries[code] || code === language) return;
  language = code;
  try { sessionStorage.setItem(KEY, code); } catch { /* storage unavailable */ }
  listeners.forEach((fn) => fn(code));
}

const lookup = (dict, key) => key.split('.').reduce((o, k) => (o == null ? o : o[k]), dict);

// t('dashboard.title') -> text in the current language, English as fallback, then the key itself.
export function t(key) {
  return lookup(dictionaries[language], key) ?? lookup(en, key) ?? key;
}

export function applyDirection() {
  document.documentElement.lang = language;
  document.documentElement.dir = language === 'ar' ? 'rtl' : 'ltr';
}
