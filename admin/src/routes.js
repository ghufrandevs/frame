// Route table. navId = which header link is active. public = open without logging in.
import { DashboardPage } from './pages/Dashboard/DashboardPage.js';
import { LoginPage } from './pages/Login/LoginPage.js';

export const routes = [
  { path: '/', navId: 'dashboard', page: DashboardPage },
  { path: '/login', page: LoginPage, public: true },
];
