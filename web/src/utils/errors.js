import { t } from '../i18n/i18n.js';

const translate = (group, code, fallback) => {
  const key = `${group}.${code}`, text = code ? t(key) : null;
  return text && text !== key ? text : t(fallback);
};

// Turns any failure (ApiError from the server, or anything unexpected) into a message for the customer.
// The server's error code picks the text; an unknown code falls back to a general message.
export const errorMessage = (error) => translate('errors', error?.code, 'errors.UNKNOWN_ERROR');

// Message for one field of a 400 VALIDATION_ERROR: error.fieldErrors = { phone: ['PHONE_INVALID'] }.
// Returns '' when that field has no error.
export function fieldError(error, field) {
  const code = error?.fieldErrors?.[field]?.[0];
  return code ? translate('fieldErrors', code, 'fieldErrors.INVALID') : '';
}
