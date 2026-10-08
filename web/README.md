# FRAME — Studio booking front-end

Pixel-accurate implementation of the FRAME Figma design: browse four studios, reserve a single day or a date range, pay (prototype), see a confirmation, manage a profile. English + Arabic (RTL). Runs on mock data today; the service layer is ready for a real backend.

## 1. Tech stack
Vanilla JavaScript (ES modules), plain CSS, [Vite](https://vite.dev) for dev server/build. No framework, no runtime dependencies. Components are functions that return HTML strings (`render…`) plus an optional `mount…` that attaches events.

## 2. Folder structure
```
index.html              page shell (#header, #app, #footer) + font links
src/
  main.js               boot: load user, start router, render shared header/footer
  routes.js             route table  ·  router.js  hash router engine
  pages/                one folder per page (compose components, own page-specific CSS)
    Home · Reservation · Payment · BookingSuccess · Profile · Location
  components/           reusable UI, each with its own .js (+ .css)
    Header · Footer · LanguageSwitcher · Button · StudioCard · StudioRotation
    DatePicker · TimeSelector · PhotographerOption · BookingSummary · BookingProgress
    PaymentMethod · CardPaymentForm · DetailList · Toast
  data/                 config.js (pricing, hours, defaults) · navigation.js · mock/ (studios, users, bookings)
  services/             api.js (HTTP client) · studio/booking/user/payment services
  state/                store.js · bookingState.js · userState.js · appState.js
  utils/                date.js · validation.js · formatting.js
  i18n/                 en.js · ar.js · i18n.js
  styles/               variables.css (tokens) · global · typography · layout · forms · responsive · index.css
  assets/               images/studios · icons · fonts
public/                 favicon
```

## 3–5. Install, run, build
```
npm install
npm run dev        # http://localhost:5173
npm run build      # outputs dist/ (static, deploy to Netlify etc.)
npm run preview
```

## 6. Where to edit what
| I want to… | Edit |
|---|---|
| Change a studio's image / price / size | `src/data/mock/studios.js` (+ image in `assets/images/studios`) |
| Change studio names/descriptions | `src/i18n/en.js` and `ar.js` → `studios.<id>` |
| Change English / Arabic text | `src/i18n/en.js` / `src/i18n/ar.js` |
| Header, footer, buttons, cards, progress bar | the component folder (e.g. `components/BookingProgress/`) |
| Header links | `src/data/navigation.js` |
| Colours, tokens | `src/styles/variables.css` |
| Responsive rules (tablet ≤1100px, mobile ≤560px) | `src/styles/responsive.css` |
| Tax, photographer rate, opening hours, default booking | `src/data/config.js` |
| Validation rules / input formats | `src/utils/validation.js`, `src/utils/formatting.js` |

Studio text is keyed by studio id in i18n, so `t('studios.edit-suite.name')`.

## 7. Routing
Hash routes (work on any static host, no rewrites): `/`, `/reservation/:studioId`, `/payment`, `/booking-success`, `/profile`, `/location`. `/studios/:studioId` redirects to the reservation page. One `ReservationPage` serves all studios. A page is `{ render(params), mount?(root, params), onParams?(params), unmount?() }`; `render` may return `{ redirect: '/path' }`. To use clean URLs later, swap `router.js` for the History API (pages don't change).

## 8. Booking state
`state/bookingState.js` is the single source of truth: `studioId, dateStart, dateEnd (null = one day), startHour, endHour, photographer, paymentMethod, step`. Use its actions (`selectDate`, `setStartHour`, …) and selectors (`calculatePricing`, `toBookingPayload`). Components receive values via props/callbacks; they never keep their own copy.

## 9. Backend integration
All network code is in `src/services/`. UI never calls `fetch`.
1. Copy `.env.example` to `.env`, set `VITE_API_URL=https://your-api` and `VITE_USE_MOCK=false`.
2. Each service has the expected endpoints in a comment (`GET /studios`, `POST /bookings`, `GET/PUT /me`, `POST /payments`). Adjust paths/shapes there only.
3. `api.js` handles base URL, JSON, `Authorization: Bearer <token>` (use `setToken()` after login) and throws `ApiError`.
4. Payments: tokenize cards with your provider's SDK and send only the token. Never send raw card numbers to your API. `paymentService` is a prototype and charges nothing.
Environment variables: only `VITE_*` names are exposed to the browser. Never put secrets in them.

## 10. 3D studio rotation
`components/StudioRotation`. Each studio is a face of a prism (`transform-style: preserve-3d`) inside a `perspective` box. Faces are placed at `rotateY(i·360/N) translateZ(apothem)`; switching studios rotates the prism by the shortest way round while the green box, header and page stay still. Works for any number of studios (N faces). Reduced-motion is respected.

## 11. Add a studio
1. Image → `src/assets/images/studios/<id>.webp`.
2. Object in `src/data/mock/studios.js` (or return it from your API).
3. `studios.<id>.{name,category,description}` in `en.js` and `ar.js`.
Cards, thumbnails and the rotation update automatically.

## 12. Add a language
Create `src/i18n/fr.js` mirroring `en.js`, import it in `i18n.js`, add it to `dictionaries` and `languages`. Add the font if needed (`index.html`). Set RTL in `applyDirection()` if the language is right-to-left. Missing keys fall back to English.

## Notes
- Prototype dates: the design is set in October 2026, so `MIN_DATE`/calendar start in `config.js` are fixed. Replace with `new Date()` when going live.
- The Omani rial symbol is shown as the text "OMR".
- Arabic copy was written for this project; have a native speaker review it.
