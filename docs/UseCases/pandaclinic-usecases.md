# PANDACLINIC – Use Case Overview (UML-style)

```mermaid
flowchart LR
    %% Actors
    Customer([<<actor>>\nCustomer])
    Admin([<<actor>>\nAdmin/Staff])

    %% Customer use cases
    UC_Browse(((Browse Products)))
    UC_Cart(((Manage Cart / Checkout)))
    UC_Orders(((View Orders)))
    UC_Animals(((Manage Animals)))
    UC_Appt(((Book Appointment)))
    UC_Host(((Book Hosting)))

    %% Admin use cases
    UC_ProductAdmin(((Manage Products)))
    UC_OrderAdmin(((Manage Orders/Payments)))
    UC_AnimalAdmin(((Manage Animals)))
    UC_ApptAdmin(((Manage Appointments)))
    UC_HostAdmin(((Manage Hosting)))
    UC_KPIs(((View KPIs & Reports)))

    %% Relationships
    Customer --- UC_Browse
    Customer --- UC_Cart
    Customer --- UC_Orders
    Customer --- UC_Animals
    Customer --- UC_Appt
    Customer --- UC_Host

    Admin --- UC_ProductAdmin
    Admin --- UC_OrderAdmin
    Admin --- UC_AnimalAdmin
    Admin --- UC_ApptAdmin
    Admin --- UC_HostAdmin
    Admin --- UC_KPIs

    %% Extensions / includes
    UC_Cart --> UC_Orders:::dashed
    UC_Host -. includes .-> UC_Animals
    UC_Appt -. includes .-> UC_Animals

    classDef dashed stroke-dasharray: 5 5
```

## Brief Descriptions
- **Browse Products**: Customer filters/searches products, views details and availability.
- **Manage Cart / Checkout**: Add/update/remove items, place order, select payment method.
- **View Orders**: Track order status and history.
- **Manage Animals**: Register pets, upload photo, view pet profile.
- **Book Appointment**: Schedule vet appointment; avoid slot conflicts.
- **Book Hosting**: Reserve boarding stay; check room availability; optional file upload for pet info.
- **Manage Products**: Admin CRUD products, pricing, stock, activation, images, archive/restore.
- **Manage Orders/Payments**: Admin reviews orders, updates status, handles payment follow-up.
- **Manage Animals / Appointments / Hosting**: Admin CRUD, restore soft-deleted records, resolve conflicts.
- **View KPIs & Reports**: Dashboard totals (animals, appointments, hosting, products, orders, revenue, payments, users).
