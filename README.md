# Frame — hourly content-studio booking (Muscat)

Customers browse studios, pick hours on a calendar, pay by card and get an invoice by email.
Admins manage studios, see every booking, cancel with an automatic refund, and follow revenue and occupancy.

**Stack:** ASP.NET Core (.NET 10) · EF Core 10 · SQL Server 2022 · Clean Architecture · JWT · FluentValidation · Serilog · MailKit · Docker · plain HTML/CSS/JS frontends served by nginx.

---

## Run everything (one command)

Requirements: Docker Desktop.

```bash
cp .env.example .env      # then fill in DB_PASSWORD, JWT_KEY (32+ random chars), ADMIN_PASSWORD
docker compose up -d --build
```

| Service | URL |
|---|---|
| Customer website | http://localhost:4200 |
| Admin panel | http://localhost:4201 |
| API + Swagger | http://localhost:8080/swagger |
| Mailpit (all sent emails) | http://localhost:8025 |
| SQL Server | localhost,1434 (user `sa`) |

On first start the API applies migrations and seeds 4 studios and the admin account from `.env`.

### Test payment tokens

The payment gateway is a fake: the result is read from the token.

| Token | Result |
|---|---|
| `tok_ok_visa_4242` / `tok_ok_mastercard_4444` | Paid |
| `tok_declined_visa_0002` | 402 `PAYMENT_DECLINED` |
| `tok_nofunds_visa_9995` | 402 `INSUFFICIENT_FUNDS` |
| anything else | 402 `PAYMENT_FAILED` |

---

## Local development (without Docker for the API)

```bash
docker compose up -d frame-db frame-mail       # database + mail only
cd backend
dotnet user-secrets set "ConnectionStrings:Default" "Server=localhost,1434;Database=FrameDb;User Id=sa;Password=...;TrustServerCertificate=True" --project Frame.Api
dotnet user-secrets set "Jwt:Key" "<32+ random chars>" --project Frame.Api
dotnet user-secrets set "Admin:Email" "admin@gmail.om" --project Frame.Api
dotnet user-secrets set "Admin:Password" "<password>" --project Frame.Api
dotnet user-secrets set "Admin:FullName" "Frame Admin" --project Frame.Api
dotnet run --project Frame.Api                  # http://localhost:8080/swagger
```

Secrets live only in user-secrets (local) and `.env` (Docker). Neither is committed.

---

## Architecture

```
Frame.Domain          Entities with their own rules (Booking, Studio, User, EmailMessage), value objects. No dependencies.
Frame.Application     Use cases (services), DTOs, validators, interfaces (repositories, clock, payment, email).
Frame.Infrastructure  EF Core, repositories, SQL sequence, JWT, password hashing, fake payment gateway, SMTP, outbox worker.
Frame.Api             Controllers, exception middleware, validation filter, auth policies, CORS, rate limiting, Swagger.
```

Dependencies point inward: Api → Application → Domain, and Infrastructure implements Application's interfaces.
Swapping the fake gateway for a real provider, or Mailpit for a real SMTP server, changes one registration line.

---

## Key design decisions

**No double booking.**
1. A fast clash check before charging (most conflicts stop here, nothing charged).
2. Charge the total computed on the server (the client never sends a price).
3. Re-check and insert inside a **Serializable** transaction, which locks the studio/day range.
4. If anything fails after the charge (lost race, deadlock, database error) the payment is **refunded at once** and the customer gets 409 `SLOT_TAKEN`.

**Every booking gets its email (outbox).**
The confirmation email is saved in the same transaction as the booking. A background worker sends pending emails every 10 seconds, retries up to 3 times, and saves after each email so nothing is sent twice. Emails are in the customer's language (Arabic RTL or English) and include the invoice.

**Money.**
Prices in OMR with 3 decimals (`decimal(10,3)`). Price = hours × hourly rate + 5% VAT. Each booking stores a snapshot of its price, so changing a studio price never changes old invoices. Admin cancellation validates first (not started, not already cancelled), then refunds, then saves; a failed refund changes nothing.

**Time.**
Muscat is UTC+4 with no daylight saving. One `IClock` is the only source of "now" and the only place that knows the offset. Business rules use Muscat time; timestamps are stored in UTC and returned with `+04:00`.

**Security.**
- JWT (HMAC-SHA256, 8 h) with separate audiences: a customer token cannot call admin endpoints, and the reverse (403).
- Admin controllers inherit one base class that is protected by default.
- The customer always comes from the token; another customer's booking returns 404, so ids cannot be probed.
- Login rate limit: 5 attempts per minute per IP (429). Same error for a wrong email or a wrong password.
- Card number and CVV never reach the server: only a gateway token, the brand and the last 4 digits.
- Passwords hashed (ASP.NET Identity hasher). Passwords, tokens and card data are never logged.
- User-typed text is HTML-encoded in emails. The API container runs as a non-root user.

**Errors.**
Every error has one JSON shape `{ status, code, message, errors, traceId }`. Validation returns per-field codes (`REQUIRED`, `HOURS_INVALID`, ...). Unexpected errors become a logged 500; the API never crashes on a bad request.

---

## Main endpoints

| Area | Endpoints |
|---|---|
| Auth | `POST /api/auth/register` · `POST /api/auth/login` · `GET /api/auth/me` · `POST /api/admin/auth/login` |
| Studios | `GET /api/studios` · `GET /api/studios/{id}` · `GET /api/studios/{id}/days?month=` · `GET /api/studios/{id}/availability?date=` |
| Bookings | `POST /api/bookings/quote` · `POST /api/bookings` · `GET /api/bookings/my` · `GET /api/bookings/{id}` |
| Admin | `GET /api/admin/dashboard` · `GET/POST /api/admin/studios` · `PUT /api/admin/studios/{id}` · `PATCH /api/admin/studios/{id}/status` · `GET /api/admin/bookings` · `GET /api/admin/bookings/{id}` · `POST /api/admin/bookings/{id}/cancel` |

Full request and response examples are in Swagger.

---

## Repository layout

```
backend/   .NET solution (Frame.slnx) + Dockerfile
web/       customer website (static files, served by nginx)
admin/     admin panel (static files, served by nginx)
docker-compose.yml · .env.example
```
