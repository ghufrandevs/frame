import { t } from '../../i18n/i18n.js';
import { navigate } from '../../router.js';
import { login, isLoggedIn, takeReturnPath } from '../../services/authService.js';
import { loadUser } from '../../state/userState.js';
import { errorMessage } from '../../utils/errors.js';
import { Button } from '../../components/Button/Button.js';
import { renderCameraJump, mountCameraJump } from '../../components/CameraJump/CameraJump.js';

const field = (id, label, attrs) => `<div class="ff"><label for="${id}">${label}</label><input id="${id}" ${attrs}></div>`;

export const LoginPage = {
  render() {
    if (isLoggedIn()) return { redirect: '/profile' };
    return `<section class="pg"><h1>${t('auth.loginTitle')}</h1><p class="sub" style="margin-top:4px">${t('auth.welcome')}</p><div class="split auth"><form class="auth-card" id="loginForm" novalidate><div class="hr" style="margin-bottom:20px"></div>${field('loginEmail', t('auth.email'), `type="email" dir="ltr" autocomplete="email" placeholder="${t('auth.emailPlaceholder')}" required`)}${field('loginPassword', t('auth.password'), 'type="password" autocomplete="current-password" required')}<p class="auth-err" id="loginError" role="alert"></p>${Button({ label: t('auth.loginButton'), large: true, id: 'loginButton', type: 'submit' })}<p class="auth-alt">${t('auth.noAccount')} <a href="#/register">${t('auth.signUpLink')}</a></p></form><div class="auth-art">${renderCameraJump()}</div></div></section>`;
  },

  mount(root) {
    mountCameraJump(root);
    const q = (s) => root.querySelector(s);
    const email = q('#loginEmail'), password = q('#loginPassword'), error = q('#loginError'), button = q('#loginButton');

    q('#loginForm').onsubmit = async (e) => {
      e.preventDefault();
      error.textContent = '';
      if (!email.value.trim() || !password.value) {
        error.textContent = t('auth.fillAll');
        return;
      }
      button.disabled = true;
      try {
        await login(email.value, password.value);
        await loadUser();
        navigate(takeReturnPath()); // back to where the customer was going (e.g. the payment page)
      } catch (err) {
        error.textContent = errorMessage(err); // INVALID_CREDENTIALS, TOO_MANY_REQUESTS, NETWORK_ERROR...
        password.value = '';
        button.disabled = false;
      }
    };
  },
};
