import { t } from '../../i18n/i18n.js';
import { refresh } from '../../router.js';
import { getStudios } from '../../services/studioService.js';
import { getBookings } from '../../services/bookingService.js';
import { bookingState } from '../../state/bookingState.js';
import { userState, saveUser } from '../../state/userState.js';
import { formatLongDate } from '../../utils/date.js';
import { escapeHtml } from '../../utils/formatting.js';
import { Button } from '../../components/Button/Button.js';
import { DetailList } from '../../components/DetailList/DetailList.js';
import { showToast } from '../../components/Toast/Toast.js';

const input = (id, label, value, extra = '') => `<div class="ff"${extra.includes('max') ? ' style="max-width:380px"' : ''}><label for="${id}">${label}</label><input id="${id}"${/mail|phone/.test(id) ? ' dir="ltr"' : ''} value="${value}" disabled></div>`;

export const ProfilePage = {
  async render() {
    const user = userState.get();
    // Latest saved booking; before any booking exists, show the current selection (prototype behaviour).
    const booking = (await getBookings()).at(-1) ?? bookingState.get();
    const studio = (await getStudios()).find((s) => s.id === booking.studioId);
    const upcoming = !booking.date ? '' : DetailList([
      { label: t('common.date'), value: formatLongDate(booking.date) },
      { label: t('common.time'), value: `${booking.startHour}:00–${booking.endHour}:00`, ltr: true },
      { label: t('common.duration'), value: t('common.hours', { h: booking.endHour - booking.startHour }) },
    ]);
    return `<section class="pg"><h1 style="font-size:48px">${t('profile.title')}</h1><div class="split" style="margin-top:24px"><div class="cd" style="max-width:804px"><div style="display:flex;gap:16px;align-items:center;flex-wrap:wrap"><div class="av" id="av" style="${user.photo ? `background-image:url(${user.photo})` : ''}">${user.photo ? '' : user.name[0]}</div><div style="flex:1;min-width:160px"><b style="font-size:18px">${user.name}</b><p class="note">${user.email}</p></div><div class="ctl">${Button({ label: t('profile.changePhoto'), variant: 'light', id: 'cp' })}${Button({ label: t('profile.edit'), id: 'ep' })}<input type="file" id="fi" accept="image/*" hidden></div></div><div style="border-top:1px solid var(--b);padding-top:18px"><b>${t('profile.personal')}</b></div><div class="rw">${input('p1', t('profile.fullName'), user.name)}${input('p2', t('common.email'), user.email)}</div>${input('p3', t('profile.phoneNumber'), user.phone, 'max')}<div class="ctl" id="pa" hidden>${Button({ label: t('profile.save'), large: true, id: 'sv' })}${Button({ label: t('common.cancel'), variant: 'light', large: true, id: 'cx' })}</div></div><div class="cd" style="max-width:420px"><h3>${t('profile.upcoming')}</h3><div class="thm"><img src="${studio.image}" alt=""></div><b>${escapeHtml(studio.name)}</b><span class="bdg">${t('success.received')}</span>${upcoming}${Button({ label: t('profile.viewStudio'), variant: 'light', href: `#/reservation/${studio.id}` })}</div></div></section>`;
  },

  mount(root) {
    const q = (s) => root.querySelector(s), fileInput = q('#fi');
    q('#cp').onclick = () => fileInput.click();
    fileInput.onchange = () => {
      const file = fileInput.files[0];
      if (!file) return;
      const reader = new FileReader();
      reader.onload = async () => {
        await saveUser({ photo: reader.result });
        Object.assign(q('#av').style, { backgroundImage: `url(${reader.result})` });
        q('#av').textContent = '';
        showToast(t('profile.photoUpdated'));
      };
      reader.readAsDataURL(file);
    };
    q('#ep').onclick = () => { ['#p1', '#p2', '#p3'].forEach((id) => (q(id).disabled = false)); q('#pa').hidden = false; q('#p1').focus(); };
    q('#cx').onclick = refresh;
    q('#sv').onclick = async () => {
      await saveUser({ name: q('#p1').value.trim() || userState.get().name, email: q('#p2').value, phone: q('#p3').value });
      showToast(t('profile.saved'));
      refresh();
    };
  },
};
