# Customers belong to one Store; Merchants and Couriers share a NexCart Account

A Customer exists only within the Store they buy from: the same person shopping at two Stores is two unrelated Customers, and Store A never learns they shop at Store B. Merchants, Couriers and Platform Admins instead sign in with one platform-wide NexCart Account that can hold several roles, because a Courier delivers for many Stores and a Merchant can run several. We chose per-Store Customers, as Shopify does, so each Merchant owns their customer list and Customers' privacy is kept between Stores.

## Considered Options

- **One platform-wide account for everyone, Customers included**: a single login across all Stores, but it makes NexCart (not the Merchant) the owner of the customer relationship and exposes cross-Store shopping.

## Consequences

- There are two separate kinds of identity: NexCart Accounts (platform-level) and Customer accounts (per Store). An email address can exist once per Store as a Customer and separately as a NexCart Account.
