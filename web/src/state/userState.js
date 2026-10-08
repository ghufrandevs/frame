import { createStore } from './store.js';
import * as userService from '../services/userService.js';

export const userState = createStore({ name: '', email: '', phone: '', photo: '' });

export async function loadUser() { userState.set(await userService.getProfile()); }
export async function saveUser(patch) { userState.set(await userService.updateProfile(patch)); }
