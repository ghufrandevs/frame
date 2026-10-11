import { t } from '../../i18n/i18n.js';

// Two camera photos that take turns jumping: the first jumps, swaps to the second at the top of the jump,
// the second lands, jumps, and swaps back. The motion itself is pure CSS (CameraJump.css).
// Put the two transparent PNGs at src/assets/images/auth/ with exactly these names.
const CAMERAS = [
  new URL('../../assets/images/auth/camera-1.png', import.meta.url).href,
  new URL('../../assets/images/auth/camera-2.png', import.meta.url).href,
];

export const renderCameraJump = () =>
  `<div class="cj" aria-hidden="true"><div class="cj-body">${CAMERAS.map((src, i) => `<img class="cj-img cj-${i + 1}" src="${src}" alt="" draggable="false">`).join('')}</div></div>`;

// Starts the loop only after BOTH photos have loaded, so it never shows a broken image mid-jump.
// If a photo fails to load, the first camera simply stays still.
export async function mountCameraJump(root) {
  const box = root.querySelector('.cj');
  if (!box) return;
  box.setAttribute('aria-label', t('auth.cameraAlt'));
  const images = [...box.querySelectorAll('.cj-img')];
  try {
    await Promise.all(images.map((img) => img.decode()));
    box.classList.add('is-ready');
  } catch {
    images[1]?.remove();
  }
}
