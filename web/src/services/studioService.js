import { api } from './api.js';
import { studioMedia, defaultStudioImage } from '../data/studioMedia.js';
import { t } from '../i18n/i18n.js';

// API: GET /studios -> StudioResponse[] (active studios only, name/description in the Accept-Language language)
// StudioResponse = { id, name, description, imageUrl, pricePerHour, openHour, closeHour }
// Pages work with the shape below: id is the URL slug, apiId is what booking requests send to the server.
function toStudio(dto) {
  const media = studioMedia[dto.id];
  const slug = media?.slug ?? `studio-${dto.id}`;
  return {
    id: slug,
    apiId: dto.id,
    name: dto.name,
    description: dto.description,
    category: media ? t(`studios.${slug}.category`) : '',
    image: media?.image ?? defaultStudioImage,
    ratePerHour: dto.pricePerHour,
    openHour: dto.openHour,
    closeHour: dto.closeHour,
    areaM2: media?.areaM2 ?? null,
    maxPeople: media?.maxPeople ?? null,
  };
}

export const getStudios = async () => (await api.get('/studios')).map(toStudio);

// Looks the slug up in the list: a paused studio is not in it, so it returns null like a missing one.
export const getStudioById = async (id) => (await getStudios()).find((s) => s.id === id) ?? null;