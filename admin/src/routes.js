// Route table. navId = which header link is active. public = open without logging in.
import { DashboardPage } from './pages/Dashboard/DashboardPage.js';
import { LoginPage } from './pages/Login/LoginPage.js';
import { BookingsPage } from './pages/Bookings/BookingsPage.js';
import { BookingDetailsPage } from './pages/BookingDetails/BookingDetailsPage.js';
import { StudiosPage } from './pages/Studios/StudiosPage.js';
import { StudioFormPage } from './pages/StudioForm/StudioFormPage.js';

export const routes = [
  { path: '/', navId: 'dashboard', page: DashboardPage },
  { path: '/bookings', navId: 'bookings', page: BookingsPage },
  { path: '/bookings/:id', navId: 'bookings', page: BookingDetailsPage },
  { path: '/studios', navId: 'studios', page: StudiosPage },
  { path: '/studios/new', navId: 'studios', page: StudioFormPage },
  { path: '/studios/:id/edit', navId: 'studios', page: StudioFormPage },
  { path: '/login', page: LoginPage, public: true },
];
