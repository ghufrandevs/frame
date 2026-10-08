import en from './en.js';
import ar from './ar.js';
import { appState } from '../state/appState.js';

const dictionaries = { en, ar };
// To add a language: create <code>.js, register it here and add it to `languages`.
export const languages = [{ code: 'en', label: 'English' }, { code: 'ar', label: 'العربية' }];
export const getLanguage = () => appState.get().language;
export const setLanguage = (code) => dictionaries[code] && code !== getLanguage() && appState.set({ language: code });

const lookup = (dict, key) => key.split('.').reduce((o, k) => (o == null ? o : o[k]), dict);

// t('booking.total') -> string; t('success.thanks', { n: 'Noor' }) fills {n}. Falls back to English, then the key.
export function t(key, vars) {
  let value = lookup(dictionaries[getLanguage()], key) ?? lookup(en, key);
  if (value == null) return key;
  if (typeof value === 'string' && vars) for (const k in vars) value = value.replaceAll(`{${k}}`, vars[k]);
  return value;
}

export function applyDirection() {
  document.documentElement.lang = getLanguage();
  document.documentElement.dir = getLanguage() === 'ar' ? 'rtl' : 'ltr';
}
