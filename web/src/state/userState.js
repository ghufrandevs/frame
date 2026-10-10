import { createStore } from './store.js';
import * as userService from '../services/userService.js';
import { isLoggedIn } from '../services/api.js';

const EMPTY_USER = { name: '', email: '', phone: '' };

// The logged-in customer, or EMPTY_USER for a visitor.
export const userState = createStore(EMPTY_USER);

// Never throws: a visitor, an expired token or a server that is down all leave an empty user.
export async function loadUser() {
  if (!isLoggedIn()) return userState.set(EMPTY_USER);
  try {
    userState.set(await userService.getProfile());
  } catch {
    userState.set(EMPTY_USER);
  }
}

export async function saveUser(patch) { userState.set(await userService.updateProfile(patch)); }
export const clearUser = () => userState.set(EMPTY_USER);
