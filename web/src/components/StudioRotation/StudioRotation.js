import { t } from '../../i18n/i18n.js';

// Studios sit on the faces of a prism (a square when there are 4). Only the prism rotates (CSS 3D);
// the green background box stays still. Also renders the "Discover other studios" thumbnails.
export function renderStudioRotation(studios) {
  const faces = studios.map((s, i) => `<div class="face" data-index="${i}"><img src="${s.image}" alt="${t(`studios.${s.id}.name`)}"></div>`).join('');
  return `<div class="show" id="show"><div class="prism" id="prism">${faces}</div></div><div class="dsc"><div class="dh"><i></i><span>${t('booking.discoverOther')}</span><i></i></div><div class="th fade" id="th"></div></div>`;
}

// onSelect(studioId) is called when a thumbnail is clicked. Returns { rotateTo, renderThumbs, destroy }.
export function mountStudioRotation(root, { studios, currentId, onSelect }) {
  const show = root.querySelector('#show'), prism = root.querySelector('#prism'), thumbs = root.querySelector('#th');
  const count = studios.length, anglePerFace = 360 / count;
  let current = studios.findIndex((s) => s.id === currentId);
  let turns = current; // cumulative, so rotation direction is never reversed by wrap-around

  function layout() {
    const width = show.clientWidth * 0.92, height = width * 0.74;
    const apothem = count > 2 ? width / 2 / Math.tan(Math.PI / count) : 0; // distance from axis to each face
    Object.assign(prism.style, { width: `${width}px`, height: `${height}px`, margin: `${-height / 2}px 0 0 ${-width / 2}px` });
    prism.querySelectorAll('.face').forEach((f, i) => (f.style.transform = `rotateY(${i * anglePerFace}deg) translateZ(${apothem}px)`));
    prism.style.transform = `translateZ(${-apothem}px) rotateY(${-turns * anglePerFace}deg)`;
  }
  const instant = () => { prism.style.transition = 'none'; layout(); requestAnimationFrame(() => requestAnimationFrame(() => (prism.style.transition = ''))); };
  const onResize = () => instant();

  function rotateTo(studioId) {
    const next = studios.findIndex((s) => s.id === studioId);
    if (next < 0 || next === current) return;
    const d = (next - current + count) % count;
    turns += d > count / 2 ? d - count : d; // shortest way round
    current = next;
    layout();
  }
  function renderThumbs(activeId) {
    thumbs.innerHTML = studios.filter((s) => s.id !== activeId).map((s) => `<button data-studio-id="${s.id}" aria-label="${t(`studios.${s.id}.name`)}"><img src="${s.image}" alt="${t(`studios.${s.id}.name`)}"></button>`).join('');
  }
  thumbs.addEventListener('click', (e) => { const b = e.target.closest('[data-studio-id]'); if (b) onSelect(b.dataset.studioId); });
  window.addEventListener('resize', onResize);
  instant();
  return { rotateTo, renderThumbs, destroy: () => window.removeEventListener('resize', onResize) };
}
