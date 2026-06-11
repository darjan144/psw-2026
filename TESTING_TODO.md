# Demo Testing TODO — Manual Checklist

You test. Claude watches, documents bugs in `BUGS.md`, fixes them.
Check off `[x]` as you go. Report anything unexpected immediately.

**Accounts:** admin/Admin123!, guide1/Guide123!, guide2/Guide123!, guide3/Guide123!
**URLs:** Frontend http://localhost:4200 · Swagger http://localhost:5106/swagger

---

## Session 0 — Pre-Flight

- [ ] Drop database `tour_management` (pgAdmin or `psql -U postgres -c "DROP DATABASE tour_management;"`)
- [ ] Recreate: from `TourManagement.API/` run `dotnet ef database update --project TourManagement.Infrastructure --startup-project TourManagement.API`
- [ ] Start API: `dotnet run` in `TourManagement.API/TourManagement.API` (seeds admin + 3 guides)
- [ ] Start frontend: `npx ng serve` in `TourManagement.Client`
- [ ] Open http://localhost:4200 — login page loads, no console errors (F12)

---

## Session 1 — Registration & Login

- [ ] **1.1** Go to /register. Fill: username `tourist1`, password (6+ chars), confirm, first/last name, YOUR REAL EMAIL, check interests **Nature + Food**, recommendations **ON**. Register.
  - Expect: success toast, redirect to login.
- [ ] **1.2** Register `tourist2`, different email (or alias like `you+t2@gmail.com`), interest **Art** only, recommendations **OFF**.
- [ ] **1.3** Edge: try mismatched passwords → submit button stays disabled.
- [ ] **1.4** Edge: register with username `tourist1` again → error toast.
- [ ] **1.5** Edge: register with tourist1's email, new username → error toast.
- [ ] **1.6** Edge: password `abc` (under 6 chars) → blocked.
- [ ] **1.7** Login `tourist1` → lands on /tours. Navbar: Tours, Cart, My Tours, Profile, My Problems + username.
- [ ] **1.8** While logged in, manually type /login in URL bar → redirected away (guard works).
- [ ] **1.9** Logout. Login `guide1` → lands on /guide/tours. Navbar: My Tours, Substitutes, Problems.
- [ ] **1.10** Logout. Login `admin` → lands on /admin/blocked-users. Navbar: Blocked Users, Problems.

---

## Session 2 — Guide: Create Tour, Key Points, Publish

- [ ] **2.1** As guide1: My Tours → Create Tour. Fill: name "Fruška Gora Hike", description, difficulty Medium, category **Nature**, price **100**, date = ~2 weeks from now. Create.
  - Expect: redirect, tour in list with **Draft** badge.
- [ ] **2.2** Open the tour. Click on the map → key point form shows lat/lng. Fill name + description, choose JPG/PNG image (under 5 MB). Add Key Point.
  - Expect: brief "Uploading...", point appears in list with thumbnail + marker on map.
- [ ] **2.3** Edge: try a non-image file (.txt/.exe) → rejected with error.
- [ ] **2.4** Edge: file over 5 MB → rejected.
- [ ] **2.5** Edge: form without image → Add button disabled.
- [ ] **2.6** With only 1 key point: confirm **Publish button is NOT shown**.
- [ ] **2.7** Add a 2nd key point → polyline drawn between points. Publish button appears. Click Publish.
  - Expect: **Published** badge, map no longer clickable, Publish button gone.
- [ ] **2.8** 📧 Check tourist1's inbox (Nature interest, recommendations ON) → recommendation email arrived.
- [ ] **2.9** 📧 Check tourist2's inbox (Art, recommendations OFF) → NO email.
- [ ] **2.10** Create a 2nd tour "City Art Walk", category **Art**, price **150**, date ~3 weeks out, add 2 key points, publish.
  - Expect: tourist2 gets NO email (recommendations off) — confirms OFF path.
- [ ] **2.11** Create a 3rd tour, leave it **Draft** (no publish).

---

## Session 3 — Guide: View My Tours

- [ ] **3.1** My Tours list: all 3 tours, correct badges (2 Published, 1 Draft), all info shown (difficulty, category, price, date, key point count).
- [ ] **3.2** Click sort toggle → order flips by date (asc/desc). Verify visually.
- [ ] **3.3** Open a published tour → map drawn with markers + polyline.

---

## Session 4 — Tourist: Browse, Cart, Purchase

- [ ] **4.1** Login `tourist1`. /tours shows ONLY the 2 published tours (Draft invisible).
- [ ] **4.2** Open a tour card → detail page: full info, map with markers + polyline, key points list, reviews section (empty).
- [ ] **4.3** Add both tours to cart. Open Cart → 2 items, prices listed, total = **250**.
- [ ] **4.4** Remove "City Art Walk" → total = **100**. Re-add it.
- [ ] **4.5** Purchase (no bonus points yet).
  - Expect: success toast, cart empties.
- [ ] **4.6** 📧 Check tourist1 inbox → purchase confirmation email with info about BOTH tours.
- [ ] **4.7** Edge: try adding an already-purchased tour to cart and purchasing again → backend rejects.
- [ ] **4.8** My Tours (tourist) → both purchases listed with price paid, dates.

---

## Session 5 — Bonus Points & Auto-Cancellation

- [ ] **5.1** As guide1: create + publish "Cancel Test Tour", price **100**, date ~5 days out (2 key points needed).
- [ ] **5.2** As tourist1: purchase it (alone in cart).
- [ ] **5.3** As guide1: open the tour → **Seek Substitute** → "Seeking Substitute" badge appears.
- [ ] **5.4** SQL — pull tour inside the 24h window (run in psql/pgAdmin):
  ```sql
  UPDATE "Tours" SET "ScheduledDate" = NOW() + INTERVAL '20 hours'
  WHERE "Name" = 'Cancel Test Tour';
  ```
- [ ] **5.5** Restart the API (background service checks every 30 min; restart triggers a fresh cycle). Wait ~1 min.
- [ ] **5.6** As tourist1: Profile → **bonus points = 100**.
- [ ] **5.7** Tour no longer purchasable/listed as active (check /tours).
- [ ] **5.8** As tourist1: add "City Art Walk"-priced tour… (already purchased — instead have guide1 publish a new tour, price **150**). Add to cart, check **Use bonus points** → total shows **50** with original struck through.
- [ ] **5.9** Purchase → Profile shows bonus points **0**.
- [ ] **5.10** Edge (optional): repeat with bonus > cart total → price floors at 0, leftover points remain.

---

## Session 6 — Substitute Flow

- [ ] **6.1** As guide3: create + publish a tour on date **X** (e.g. 10 days out). This creates the conflict for 6.5.
- [ ] **6.2** As guide1: create + publish "Substitute Test Tour" on the SAME date X. Click **Seek Substitute**.
- [ ] **6.3** As guide2: Substitutes page → tour listed. Click **Accept**.
  - Expect: tour disappears from list, success toast.
- [ ] **6.4** guide2 My Tours → tour now belongs to guide2. guide1 My Tours → tour gone.
- [ ] **6.5** Edge: as guide1, seek substitute on another tour dated X… then as guide3 (has own tour on X) try Accept → backend rejects with error.

---

## Session 7 — 48h Reminder Email

- [ ] **7.1** SQL — move a tour tourist1 purchased into the 48h window:
  ```sql
  UPDATE "Tours" SET "ScheduledDate" = NOW() + INTERVAL '40 hours'
  WHERE "Name" = 'Fruška Gora Hike';
  ```
- [ ] **7.2** Restart API. Wait ~1 min.
- [ ] **7.3** 📧 tourist1 inbox → reminder email with tour info.
- [ ] **7.4** ⚠ Watch item: leave API running 1+ hour OR restart again → check NO duplicate reminder arrives. Report result to Claude.

---

## Session 8 — Reviews

- [ ] **8.1** SQL — put a purchased tour in the past:
  ```sql
  UPDATE "Tours" SET "ScheduledDate" = NOW() - INTERVAL '1 day'
  WHERE "Name" = 'Fruška Gora Hike';
  ```
- [ ] **8.2** As tourist1: open that tour's detail → review form visible.
- [ ] **8.3** Select rating **2**, leave comment empty → submit blocked / error.
- [ ] **8.4** Add comment with rating 2 → submits OK. Review appears, form disappears.
- [ ] **8.5** On "City Art Walk" (purchased): set date to yesterday (SQL), rating **4**, NO comment → accepted.
- [ ] **8.6** Edge: tour NOT purchased (login tourist2) → no review form.
- [ ] **8.7** Edge SQL — 8 days past:
  ```sql
  UPDATE "Tours" SET "ScheduledDate" = NOW() - INTERVAL '8 days'
  WHERE "Name" = 'City Art Walk';
  ```
  As tourist1 (if not yet reviewed, use a fresh purchase) → no review form.
- [ ] **8.8** Edge: already-reviewed tour → no form.

---

## Session 9 — Profile & Recommendations Toggle

- [ ] **9.1** As tourist1: Profile → username + bonus points shown read-only, interests + recommendations editable.
- [ ] **9.2** Change interests (add Art), save → success toast. Reload page → persisted.
- [ ] **9.3** Turn recommendations **OFF**, save.
- [ ] **9.4** As guide1: publish a new **Art** tour.
- [ ] **9.5** 📧 tourist1 gets NO email (recommendations off). Turn back ON, publish another matching tour → email arrives.

---

## Session 10 — Problems & Event Sourcing

- [ ] **10.1** As tourist1: report a problem (title + description) on a purchased tour.
  - ⚠ Watch item: check WHERE the Report Problem button is and whether it requires the tour date to have passed. Spec allows reporting on ANY purchased tour. Report findings to Claude.
- [ ] **10.2** Expect: problem created, status **Pending**.
- [ ] **10.3** 📧 guide's email (guide1@psw.test — check server logs if unreal) → problem notification.
- [ ] **10.4** My Problems (tourist) → problem listed, Pending badge. Expand history → ProblemCreated event with timestamp.
- [ ] **10.5** Report a 2nd problem on another purchased tour.
- [ ] **10.6** As guide1: Problems page → both problems visible. Problem #1 → **Resolve** → status Resolved.
- [ ] **10.7** Problem #2 → **Send to Review** → status InReview.
- [ ] **10.8** As admin: Problems page → problem #2 listed. **Return to Guide** → back to Pending.
- [ ] **10.9** As guide1: problem #2 again Pending → **Send to Review** again.
- [ ] **10.10** As admin: **Reject** → status Rejected.
- [ ] **10.11** Expand event history on problem #2 → full ordered timeline: Created → SentToReview → ReturnedToGuide → SentToReview → Rejected, all timestamped.
- [ ] **10.12** As tourist1: My Problems → sees current statuses (Resolved, Rejected).

---

## Session 11 — Blocked Users (Admin)

- [ ] **11.1** Logout. Try login `tourist2` with WRONG password **5 times**.
- [ ] **11.2** 6th attempt with CORRECT password → still refused ("blocked").
- [ ] **11.3** As admin: Blocked Users → tourist2 listed, block count **1**. Click **Unblock**.
- [ ] **11.4** tourist2 logs in OK with correct password.
- [ ] **11.5** Repeat wrong-password ×5 two more times (unblock between rounds) → block count reaches **3** → **Unblock button disabled**, "permanently blocked" shown.
- [ ] **11.6** Note for Claude: what happens after 4 wrong + 1 correct? (counter should reset on success — verify tourist1 not blocked after partial failures).

---

## Session 12 — Final Sweep

- [ ] **12.1** Claude runs `dotnet test` → all green.
- [ ] **12.2** Swagger as tourist token: call a guide endpoint (e.g. POST /api/tour) → 403.
- [ ] **12.3** Remove token from localStorage (F12 → Application) → next action redirects to /login.
- [ ] **12.4** Review BUGS.md with Claude → everything fixed or consciously accepted.

---

## Bug Reporting Format (tell Claude)

When something breaks, say: **session step + what you did + what you expected + what happened** (paste error/console output if any). Claude logs it in `BUGS.md` and fixes it.
