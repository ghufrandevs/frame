// English UI copy. Keys are namespaced: t("namespace.key"). Studio copy lives under studios.<id>.
export default {
  nav: {
    home: "Home",
    discover: "Discover studios",
    location: "Location",
    contact: "Contact",
    profile: "Profile"
  },
  home: {
    eyebrow: "ROOM TO CREATE",
    title: [
      "Find your space.",
      "Make it happen."
    ],
    lead: "Discover a studio that fits your vision. Compare the spaces, choose your session and book your next creative chapter."
  },
  studios: {
    'daylight-loft': {
      name: "The Daylight Loft",
      category: "Photography",
      description: "A clean canvas for portraits, products and your next campaign."
    },
    'edit-suite': {
      name: "The Edit Suite",
      category: "Post-production",
      description: "A focused setting for your edit, colour grade and final mix."
    },
    'content-room': {
      name: "The Content Room",
      category: "Podcast & content",
      description: "Settle in for conversations, interviews and stories worth sharing."
    },
    'chroma-stage': {
      name: "The Chroma Stage",
      category: "Film & green screen",
      description: "Room to build new worlds, from motion tests to full productions."
    }
  },
  studio: {
    capacity: "Up to {n} people",
    from: "From ",
    book: "Book studio"
  },
  common: {
    perHour: "/ hr",
    cancel: "Cancel",
    date: "Date",
    time: "Time",
    duration: "Duration",
    location: "Location",
    days: "{d} days",
    hours: "{h} hours",
    name: "Name",
    phone: "Phone",
    email: "Email"
  },
  footer: {
    tagline: [
      "Good work starts with the right space.",
      "Find yours, and make something that matters."
    ],
    legal: "© 2026 FRAME.Good work starts with the right space."
  },
  booking: {
    session: "YOUR SESSION",
    reserve: "Reserve the loft",
    startTime: "Start time",
    endTime: "End time",
    durationSingle: "{h}-hour session · Include setup and pack-down time.",
    photographer: "Photographer",
    photographerNote: "Creative support for all {h} hours",
    studio: "Studio",
    tax: "TAX {p}%",
    total: "Session total",
    continue: "Continue to reservation",
    review: "Review your details before confirming. ",
    discoverOther: "Discover other studios",
    weekdays: [
      "M",
      "T",
      "W",
      "T",
      "F",
      "S",
      "S"
    ],
    hoursWord: "hours",
    noHours: "This day is fully booked. Please pick another day.",
    pickDay: "Pick a day",
    previousMonth: "Previous month",
    nextMonth: "Next month"
  },
  payment: {
    title: "Payment Details",
    applePay: " Apple Pay",
    card: "Card Payment",
    cardName: "Card name",
    cardNumber: "Card Number",
    expiry: "Expiry Date",
    cvv: "CVV",
    payNow: "Pay Now",
    prototype: "Prototype only — no real payment is processed.",
    applePayNote: "Apple Pay (prototype): confirm with Pay Now to complete your booking.",
    errors: {
      name: "Enter the name on your card",
      number: "Card number must be 16 digits",
      expiry: "Use a valid future date (MM / YY)",
      cvv: "CVV must be 3 digits"
    },
    processing: "Processing…"
  },
  progress: {
    reservation: "Reservation",
    payment: "Payment",
    complete: "Booking Complete"
  },
  success: {
    thanks: "Thank you, {n}.",
    message: "Your booking was successfully received. A confirmation email will be sent — please check your email.",
    received: "Booking received",
    yourDetails: "Your details",
    checkEmail: "Check your email",
    checkEmailBody: "A confirmation email will be sent to {e}.",
    backHome: "Back to Home",
    viewProfile: "View My Profile"
  },
  profile: {
    title: "My profile",
    changePhoto: "Change Photo",
    edit: "Edit Profile",
    personal: "Personal information",
    fullName: "Full name",
    phoneNumber: "Phone number",
    save: "Save changes",
    upcoming: "Upcoming booking",
    saved: "Profile saved",
    photoUpdated: "Photo updated",
    viewStudio: "View studio"
  },
  location: {
    getDirections: "Get Directions",
    subtitle: "Find your way to your next creative session.",
    label: "STUDIO LOCATION",
    name: "FRAME Studios",
    address: [
      "Al Maha Street",
      "Building No. 1-21",
      "Muscat Governorate, Oman"
    ],
    openMaps: "Open in Google Maps",
    note: "Example address. Replace with your final studio address."
  },
  errors: {
    NETWORK_ERROR: "We can't reach the server right now. Check your connection and try again.",
    UNKNOWN_ERROR: "Something went wrong. Please try again.",
    VALIDATION_ERROR: "Some details are missing or not valid.",
    STUDIO_NOT_FOUND: "This studio is not available right now.",
    SLOT_TAKEN: "Sorry, someone just booked these hours. Please pick another time.",
    PAST_TIME: "This time has already passed. Please pick a later time.",
    TOO_FAR_AHEAD: "Bookings can be made up to one year ahead."
  }
};
