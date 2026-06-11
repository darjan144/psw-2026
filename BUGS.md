# Bug Log — Demo Testing Phase

Format: ID | Session/Step | Severity | Status

## BUG-001 — Duplicate purchase silently "succeeds" with empty confirmation email
- **Found:** Session 4, step 4.7
- **Severity:** major
- **Repro:** Purchase tour A. Add tour A to cart again. Purchase.
- **Expected:** Backend rejects purchase ("already purchased").
- **Actual:** Purchase succeeds, cart cleared, confirmation email sent with NO tours listed but "Ukupno plaćeno: 100.00 RSD" (total computed from skipped item). Bonus points would also be wrongly consumed.
- **Cause:** `PurchaseToursCommandHandler` skips already-purchased items via `continue` instead of throwing; total/email still computed from full cart.
- **Fix:** Purchase handler validates all cart items upfront, throws "already purchased" before bonus deduction/email. AddToCart also rejects already-purchased tours. New tests added (duplicate → throw; bonus untouched on failure; add-to-cart rejection).
- **Status:** fixed (pending live re-test 4.7)

## BUG-002 — Cancelled tour shown as normal purchase; detail page stuck on "Loading tour"
- **Found:** Session 5 (after auto-cancellation)
- **Severity:** major (UX/correctness)
- **Repro:** Tour auto-cancelled (bonus points awarded). Tourist opens My Tours.
- **Expected:** Purchase marked as Cancelled; no broken navigation.
- **Actual:** Tour listed as if upcoming; clicking it → permanent "Loading tour" (detail looks up published-tours cache only).
- **Cause:** `TouristPurchaseDto` has no tour status; `tour-detail.component.ts` has no not-found fallback.
- **Fix:** `TourStatus` added to purchase DTO (+ test). My Tours shows red "Cancelled" badge, name unlinked, Report Problem hidden for cancelled tours. Tour detail shows "no longer available" fallback instead of infinite loading.
- **Status:** fixed (pending live re-test)

## BUG-003 — Report Problem only available for past tours
- **Found:** Session 10, step 10.1 (predicted watch item #1)
- **Severity:** minor (spec deviation)
- **Repro:** My Tours as tourist — upcoming purchased tour has no Report Problem button.
- **Expected:** Spec allows reporting on ANY purchased tour ("na turama koje je prethodno kupio").
- **Actual:** Button gated on scheduled date having passed (frontend only; backend had no date check).
- **Fix:** Date check removed from `canReportProblem` in `my-tours.component.ts`; still hidden for cancelled tours.
- **Status:** fixed (pending live re-test)

---

<!-- Template:
## BUG-001 — short title
- **Found:** Session X, step X.Y
- **Severity:** critical / major / minor
- **Repro:** ...
- **Expected:** ...
- **Actual:** ...
- **Status:** open / fixed (commit) / accepted
-->
