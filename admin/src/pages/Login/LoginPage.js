import { t } from '../../i18n/i18n.js';
import { login, isLoggedIn } from '../../services/authService.js';
import { navigate } from '../../router.js';
import { errorMessage } from '../../utils/errors.js';

export const LoginPage = {
  render() {
    if (isLoggedIn()) return { redirect: '/' };
    return `<section class="login">
      <form class="login-card" id="adminLoginForm" novalidate>
        <h1>${t('login.title')}</h1>
        <p class="page-sub">${t('login.subtitle')}</p>
        <div class="field">
          <label for="adminEmail">${t('login.email')}</label>
          <input id="adminEmail" type="email" autocomplete="username" required>
        </div>
        <div class="field">
          <label for="adminPassword">${t('login.password')}</label>
          <input id="adminPassword" type="password" autocomplete="current-password" required>
        </div>
        <p class="form-error" id="adminLoginError" role="alert"></p>
        <button type="submit" class="btn btn-wide" id="adminLoginButton">${t('login.button')}</button>
      </form>
    </section>`;
  },

  mount(root) {
    const form = root.querySelector('#adminLoginForm');
    const email = root.querySelector('#adminEmail');
    const password = root.querySelector('#adminPassword');
    const error = root.querySelector('#adminLoginError');
    const button = root.querySelector('#adminLoginButton');

    form.onsubmit = async (e) => {
      e.preventDefault();
      error.textContent = '';
      if (!email.value.trim() || !password.value) { error.textContent = t('login.fillAll'); return; }
      button.disabled = true;
      try {
        await login(email.value.trim(), password.value);
        navigate('/');
      } catch (err) {
        error.textContent = errorMessage(err);
        password.value = '';
        password.focus();
      } finally {
        button.disabled = false;
      }
    };
  },
};
