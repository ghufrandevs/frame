// Route table. navId = which header link is underlined. auth = only for logged-in customers. Add new pages here.
import { HomePage } from './pages/Home/HomePage.js';
import { ReservationPage } from './pages/Reservation/ReservationPage.js';
import { PaymentPage } from './pages/Payment/PaymentPage.js';
import { BookingSuccessPage } from './pages/BookingSuccess/BookingSuccessPage.js';
import { ProfilePage } from './pages/Profile/ProfilePage.js';
import { LocationPage } from './pages/Location/LocationPage.js';

export const routes = [
  { path: '/', navId: 'home', page: HomePage },
  { path: '/reservation/:studioId', navId: 'studios', page: ReservationPage },
  { path: '/studios/:studioId', redirect: ({ studioId }) => `/reservation/${studioId}` },
  { path: '/payment', page: PaymentPage, auth: true },
  { path: '/booking-success', page: BookingSuccessPage, auth: true },
  { path: '/profile', page: ProfilePage, auth: true },
  { path: '/location', navId: 'location', page: LocationPage },
];
