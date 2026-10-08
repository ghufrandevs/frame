// Presentation data the API does not store: photo, floor area and capacity, keyed by the studio's id in the API.
// Name, description, price and opening hours always come from the API (GET /studios).
// slug is the readable id used in URLs (#/reservation/daylight-loft) and for the category text in i18n.
const img = (file) => new URL(`../assets/images/studios/${file}`, import.meta.url).href;

export const studioMedia = {
  1: { slug: 'daylight-loft', image: img('daylight-loft.webp'), areaM2: 60, maxPeople: 8 },
  2: { slug: 'content-room', image: img('content-room.webp'), areaM2: 32, maxPeople: 4 },
  3: { slug: 'chroma-stage', image: img('chroma-stage.webp'), areaM2: 85, maxPeople: 10 },
  4: { slug: 'edit-suite', image: img('edit-suite.webp'), areaM2: 18, maxPeople: 2 },
};

// A studio the admin adds later still gets a photo; its size and capacity are simply not shown.
export const defaultStudioImage = img('daylight-loft.webp');