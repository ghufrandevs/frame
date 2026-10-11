import { errorMessage } from '../../utils/errors.js';

// Modal "Are you sure?" box (native <dialog>: Esc closes it, focus stays inside).
// onConfirm runs when the admin confirms. If it fails, the error shows inside the box and it stays open;
// if it succeeds, the box closes. Texts are plain strings except body, which may contain formatted HTML.
export function confirmDialog({ title, body, confirmLabel, cancelLabel, onConfirm }) {
  const dialog = document.createElement('dialog');
  dialog.className = 'dlg';
  dialog.setAttribute('aria-labelledby', 'dlgTitle');
  dialog.innerHTML = `<h2 class="dlg-title" id="dlgTitle"></h2>
    <p class="dlg-body">${body}</p>
    <p class="form-error" role="alert"></p>
    <div class="dlg-actions">
      <button type="button" class="btn btn-danger btn-tall" data-confirm></button>
      <button type="button" class="btn btn-light btn-tall" data-cancel></button>
    </div>`;
  dialog.querySelector('.dlg-title').textContent = title;
  const yes = dialog.querySelector('[data-confirm]');
  const no = dialog.querySelector('[data-cancel]');
  const error = dialog.querySelector('.form-error');
  yes.textContent = confirmLabel;
  no.textContent = cancelLabel;

  no.onclick = () => dialog.close();
  dialog.addEventListener('close', () => dialog.remove());
  dialog.addEventListener('cancel', (e) => yes.disabled && e.preventDefault()); // no Esc while the request runs

  yes.onclick = async () => {
    yes.disabled = no.disabled = true;
    error.textContent = '';
    try {
      await onConfirm();
      dialog.close();
    } catch (err) {
      error.textContent = errorMessage(err);
      yes.disabled = no.disabled = false;
    }
  };

  document.body.append(dialog);
  dialog.showModal();
  no.focus(); // the safe choice is focused first
}
