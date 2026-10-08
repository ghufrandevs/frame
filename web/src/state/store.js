// Tiny observable store: get(), set(patch), subscribe(fn), reset().
export function createStore(initial) {
  let state = { ...initial };
  const listeners = new Set();
  const emit = () => listeners.forEach((fn) => fn(state));
  return {
    get: () => state,
    set(patch) { state = { ...state, ...patch }; emit(); },
    reset() { state = { ...initial }; emit(); },
    subscribe(fn) { listeners.add(fn); return () => listeners.delete(fn); },
  };
}
