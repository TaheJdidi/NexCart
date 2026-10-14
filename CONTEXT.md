# NexCart

NexCart is a multi-tenant e-commerce platform: Merchants create Stores on it, and each Store gets its own shop site where Customers buy.

## Language

### Platform

**Store**:
One Merchant's shop on the platform, with its own products, orders, settings, currency and site at its own subdomain.
_Avoid_: Shop, tenant (in domain discussions), site

**Suspended**:
The state of a Store taken offline by a Platform Admin: its site is unavailable and its admin area is read-only, but nothing is deleted.
_Avoid_: Banned, disabled, blocked

**Store Handle**:
The unique, permanent short name of a Store that forms its subdomain (e.g. `ahmed-shoes`).
_Avoid_: Slug, subdomain (for the name itself), store name

**NexCart Account**:
A platform-wide login that can hold several roles at once: Merchant of some Stores, Courier for others, or Platform Admin.
_Avoid_: User, profile, member

**Merchant**:
A role in which a NexCart Account owns and runs one or more Stores through their admin area.
_Avoid_: Seller, vendor, store owner, user

**Customer**:
A person who browses a Store's site and places Orders on it; a Customer belongs to exactly one Store and has no NexCart Account.
_Avoid_: Buyer, client, shopper, user

**Guest Customer**:
A Customer who places an Order without creating an account.
_Avoid_: Anonymous user

**Platform Admin**:
A person who operates NexCart itself and oversees all Stores.
_Avoid_: Superadmin, admin (unqualified)

### Catalog

**Product**:
Something a Store sells, shown to Customers once Published.
_Avoid_: Item, article

**Variant**:
One buyable version of a Product (e.g. size M, blue), with its own SKU, price and stock; every Product has at least one.
_Avoid_: Option, SKU (as the thing itself), model

**Category**:
A node in a Store's tree of product groupings; each Product sits in one Category.
_Avoid_: Collection, section, tag

**Draft** / **Published** / **Archived**:
The states of a Product: visible only to the Merchant, visible to Customers, and hidden but kept for past Orders.
_Avoid_: Inactive, deleted, hidden

**Stock**:
How many units of a Variant the Store can still sell; taken when an Order is Placed and returned when it is Cancelled.
_Avoid_: Quantity, inventory (for the count itself)

### Ordering

**Order**:
A Customer's request to buy Variants from one Store, delivered to one Delivery Address.
_Avoid_: Commande, cmd, purchase

**Placed** / **Confirmed** / **Out for Delivery** / **Delivered** / **Cancelled**:
The states of an Order: submitted by the Customer, checked by the Merchant (usually by phone), with a Courier, handed over, or abandoned.
_Avoid_: Pending, new, shipped, completed

**Order Line**:
One Variant and quantity in an Order, keeping the product name, Variant and price as they were when the Order was Placed.
_Avoid_: Order item, cart line

**Order Tracking Link**:
A private link given to the Customer when an Order is Placed, through which anyone holding it can follow that Order's state.
_Avoid_: Tracking number, receipt link

**Delivery Address**:
Where and to whom an Order is delivered: full name, phone, governorate, city, address line and an optional note.
_Avoid_: Shipping address

**Delivery Fee**:
A flat amount a Store adds to every Order to cover delivery.
_Avoid_: Shipping cost, shipping fee

**Cash on Delivery**:
A payment method where the Customer pays the Courier in cash when the Order is handed over.
_Avoid_: COD (in UI text), cash payment

### Delivery

**Courier**:
A role in which a NexCart Account delivers Orders for one or more Stores that have invited it; a Store can work with many Couriers.
_Avoid_: Livreur, driver, delivery guy

**Courier Invitation**:
A Merchant's request, sent by email, for a Courier to start delivering for their Store.
_Avoid_: Courier request, link

**Delivery**:
One attempt to carry a Confirmed Order to its Customer, by one Courier; a failed attempt leaves the Order Confirmed for a new Delivery.
_Avoid_: Shipment, assignment, notification

**Offered** / **Accepted** / **Declined** / **Withdrawn** / **Picked Up** / **Delivered** / **Failed**:
The states of a Delivery: waiting for the Courier's answer, taken, refused, taken back by the Merchant, collected from the Store, handed over, or not handed over (with a reason).
_Avoid_: Pending, assigned, in transit, returned

**Collected Cash**:
The amount a Courier confirms receiving from the Customer when marking a Delivery as Delivered.
_Avoid_: Payment, COD amount
