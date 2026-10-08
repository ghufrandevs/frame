import { createStore } from './store.js';

const KEY = 'frame_lang';
const read = () => { try { return sessionStorage.getItem(KEY) || 'en'; } catch { return 'en'; } };

// Global UI state. Language persists for the browser session.
export const appState = createStore({ language: read() });
appState.subscribe(({ language }) => { try { sessionStorage.setItem(KEY, language); } catch { /* ignore */ } });
