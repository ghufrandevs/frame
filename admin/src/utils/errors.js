import { t } from '../i18n/i18n.js';

// The server's error code picks the message; an unknown code falls back to a general one.
export function errorMessage(error) {
  const key = `errors.${error?.code}`;
  const text = t(key);
  return text === key ? t('errors.UNKNOWN_ERROR') : text;
}
