// Header links. `path` may use {studioId}; items with `anchor` scroll to an element instead of routing.
export const navItems = [
  { id: 'home', labelKey: 'nav.home', path: '/' },
  { id: 'studios', labelKey: 'nav.discover', path: '/reservation/{studioId}' },
  { id: 'location', labelKey: 'nav.location', path: '/location' },
  { id: 'contact', labelKey: 'nav.contact', anchor: 'ft' },
];
