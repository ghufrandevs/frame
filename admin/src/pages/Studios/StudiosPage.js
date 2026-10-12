import { t } from '../../i18n/i18n.js';
import { refresh } from '../../router.js';
import { getStudios, setStudioActive, studioName } from '../../services/studioService.js';
import { StudioCard } from '../../components/StudioCard/StudioCard.js';
import { confirmDialog } from '../../components/ConfirmDialog/ConfirmDialog.js';
import { showToast } from '../../components/Toast/Toast.js';
import { errorMessage } from '../../utils/errors.js';
import { escapeHtml } from '../../utils/format.js';
import { frameSymbol } from '../../assets/icons.js';

// The studios shown on the page, kept for the Pause / Resume buttons.
let studios = [];

const countText = (list) => t('studios.count')
  .replace('{n}', list.length)
  .replace('{a}', list.filter((s) => s.isActive).length);

// Pausing hides the studio from customers, so it asks first. Resuming is safe and happens at once.
function toggle(studio) {
  const name = studioName(studio);
  if (studio.isActive) {
    confirmDialog({
      title: t('studios.pauseTitle').replace('{n}', name),
      body: t('studios.pauseBody'),
      confirmLabel: t('studios.pauseYes'),
      cancelLabel: t('studios.keepOpen'),
      onConfirm: async () => {
        await setStudioActive(studio.id, false);
        showToast(t('studios.pausedToast').replace('{n}', name));
        refresh();
      },
    });
    return;
  }
  setStudioActive(studio.id, true)
    .then(() => { showToast(t('studios.resumedToast').replace('{n}', name)); refresh(); })
    .catch((error) => showToast(errorMessage(error)));
}

export const StudiosPage = {
  async render() {
    studios = await getStudios();
    return `<section class="page">
      <header class="page-head page-head-row">
        <div>
          <h1>${t('studios.title')}</h1>
          <p class="page-sub">${escapeHtml(countText(studios))}</p>
        </div>
        <a class="btn btn-tall" href="#/studios/new">${t('studios.add')}</a>
      </header>
      ${studios.length
        ? `<div class="sc-grid">${studios.map(StudioCard).join('')}</div>`
        : `<div class="panel bt-empty"><p>${t('studios.empty')}</p></div>`}
    </section>`;
  },

  mount(root) {
    // A photo link that does not load shows the FRAME placeholder instead of a broken image.
    const usePlaceholder = (img) => img.replaceWith(Object.assign(document.createElement('span'), { className: 'sc-placeholder', innerHTML: frameSymbol }));
    root.querySelectorAll('.sc-media img').forEach((img) => {
      if (img.complete && !img.naturalWidth) usePlaceholder(img); // already failed before this ran
      else img.addEventListener('error', () => usePlaceholder(img), { once: true });
    });
    root.querySelector('.sc-grid')?.addEventListener('click', (e) => {
      const button = e.target.closest('[data-toggle]');
      if (!button) return;
      const studio = studios.find((s) => String(s.id) === button.dataset.toggle);
      if (studio) toggle(studio);
    });
  },
};
