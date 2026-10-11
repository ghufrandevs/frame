import { t } from '../../i18n/i18n.js';
import { navigate } from '../../router.js';
import { register, isLoggedIn, takeReturnPath } from '../../services/authService.js';
import { loadUser } from '../../state/userState.js';
import { errorMessage, fieldError } from '../../utils/errors.js';
import { Button } from '../../components/Button/Button.js';
import { renderCameraJump, mountCameraJump } from '../../components/CameraJump/CameraJump.js';

// One input with its own error line underneath (id + "Error"), filled from the server's field errors.
const field = (id, label, attrs) => `<div class="ff"><label for="${id}">${label}</label><input id="${id}" ${attrs}><div class="er" id="${id}Error"></div></div>`;

// Server field name -> input id. The server checks every rule; the page only shows what it says.
const FIELDS = { fullName: 'registerFullName', email: 'registerEmail', phone: 'registerPhone', password: 'registerPassword' };

export const RegisterPage = {
  render() {
    if (isLoggedIn()) return { redirect: '/profile' };
    return `<section class="pg"><h1>${t('auth.signupTitle')}</h1><p class="sub" style="margin-top:4px">${t('auth.welcome')}</p><div class="split auth"><form class="auth-card" id="registerForm" novalidate><div class="hr" style="margin-bottom:20px"></div>${field('registerFullName', t('auth.fullName'), `type="text" autocomplete="name" placeholder="${t('auth.namePlaceholder')}"`)}${field('registerEmail', t('auth.email'), `type="email" dir="ltr" autocomplete="email" placeholder="${t('auth.emailPlaceholder')}"`)}${field('registerPhone', t('auth.phone'), `type="tel" dir="ltr" autocomplete="tel" inputmode="tel" placeholder="${t('auth.phonePlaceholder')}"`)}${field('registerPassword', t('auth.password'), 'type="password" autocomplete="new-password"')}<p class="auth-err" id="registerError" role="alert"></p>${Button({ label: t('auth.signupButton'), large: true, id: 'registerButton', type: 'submit' })}<p class="auth-alt">${t('auth.haveAccount')} <a href="#/login">${t('auth.logInLink')}</a></p></form><div class="auth-art">${renderCameraJump()}</div></div></section>`;
  },

  mount(root) {
    mountCameraJump(root);
    const q = (s) => root.querySelector(s);
    const input = (name) => q(`#${FIELDS[name]}`);
    const general = q('#registerError'), button = q('#registerButton');

    const showError = (name, message) => {
      input(name).classList.toggle('bad', Boolean(message));
      q(`#${FIELDS[name]}Error`).textContent = message;
    };
    const clearErrors = () => { Object.keys(FIELDS).forEach((name) => showError(name, '')); general.textContent = ''; };

    q('#registerForm').onsubmit = async (e) => {
      e.preventDefault();
      clearErrors();
      button.disabled = true;
      try {
        await register({
          fullName: input('fullName').value,
          email: input('email').value,
          phone: input('phone').value.replace(/\s/g, ''), // "9123 4567" -> "91234567"
          password: input('password').value,
        });
        await loadUser();
        navigate(takeReturnPath());
      } catch (err) {
        button.disabled = false;
        if (err.code === 'VALIDATION_ERROR') Object.keys(FIELDS).forEach((name) => showError(name, fieldError(err, name)));
        else if (err.code === 'EMAIL_TAKEN') showError('email', errorMessage(err));
        else general.textContent = errorMessage(err);
      }
    };
  },
};
