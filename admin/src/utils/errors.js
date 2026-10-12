import { t } from '../i18n/i18n.js';

const translate = (group, code, fallback) => {
  const key = `${group}.${code}`;
  const text = code ? t(key) : key;
  return text === key ? t(fallback) : text;
};

// The server's error code picks the message; an unknown code falls back to a general one.
export const errorMessage = (error) => translate('errors', error?.code, 'errors.UNKNOWN_ERROR');

// Message for one field of a 400 VALIDATION_ERROR (error.fieldErrors = { nameAr: ['REQUIRED'] }), or ''.
export function fieldError(error, field) {
  const code = error?.fieldErrors?.[field]?.[0];
  return code ? translate('fieldErrors', code, 'fieldErrors.INVALID') : '';
}
