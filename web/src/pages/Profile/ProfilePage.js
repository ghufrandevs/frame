import { t } from '../../i18n/i18n.js';
import { navigate, refresh } from '../../router.js';
import { getStudios } from '../../services/studioService.js';
import { getMyBookings, getBooking } from '../../services/bookingService.js';
import { logout } from '../../services/authService.js';
import { userState, saveUser, clearUser } from '../../state/userState.js';
import { formatLongDate } from '../../utils/date.js';
import { escapeHtml, money } from '../../utils/formatting.js';
import { errorMessage, fieldError } from '../../utils/errors.js';
import { defaultStudioImage } from '../../data/studioMedia.js';
import { Button } from '../../components/Button/Button.js';
import { DetailList } from '../../components/DetailList/DetailList.js';
import { showToast } from '../../components/Toast/Toast.js';

// Name and phone can be edited; the email is the login name, so it is always read-only.
const input = (id, label, value, { ltr = false, narrow = false } = {}) =>
  `<div class="ff"${narrow ? ' style="max-width:380px"' : ''}><label for="${id}">${label}</label><input id="${id}"${ltr ? ' dir="ltr"' : ''} value="${escapeHtml(value)}" disabled><div class="er"></div></div>`;

// "Noor Al Harthy" -> "NA": shown in the circle instead of a photo.
const initials = (name) => name.trim().split(/\s+/).slice(0, 2).map((w) => w[0]).join('').toUpperCase();

const timeOf = (b) => `${b.startHour}:00–${b.endHour}:00`;

// The next session that has not finished yet (bookings come from GET /bookings/my).
const nextSession = (bookings) => bookings
  .filter((b) => b.status === 'Upcoming' || b.status === 'InProgress')
  .sort((a, b) => a.date.localeCompare(b.date) || a.startHour - b.startHour)[0];

async function renderUpcoming(bookings) {
  const next = nextSession(bookings);
  if (!next) return `<p class="note">${t('profile.noUpcoming')}</p>${Button({ label: t('profile.bookStudio'), href: '#/' })}`;
  const [details, studios] = await Promise.all([getBooking(next.id), getStudios()]);
  const studio = studios.find((s) => s.apiId === details.studio.id);
  const rows = DetailList([
    { label: t('success.bookingNumber'), value: escapeHtml(next.bookingNumber), ltr: true },
    { label: t('common.date'), value: formatLongDate(next.date) },
    { label: t('common.time'), value: timeOf(next), ltr: true },
    { label: t('booking.total'), value: money(next.total) },
  ]);
  const studioLink = studio ? Button({ label: t('profile.viewStudio'), variant: 'light', href: `#/reservation/${studio.id}` }) : '';
  return `<div class="thm"><img src="${studio?.image ?? defaultStudioImage}" alt=""></div><b>${escapeHtml(next.studioName)}</b><span class="bdg">${t(`status.${next.status}`)}</span>${rows}${studioLink}`;
}

// Every booking of the customer, newest date first, with its status.
const renderHistory = (bookings) => bookings.length === 0 ? '' : `<div style="border-top:1px solid var(--b);padding-top:18px"><b>${t('profile.allBookings')}</b></div>${DetailList(
  [...bookings].sort((a, b) => b.date.localeCompare(a.date) || b.startHour - a.startHour).map((b) => ({
    label: `${escapeHtml(b.bookingNumber)}<br><small>${t(`status.${b.status}`)}</small>`,
    value: `${escapeHtml(b.studioName)}<br><small>${formatLongDate(b.date, false)} · <span dir="ltr">${timeOf(b)}</span> · ${money(b.total)}</small>`,
  })))}`;

export const ProfilePage = {
  async render() {
    const user = userState.get();
    const bookings = await getMyBookings();
    const upcoming = await renderUpcoming(bookings);
    return `<section class="pg"><h1 style="font-size:48px">${t('profile.title')}</h1><div class="split" style="margin-top:24px"><div class="cd" style="max-width:804px"><div style="display:flex;gap:16px;align-items:center;flex-wrap:wrap"><div class="av" aria-hidden="true">${escapeHtml(initials(user.name))}</div><div style="flex:1;min-width:160px"><b style="font-size:18px">${escapeHtml(user.name)}</b><p class="note">${escapeHtml(user.email)}</p></div><div class="ctl">${Button({ label: t('profile.logout'), variant: 'light', id: 'lo' })}${Button({ label: t('profile.edit'), id: 'ep' })}</div></div><div style="border-top:1px solid var(--b);padding-top:18px"><b>${t('profile.personal')}</b></div><div class="rw">${input('p1', t('profile.fullName'), user.name)}${input('p2', t('common.email'), user.email, { ltr: true })}</div>${input('p3', t('profile.phoneNumber'), user.phone, { ltr: true, narrow: true })}<p class="note" id="pe" role="alert"></p><div class="ctl" id="pa" hidden>${Button({ label: t('profile.save'), large: true, id: 'sv' })}${Button({ label: t('common.cancel'), variant: 'light', large: true, id: 'cx' })}</div></div><div class="cd" style="max-width:420px"><h3>${t('profile.upcoming')}</h3>${upcoming}${renderHistory(bookings)}</div></div></section>`;
  },

  mount(root) {
    const q = (s) => root.querySelector(s);
    const name = q('#p1'), phone = q('#p3'), formError = q('#pe');
    const showFieldError = (el, message) => {
      el.classList.toggle('bad', Boolean(message));
      el.nextElementSibling.textContent = message;
    };

    q('#lo').onclick = () => { logout(); clearUser(); navigate('/'); };
    q('#ep').onclick = () => { name.disabled = phone.disabled = false; q('#pa').hidden = false; name.focus(); };
    q('#cx').onclick = refresh;
    q('#sv').onclick = async (e) => {
      const button = e.currentTarget;
      button.disabled = true;
      formError.textContent = '';
      try {
        await saveUser({ name: name.value, phone: phone.value });
        showToast(t('profile.saved'));
        refresh();
      } catch (error) {
        button.disabled = false;
        showFieldError(name, fieldError(error, 'fullName'));
        showFieldError(phone, fieldError(error, 'phone'));
        if (error.code !== 'VALIDATION_ERROR') formError.textContent = errorMessage(error);
      }
    };
  },
};
