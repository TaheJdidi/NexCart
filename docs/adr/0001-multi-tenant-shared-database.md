# One shared platform for all Stores, scoped by Store

NexCart runs as a single application and database that hosts every Store, with each Store's data separated by its Store, rather than one deployment and database per Merchant. We chose this because the goal is a Shopify-style platform where creating a Store is instant and a Merchant can run several Stores from one login; per-store deployments would make the domain simpler but push the complexity into provisioning, upgrading and paying for many copies.

## Consequences

- Every query on Store-owned data must be scoped to one Store; forgetting this leaks one Merchant's data to another, so the scoping must be enforced centrally, not remembered per query.
- A Store's site is found by its Store Handle in the subdomain, so the request pipeline resolves the current Store before anything else runs.
- Removing a Store's data or moving one Store elsewhere is a data operation, not a matter of deleting a database.
