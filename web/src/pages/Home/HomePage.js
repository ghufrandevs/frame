import { getStudios } from '../../services/studioService.js';
import { StudioCard } from '../../components/StudioCard/StudioCard.js';
import { t } from '../../i18n/i18n.js';
import { navigate } from '../../router.js';

export const HomePage = {
  async render() {
    const studios = await getStudios();
    return `<section class="pad"><div class="intro"><p class="eb">${t('home.eyebrow')}</p><h1>${t('home.title')[0]}<br>${t('home.title')[1]}</h1><p class="lead">${t('home.lead')}</p></div><div class="grid">${studios.map(StudioCard).join('')}</div></section>`;
  },
  mount(root) {
    const open = (card) => card && navigate(`/reservation/${card.dataset.studioId}`);
    root.addEventListener('click', (e) => open(e.target.closest('.card')));
    root.addEventListener('keydown', (e) => e.key === 'Enter' && open(e.target.closest('.card')));
  },
};
