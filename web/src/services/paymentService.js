// Card handling stays in the browser: the server only ever receives a payment token, never the card.
// The test gateway reads the result from the token itself: tok_{outcome}_{brand}_{last4}
//   outcome: ok | declined | nofunds    brand: visa | mastercard
// Test cards: 4242 4242 4242 4242 (Visa, paid), 5555 5555 5555 4444 (Mastercard, paid),
//             4000 0000 0000 0002 (declined), 4000 0000 0000 9995 (insufficient funds).
const OUTCOMES = { '4000000000000002': 'declined', '4000000000009995': 'nofunds' };

const digitsOf = (cardNumber) => cardNumber.replace(/\D/g, '');

// 'visa' for numbers starting with 4, 'mastercard' for 5; anything else is not accepted.
export function cardBrand(cardNumber) {
  const first = digitsOf(cardNumber)[0];
  return first === '4' ? 'visa' : first === '5' ? 'mastercard' : null;
}

export function toPaymentToken(cardNumber) {
  const digits = digitsOf(cardNumber);
  return `tok_${OUTCOMES[digits] ?? 'ok'}_${cardBrand(digits)}_${digits.slice(-4)}`;
}
