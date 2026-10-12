import { t } from '../../i18n/i18n.js';
import { navigate } from '../../router.js';
import { getStudio, createStudio, updateStudio, studioName } from '../../services/studioService.js';
import { showToast } from '../../components/Toast/Toast.js';
import { errorMessage, fieldError } from '../../utils/errors.js';
import { escapeHtml } from '../../utils/format.js';

// Add (#/studios/new) and edit (#/studios/:id/edit) use the same form.
// Each language section is labelled in its own language, whatever language the admin app is in.
// The photo is chosen once, when the studio is added; editing keeps it (the current link is sent back unchanged).
// The browser checks the same rules as the server first, so most mistakes show instantly;
// the server's answer (400 VALIDATION_ERROR) is shown under the same fields.

const MAX_NAME = 100;
const MAX_DESCRIPTION = 1000;
const MAX_IMAGE_URL = 500;
const EMPTY = { nameEn: '', nameAr: '', descriptionEn: '', descriptionAr: '', imageUrl: '', pricePerHour: '', openHour: 9, closeHour: 22 };

let current = null; // the studio being edited, or null when adding

const hourLabel = (h) => `${String(h).padStart(2, '0')}:00`;
const hourOptions = (from, to, selected) => Array.from({ length: to - from + 1 }, (_, i) => from + i)
  .map((h) => `<option value="${h}"${h === Number(selected) ? ' selected' : ''}>${hourLabel(h)}</option>`).join('');

const field = (id, label, control, hint = '') => `<div class="field" data-field="${id}">
  <label for="studio-${id}">${label}</label>
  ${control}
  ${hint ? `<p class="field-hint">${hint}</p>` : ''}
  <p class="field-error" id="studio-${id}-error" dir="auto"></p>
</div>`;

const text = (id, value, { rtl = false, max } = {}) =>
  `<input id="studio-${id}" name="${id}" value="${escapeHtml(value)}" maxlength="${max}"${rtl ? ' dir="rtl" lang="ar"' : ''} aria-describedby="studio-${id}-error">`;
const area = (id, value, { rtl = false } = {}) =>
  `<textarea id="studio-${id}" name="${id}" rows="4" maxlength="${MAX_DESCRIPTION}"${rtl ? ' dir="rtl" lang="ar"' : ''} aria-describedby="studio-${id}-error">${escapeHtml(value)}</textarea>`;

// Same rules as AdminStudioRequestValidator on the server.
function validate(v) {
  const errors = {};
  for (const key of ['nameEn', 'nameAr', 'descriptionEn', 'descriptionAr', 'imageUrl']) if (key in v && !v[key]) errors[key] = 'REQUIRED';
  const price = Number(v.pricePerHour);
  if (v.pricePerHour === '') errors.pricePerHour = 'REQUIRED';
  else if (!(price > 0) || Number(price.toFixed(3)) !== price) errors.pricePerHour = 'PRICE_INVALID'; // more than 0, at most 3 decimals
  if (Number(v.closeHour) <= Number(v.openHour)) errors.closeHour = 'HOURS_INVALID';
  return errors;
}

function readForm(form) {
  const data = Object.fromEntries(new FormData(form));
  for (const key of Object.keys(data)) data[key] = data[key].trim();
  return data;
}

function showErrors(form, errors) {
  form.querySelectorAll('[data-field]').forEach((box) => {
    const message = errors[box.dataset.field] ?? '';
    box.querySelector('.field-error').textContent = message;
    box.querySelector('input, textarea, select').toggleAttribute('aria-invalid', Boolean(message));
  });
  form.querySelector('[aria-invalid]')?.focus();
}

export const StudioFormPage = {
  async render({ id }) {
    current = id ? await getStudio(id) : null;
    const s = current ?? EMPTY;
    const title = current ? t('studioForm.editTitle').replace('{n}', escapeHtml(studioName(current))) : t('studioForm.addTitle');
    return `<section class="page">
      <a class="back" href="#/studios">${t('studioForm.back')}</a>
      <header class="page-head"><h1>${title}</h1></header>
      <form class="panel studio-form" id="studioForm" novalidate>
        <fieldset lang="en" dir="ltr">
          <legend>English</legend>
          ${field('nameEn', 'Name', text('nameEn', s.nameEn, { max: MAX_NAME }))}
          ${field('descriptionEn', 'Description', area('descriptionEn', s.descriptionEn))}
        </fieldset>
        <fieldset lang="ar" dir="rtl">
          <legend>العربية</legend>
          ${field('nameAr', 'الاسم', text('nameAr', s.nameAr, { rtl: true, max: MAX_NAME }))}
          ${field('descriptionAr', 'الوصف', area('descriptionAr', s.descriptionAr, { rtl: true }))}
        </fieldset>
        <fieldset class="studio-form-wide">
          <legend>${t(current ? 'studioForm.detailsEdit' : 'studioForm.details')}</legend>
          <div class="studio-form-row">
            ${field('pricePerHour', t('studioForm.price'), `<input id="studio-pricePerHour" name="pricePerHour" type="number" inputmode="decimal" min="0.001" step="0.001" dir="ltr" value="${s.pricePerHour}" aria-describedby="studio-pricePerHour-error">`, current ? t('studioForm.priceHint') : '')}
            ${field('openHour', t('studioForm.opens'), `<select id="studio-openHour" name="openHour" dir="ltr">${hourOptions(0, 23, s.openHour)}</select>`)}
            ${field('closeHour', t('studioForm.closes'), `<select id="studio-closeHour" name="closeHour" dir="ltr" aria-describedby="studio-closeHour-error">${hourOptions(1, 24, s.closeHour)}</select>`)}
          </div>
          ${current ? '' : field('imageUrl', t('studioForm.image'), `<input id="studio-imageUrl" name="imageUrl" type="text" dir="ltr" maxlength="${MAX_IMAGE_URL}" placeholder="https://" value="${escapeHtml(s.imageUrl)}" aria-describedby="studio-imageUrl-error">`, t('studioForm.imageHint'))}
        </fieldset>
        <p class="form-error" id="studioFormError" role="alert"></p>
        <div class="studio-form-actions">
          <button type="submit" class="btn btn-tall" id="saveStudio">${t(current ? 'studioForm.save' : 'studioForm.create')}</button>
          <a class="btn btn-light btn-tall" href="#/studios">${t('studioForm.cancel')}</a>
        </div>
      </form>
    </section>`;
  },

  mount(root) {
    const form = root.querySelector('#studioForm');
    const save = root.querySelector('#saveStudio');
    const formError = root.querySelector('#studioFormError');

    // A fixed field loses its error as soon as the admin types.
    form.addEventListener('input', (e) => {
      const box = e.target.closest('[data-field]');
      if (!box) return;
      box.querySelector('.field-error').textContent = '';
      e.target.removeAttribute('aria-invalid');
    });

    form.onsubmit = async (e) => {
      e.preventDefault();
      formError.textContent = '';
      const values = readForm(form);
      const local = validate(values);
      if (Object.keys(local).length) {
        showErrors(form, Object.fromEntries(Object.entries(local).map(([k, code]) => [k, t(`fieldErrors.${code}`)])));
        return;
      }
      const body = { ...values, imageUrl: current ? current.imageUrl : values.imageUrl, pricePerHour: Number(values.pricePerHour), openHour: Number(values.openHour), closeHour: Number(values.closeHour) };
      save.disabled = true;
      try {
        const saved = current ? await updateStudio(current.id, body) : await createStudio(body);
        showToast(t(current ? 'studioForm.saved' : 'studioForm.created').replace('{n}', studioName(saved)));
        navigate('/studios');
      } catch (error) {
        save.disabled = false;
        if (error.code === 'VALIDATION_ERROR') {
          showErrors(form, Object.fromEntries(Object.keys(error.fieldErrors).map((k) => [k, fieldError(error, k)])));
        }
        formError.textContent = errorMessage(error);
      }
    };
  },
};
