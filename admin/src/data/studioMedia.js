// Photos of the four original studios, the same files as the customer site (keyed by the studio's id in the API).
const img = (file) => new URL(`../assets/images/studios/${file}`, import.meta.url).href;
const photos = {
  1: img('daylight-loft.webp'),
  2: img('content-room.webp'),
  3: img('chroma-stage.webp'),
  4: img('edit-suite.webp'),
};

// A studio added later shows its own image link if it is a full web address; otherwise no photo.
export const studioPhoto = (studio) => photos[studio.id] ?? (/^https?:\/\//i.test(studio.imageUrl ?? '') ? studio.imageUrl : null);
