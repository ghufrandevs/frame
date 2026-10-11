// Pure helpers over the server's hourly slots: [{ hour: 9, status: 'Available' | 'Booked' }, ...].
// A slot is one hour starting at `hour`; the server already removed hours that have passed.

export const freeStartHours = (slots) => slots.filter((s) => s.status === 'Available').map((s) => s.hour);

// Possible end hours for a start: the session can run until the next booked hour or closing time.
export function endHoursFor(slots, startHour) {
  const free = new Set(freeStartHours(slots));
  const ends = [];
  for (let hour = startHour; free.has(hour); hour++) ends.push(hour + 1);
  return ends;
}

// Keeps the customer's hours if they are still free on this day, otherwise picks the first free hour.
export function pickHours(slots, startHour, endHour) {
  const starts = freeStartHours(slots);
  if (starts.length === 0) return { startHour: null, endHour: null };
  const start = starts.includes(startHour) ? startHour : starts[0];
  const ends = endHoursFor(slots, start);
  return { startHour: start, endHour: ends.includes(endHour) ? endHour : ends[0] };
}
