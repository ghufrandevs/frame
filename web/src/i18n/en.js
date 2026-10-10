// English UI copy. Keys are namespaced: t("namespace.key"). Studio copy lives under studios.<id>.
export default {
  nav: {
    home: "Home",
    discover: "Discover studios",
    location: "Location",
    contact: "Contact",
    profile: "Profile",
    login: "Log in"
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
      category: "Photography"
    },
    'edit-suite': {
      category: "Post-production"
    },
    'content-room': {
      category: "Podcast & content"
    },
    'chroma-stage': {
      category: "Film & green screen"
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
    errors: {
      name: "Enter the name on your card (at least 3 letters)",
      number: "Enter a valid Visa or Mastercard number (16 digits)",
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
    viewProfile: "View My Profile",
    bookingNumber: "Booking number",
    paidWith: "Paid with"
  },
  profile: {
    title: "My profile",
    edit: "Edit Profile",
    personal: "Personal information",
    fullName: "Full name",
    phoneNumber: "Phone number",
    save: "Save changes",
    upcoming: "Upcoming booking",
    saved: "Profile saved",
    viewStudio: "View studio",
    logout: "Log out",
    noUpcoming: "You have no upcoming bookings yet.",
    bookStudio: "Book a studio",
    allBookings: "Your bookings"
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
    openMaps: "Open in Google Maps"
  },
  errors: {
    NETWORK_ERROR: "We can't reach the server right now. Check your connection and try again.",
    UNKNOWN_ERROR: "Something went wrong. Please try again.",
    VALIDATION_ERROR: "Some details are missing or not valid.",
    STUDIO_NOT_FOUND: "This studio is not available right now.",
    SLOT_TAKEN: "Sorry, someone just booked these hours. Please pick another time.",
    PAST_TIME: "This time has already passed. Please pick a later time.",
    TOO_FAR_AHEAD: "Bookings can be made up to one year ahead.",
    UNAUTHORIZED: "Please log in to continue.",
    FORBIDDEN: "You don't have access to this page.",
    NOT_FOUND: "We couldn't find what you're looking for.",
    TOO_MANY_REQUESTS: "Too many attempts. Please wait a minute and try again.",
    INTERNAL_ERROR: "Something went wrong on our side. Please try again.",
    EMAIL_TAKEN: "An account with this email already exists. Try logging in.",
    INVALID_CREDENTIALS: "Email or password is incorrect.",
    BOOKING_NOT_FOUND: "We couldn't find this booking.",
    STUDIO_INACTIVE: "This studio is not taking bookings right now.",
    PAYMENT_DECLINED: "Your card was declined. Nothing was charged. Try another card.",
    INSUFFICIENT_FUNDS: "Your card has insufficient funds. Nothing was charged.",
    PAYMENT_FAILED: "The payment could not be completed. Nothing was charged. Please try again.",
    REFUND_FAILED: "The refund could not be completed. Please contact us."
  },
  fieldErrors: {
    INVALID: "This value is not valid.",
    REQUIRED: "This field is required.",
    TOO_LONG: "This is too long.",
    EMAIL_INVALID: "Enter a valid email address.",
    PHONE_INVALID: "Enter an Omani mobile number: 8 digits starting with 7 or 9.",
    PASSWORD_WEAK: "Use at least 8 characters with letters and numbers.",
    NAME_TOO_SHORT: "Enter at least 3 letters."
  },
  status: {
    Upcoming: "Upcoming",
    InProgress: "In progress",
    Completed: "Completed",
    Cancelled: "Cancelled"
  }
};
