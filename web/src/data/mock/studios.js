// Mock studio catalogue. Text (name, category, description) lives in i18n under studios.<id>.
// To add a studio: add an object here, drop its image in assets/images/studios, add text to i18n/en.js + ar.js.
const img = (file) => new URL(`../../assets/images/studios/${file}`, import.meta.url).href;

export const studios = [
  { id: 'daylight-loft', image: img('daylight-loft.webp'), ratePerHour: 25, areaM2: 60, maxPeople: 8 },
  { id: 'edit-suite', image: img('edit-suite.webp'), ratePerHour: 25, areaM2: 18, maxPeople: 2 },
  { id: 'content-room', image: img('content-room.webp'), ratePerHour: 30, areaM2: 32, maxPeople: 4 },
  { id: 'chroma-stage', image: img('chroma-stage.webp'), ratePerHour: 30, areaM2: 85, maxPeople: 10 },
];
