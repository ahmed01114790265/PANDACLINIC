# PANDACLINIC – Requirements (Draft)

## 1. Overview
- Purpose: Manage a pet clinic with unified experiences for customers (public web) and staff/admins (dashboard).
- Architecture: Clean architecture / DDD-style layers (Application, Domain, Persistence, Web, Dashboard, Shared, ExternalServices).
- Platforms: ASP.NET Core 9 Web (customer) + ASP.NET Core Dashboard (admin).

## 2. Stakeholders & Roles
- Customer (pet owner): browse products, book appointments, book hosting, manage animals, place orders, checkout/pay.
- Admin/Staff: manage animals, appointments, hosting stays, products, orders, payments, file storage.
- System Integrations: file storage (uploads), localization (currently EN), email/SMS gateways TBD.

## 3. Core Functional Requirements
### Customer Web
- Products: list/filter/search, view details, images, pricing, stock status.
- Cart & Orders: add/update cart items, checkout, create order with payment method, view order status.
- Animals: create/manage owned animals; view details and history.
- Appointments: book appointments for an animal; prevent overlaps; view upcoming and past appointments.
- Hosting (boarding): request/ book hosting stays; show availability/room allocation; view current/previous stays.
- Account: login/register, profile basics, localized UI (EN), file uploads for animal images and product images.
- File serving: serve uploaded images from `/uploads` (configurable path).

### Admin Dashboard
- KPIs: totals for animals, appointments (today/ scheduled), hosting (ongoing/total), products (active/low stock), orders (total/pending/completed), revenue, payments needing attention, total users.
- Products: create/edit/delete/archive/restore, upload images, set pricing/discounts/stock/activation.
- Orders: list, view details, update status, restore archived orders.
- Animals: CRUD, delete/restore, view hosting/appointment links.
- Appointments: CRUD, conflict checks, status updates, restore deleted.
- Hosting: CRUD, room availability check, status updates, restore deleted stays.
- Users: count/reporting (read-only at present).
- File management: uploads for products/animals; deletion of replaced files.

## 4. Non‑Functional Requirements
- Security: role-based authorization (Admin, Staff, User); auth cookies with 60‑minute lifetime; anti-forgery on mutations.
- Localization: infrastructure wired; resources present for EN (primary); AR/Future locales should be supported via resx.
- Performance: paginate lists (page/size); async data access; image CDN-path configurable.
- Reliability: soft-delete with restore (animals, products, appointments, hostings, orders).
- Maintainability: clean layering; DI everywhere; mapping via Mapster; Result<T> pattern for service responses.
- Observability: logging via ILogger<>; dashboard timestamps on KPI responses.

## 5. Data & Domain Highlights
- Entities: Animal, Appointment, HostingStay, Product, Order, OrderItem, Payment, ApplicationUser, Money value object.
- Status enums: AppointmentStatus, HostingStayStatus, OrderStatus, PaymentStatus, ProductType.
- Soft-delete fields: IsDeleted, DeletedAt used across major entities with repository helpers.
- File storage: configured via `FileStorage:Path`; paths stored as URLs under `/uploads/...`.

## 6. External Interfaces
- Database: EF Core (SQL Server assumed) via Persistence layer, repositories + UnitOfWork.
- File storage: local file system path from configuration; PhysicalFileProvider serves `/uploads`.
- Payment: stubbed Payment entity; no gateway integration yet (extension point).

## 7. Configuration
- `FileStorage:Path` for upload root (required for correct static files serving).
- `appsettings.*` for DB connection strings, Identity cookie settings, localization cultures.

## 8. Known Gaps / To‑Do
- Full AR localization content completion and switcher UI.
- Payment gateway integration and webhooks.
- Email/SMS notifications for bookings/orders.
- Audit trails and more granular user management.
- Validation & error messaging localization.

## 9. Acceptance Criteria (sample)
- Customer can create an account, add an animal, book an appointment, add a hosting stay, purchase a product, and view the order in dashboard with correct statuses.
- Admin can create/edit product with image upload; low-stock badge shows when stock ≤ 5.
- Dashboard KPIs render with live counts and last-4-week revenue chart without errors.
- Uploaded images are accessible via `/uploads/...` and cleaned up on replacement.
- All projects build successfully (`dotnet build`) with zero errors.
