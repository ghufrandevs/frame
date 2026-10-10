import { t } from '../i18n/i18n.js';

// Turns any failure (ApiError from the server, or anything unexpected) into a message for the customer.
// The server's error code picks the text; an unknown code falls back to a general message.
export function errorMessage(error) {
  const code = error?.code;
  const text = code ? t(`errors.${code}`) : null;
  return text && text !== `errors.${code}` ? text : t('errors.UNKNOWN_ERROR');
}
