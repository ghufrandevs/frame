import { t } from '../../i18n/i18n.js';
import { getBooking, cancelBooking } from '../../services/bookingService.js';
import { refresh } from '../../router.js';
import { StatusBadge } from '../../components/BookingTable/BookingTable.js';
import { DetailCard } from '../../components/DetailList/DetailList.js';
import { confirmDialog } from '../../components/ConfirmDialog/ConfirmDialog.js';
import { showToast } from '../../components/Toast/Toast.js';
import { money, hourRange, dateLabel, dateTimeLabel, escapeHtml } from '../../utils/format.js';

// Only an upcoming booking can be cancelled; the server checks it again.
const canCancel = (b) => b.status === 'Upcoming';

// The booking shown on the page, kept for the cancel dialog.
let booking = null;

function cards(b) {
  const inv = b.invoice;
  const list = [
    DetailCard(t('details.session'), [
      [t('table.studio'), escapeHtml(b.studio.name)],
      [t('table.date'), dateLabel(b.date)],
      [t('table.time'), `<span dir="ltr">${hourRange(b.startHour, b.endHour)}</span>`],
      [t('details.hours'), inv.hours],
    ]),
    DetailCard(t('table.customer'), [
      [t('details.name'), escapeHtml(b.customer.fullName)],
      [t('details.email'), `<a href="mailto:${escapeHtml(b.customer.email)}">${escapeHtml(b.customer.email)}</a>`],
      [t('details.phone'), `<a href="tel:${escapeHtml(b.customer.phone)}" dir="ltr">${escapeHtml(b.customer.phone)}</a>`],
    ]),
    DetailCard(`${t('details.invoice')} <span class="muted-inline">${escapeHtml(inv.invoiceNumber)}</span>`, [
      [`${t('details.studioRate')} <span class="muted-inline" dir="ltr">${money(inv.hourlyRate)} × ${inv.hours}</span>`, money(inv.hourlyRate * inv.hours)],
      [t('details.photographer'), money(inv.photographerFee)],
      [t('details.subtotal'), money(inv.subtotal)],
      [t('details.vat'), money(inv.vatAmount)],
      [t('table.total'), money(inv.total)],
    ], { total: true }),
    DetailCard(t('details.payment'), [
      [t('details.card'), `${escapeHtml(b.payment.cardBrand)} •••• ${escapeHtml(b.payment.cardLast4)}`],
      [t('details.reference'), escapeHtml(b.payment.reference)],
      [t('details.paidAt'), dateTimeLabel(b.payment.paidAt)],
    ]),
  ];
  if (b.cancellation) {
    list.push(DetailCard(t('details.cancellation'), [
      [t('details.cancelledAt'), dateTimeLabel(b.cancellation.cancelledAt)],
      [t('details.refunded'), money(b.cancellation.refundAmount)],
    ]));
  }
  return list.join('');
}

export const BookingDetailsPage = {
  async render({ id }) {
    const b = await getBooking(id);
    booking = b;
    return `<section class="page">
      <a class="back" href="#/bookings">${t('details.back')}</a>
      <header class="details-head">
        <div>
          <div class="details-title"><h1>${escapeHtml(b.bookingNumber)}</h1>${StatusBadge(b.status)}</div>
          <p class="page-sub">${escapeHtml(b.studio.name)} · ${dateLabel(b.date)} · <span dir="ltr">${hourRange(b.startHour, b.endHour)}</span></p>
        </div>
        ${canCancel(b) ? `<button type="button" class="btn btn-outline-danger btn-tall" id="startCancel">${t('details.cancel')}</button>` : ''}
      </header>
      <div class="details-grid">${cards(b)}</div>
    </section>`;
  },

  mount(root, { id }) {
    const start = root.querySelector('#startCancel');
    if (!start) return;
    const b = booking;
    start.onclick = () => confirmDialog({
      title: t('details.confirmTitle').replace('{n}', b.bookingNumber),
      body: t('details.confirmBody').replace('{a}', `<strong>${money(b.invoice.total)}</strong>`),
      confirmLabel: t('details.confirmYes'),
      cancelLabel: t('details.confirmNo'),
      onConfirm: async () => {
        const updated = await cancelBooking(id);
        showToast(t('details.cancelled').replace('{a}', money(updated.cancellation.refundAmount)));
        refresh();
      },
    });
  },
};
