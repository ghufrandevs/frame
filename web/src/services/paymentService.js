import { api, USE_MOCK } from './api.js';

// Prototype only: nothing is charged. With a real provider, tokenize the card in the provider SDK
// and send only the token to the backend. Never post raw card numbers to your own API.
// Expected API: POST /payments { method, amount, currency, token? } -> { status: 'succeeded' | 'failed' }
export const processPayment = async (payment) => {
  if (!USE_MOCK) return api.post('/payments', payment);
  await new Promise((resolve) => setTimeout(resolve, 800));
  return { status: 'succeeded' };
};
