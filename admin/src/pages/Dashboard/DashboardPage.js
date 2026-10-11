import { t } from '../../i18n/i18n.js';
import { getDashboard } from '../../services/dashboardService.js';
import { money, percent, monthLabel } from '../../utils/format.js';
import { StatCard } from '../../components/StatCard/StatCard.js';
import { RevenueBars } from '../../components/RevenueBars/RevenueBars.js';
import { BookingTable } from '../../components/BookingTable/BookingTable.js';

// Everything on this page comes from GET /admin/dashboard (one request).
export const DashboardPage = {
  async render() {
    const d = await getDashboard();
    return `<section class="page">
      <header class="page-head">
        <h1>${t('dashboard.title')}</h1>
        <p class="page-sub">${monthLabel(d.month)}</p>
      </header>
      <div class="stats">
        ${StatCard({ label: t('dashboard.netRevenue'), value: money(d.netRevenue), note: t('dashboard.netRevenueNote') })}
        ${StatCard({ label: t('dashboard.bookings'), value: d.bookingsCount, note: t('dashboard.bookingsNote') })}
        ${StatCard({ label: t('dashboard.occupancy'), value: percent(d.occupancyNext7Days), note: t('dashboard.occupancyNote') })}
      </div>
      <section class="panel">
        <h2 class="panel-title">${t('dashboard.revenueByStudio')} <span class="panel-hint">${t('dashboard.thisMonth')}</span></h2>
        ${RevenueBars(d.revenueByStudio)}
      </section>
      <section class="panel">
        <h2 class="panel-title">${t('dashboard.today')}</h2>
        ${BookingTable(d.today, t('dashboard.noBookingsToday'))}
      </section>
    </section>`;
  },
};
