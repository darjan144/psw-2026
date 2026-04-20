# Manual Testing Guide

## Prerequisites

1. **PostgreSQL** running on `localhost:5432` with a database named `tour_management`
2. **SMTP credentials** configured in `appsettings.Development.json` (Email section) for purchase/reminder/recommendation emails
3. **.NET 8 SDK** and **Node.js 18+** installed

## Starting the Application

```bash
# Terminal 1 — Backend (seeds admin + 3 guides on first run)
cd TourManagement.API/TourManagement.API
dotnet run

# Terminal 2 — Frontend
cd TourManagement.Client
npx ng serve
```

- Backend: http://localhost:5106/swagger
- Frontend: http://localhost:4200

## Seeded Accounts

| Username | Password   | Role          |
|----------|------------|---------------|
| admin    | Admin123!  | Administrator |
| guide1   | Guide123!  | Guide         |
| guide2   | Guide123!  | Guide         |
| guide3   | Guide123!  | Guide         |

Tourists are created via the Register page.

---

## Test Scenarios

### 1. Registration (Tourist)

1. Go to http://localhost:4200/register
2. Fill in: username, password (min 6 chars), confirm password, first name, last name, email
3. Check at least one interest (Nature, Art, Sport, Shopping, Food)
4. Toggle "Receive recommendations" on or off
5. Click **Register**
6. **Expected:** toast notification, redirect to login page

**Edge cases:**
- Mismatched passwords — submit button stays disabled
- Duplicate username — backend returns error
- Already logged in — guard redirects away from register page

---

### 2. Login

1. Go to http://localhost:4200/login
2. Enter valid credentials (e.g. the tourist you just registered)
3. **Expected:** redirect to `/tours` (Tourist), `/guide/tours` (Guide), or `/admin/blocked-users` (Admin)
4. Nav bar shows username, role, and role-appropriate links

**Edge cases:**
- Wrong password 5 times — account gets temporarily blocked
- Blocked user cannot log in until admin unblocks them

---

### 3. Guide: Create Tour

1. Log in as `guide1` / `Guide123!`
2. Nav bar shows: **My Tours**, **Substitutes**, **Problems**
3. Click **My Tours** — empty list initially
4. Click **Create Tour**
5. Fill in: name, description, difficulty (Easy/Medium/Hard), category (Nature/Art/Sport/Shopping/Food), price, scheduled date (pick a future date)
6. Click **Create**
7. **Expected:** redirect to tour list, new tour appears with "Draft" badge

---

### 4. Guide: Add Key Points with Image Upload

1. From My Tours, click on the draft tour
2. Tour detail page shows the map (centered on Novi Sad)
3. Click a location on the map — key point form appears showing lat/lng
4. Fill in: name, description
5. Click **Choose File** — select a JPG/PNG image (max 5 MB)
6. File name and size appear below the input
7. Click **Add Key Point**
8. **Expected:** "Uploading..." shown briefly, then key point appears in the list with a thumbnail image, and as a marker on the map
9. Add at least one more key point (need 2+ to publish)
10. **Expected:** polyline drawn between key points on the map

**Edge cases:**
- Non-image file (e.g. .exe) — backend rejects with error
- File over 5 MB — backend rejects
- No file selected — Add Key Point button stays disabled

---

### 5. Guide: Publish Tour

1. On tour detail page with 2+ key points, click **Publish Tour**
2. **Expected:** status badge changes to "Published", map becomes non-editable (no click-to-add), "Publish" button disappears
3. If the tourist who registered has matching interests and recommendations enabled, they receive an email about this new tour

---

### 6. Guide: View My Tours

1. Go to My Tours page
2. Tours listed with name, status badge (Draft/Published), difficulty, category, price, date
3. Click the sort toggle to switch between ascending/descending by date
4. Click a tour to see its detail page with map

---

### 7. Tourist: Browse Published Tours

1. Log in as the tourist you registered
2. Lands on `/tours` — card grid of all published tours
3. Each card shows: name, description, difficulty, category, price, date, number of key points
4. Click **Add to Cart** on a tour card

---

### 8. Tourist: View Tour Detail

1. Click on a tour card to open `/tours/:tourId`
2. **Expected:** full tour info, Leaflet map with key point markers and polyline, key points list
3. Reviews section shown at the bottom (empty initially)

---

### 9. Tourist: Shopping Cart

1. Click **Cart** in the nav bar
2. **Expected:** list of items with tour name, price, and remove button
3. Total price displayed at the bottom
4. Optionally check **Use bonus points** (only relevant if tourist has bonus points from a cancelled tour)
5. Click **Purchase**
6. **Expected:** success toast, cart emptied, confirmation email sent to tourist's email address
7. Email contains basic info about each purchased tour

**Edge cases:**
- Try purchasing the same tour again — backend rejects (duplicate purchase check)
- Remove item from cart — total updates

---

### 10. Tourist: Rate a Purchased Tour

1. Go to `/tours/:tourId` for a tour you purchased **whose scheduled date has passed**
2. Review form appears at the bottom (if within 7 days of the tour date)
3. Select rating 1-5
4. If rating is 1 or 2 — comment field becomes required
5. For rating 3-5 — comment is optional
6. Click **Submit Review**
7. **Expected:** review appears in the list, form disappears

**Edge cases:**
- Tour date hasn't passed yet — no review form shown
- More than 7 days after tour date — no review form shown
- Already reviewed — no review form shown

---

### 11. Tourist: Profile & Interests

1. Click **Profile** in the nav bar
2. Username and email shown (read-only)
3. Interest checkboxes and recommendations toggle (editable)
4. Change interests, click **Save**
5. **Expected:** success toast

---

### 12. Tourist: Report a Problem

1. Go to `/tours/:tourId` for a purchased tour
2. Fill in problem title and description in the problem form
3. Submit
4. **Expected:** problem created with "Pending" status
5. The guide of that tour receives an email notification about the new problem

---

### 13. Tourist: View My Problems

1. Click **My Problems** in the nav bar
2. **Expected:** list of reported problems with status badges (Pending, Resolved, InReview, Rejected)
3. Click a problem to expand and see the event history timeline

---

### 14. Guide: View & Handle Problems

1. Log in as the guide whose tour has a reported problem
2. Click **Problems** in the nav bar
3. **Expected:** problems grouped by tour, each with status
4. For a "Pending" problem, two actions available:
   - **Resolve** — marks it as resolved
   - **Send to Review** — escalates to admin (status becomes "InReview")

---

### 15. Admin: Review Escalated Problems

1. Log in as `admin` / `Admin123!`
2. Click **Problems** in the nav bar
3. **Expected:** list of problems with "InReview" status
4. Two actions per problem:
   - **Return to Guide** — status goes back to "Pending" (guide must handle it)
   - **Reject** — problem is dismissed (status becomes "Rejected")
5. Click to expand event history — shows full timeline of state changes (event sourcing)

---

### 16. Guide: Seek Substitute

1. Log in as `guide1`, go to a published tour's detail page
2. Click **Seek Substitute**
3. **Expected:** "Seeking Substitute" label appears, button disappears

---

### 17. Guide: Accept Substitute

1. Log in as `guide2` (a different guide)
2. Click **Substitutes** in the nav bar
3. **Expected:** the tour seeking a substitute appears in the list (only if guide2 has no tour on the same date)
4. Click **Accept**
5. **Expected:** tour disappears from the list, tour ownership transfers to guide2

**Edge case:** If guide2 already has a tour scheduled on the same date, the backend rejects the assignment.

---

### 18. Automatic Tour Cancellation (24h rule)

This runs as a background service — no UI action needed.

1. Create a tour with a scheduled date that is less than 24h from now
2. Mark it as seeking substitute
3. If no substitute is found before the 24h window, the background service cancels the tour
4. **Expected:** all tourists who purchased the tour receive bonus points equal to the tour price

To test manually: create a tour scheduled for tomorrow, seek substitute, and wait — or adjust the server clock.

---

### 19. 48h Tour Reminder Email

Background service — no UI action needed.

1. Purchase a tour whose date is 48h away
2. **Expected:** system sends a reminder email to the tourist with tour info

To verify: check the tourist's email inbox around the 48h mark, or check server logs.

---

### 20. Tour Recommendation Email

1. Register a tourist with interest "Nature" and recommendations enabled
2. Log in as a guide and publish a tour with category "Nature"
3. **Expected:** the tourist receives an email recommending the new tour

---

### 21. Admin: Blocked Users

1. Attempt to log in with any account using a wrong password **5 times**
2. **Expected:** the account becomes temporarily blocked
3. Log in as `admin` / `Admin123!`
4. Click **Blocked Users** in the nav bar
5. **Expected:** blocked user appears in the table with username, email, and block count
6. Click **Unblock** — user can log in again
7. If a user has been blocked 3 times total, the **Unblock button is disabled** (permanent block)

---

## Bonus Points Flow (End-to-End)

1. Guide1 creates and publishes a tour (price: 100 RSD)
2. Tourist purchases the tour
3. Guide1 seeks substitute, no one accepts
4. Background service cancels the tour after 24h deadline
5. Tourist receives 100 bonus points
6. Tourist adds another tour to cart (price: 150 RSD)
7. Checks "Use bonus points" — price reduced to 50 RSD
8. Purchases — remaining 0 bonus points

---

## Event Sourcing Verification

For any problem, expand the event history. Each state transition produces a timestamped event:

```
1. ProblemCreated      — Tourist reported the problem
2. SentToReview        — Guide escalated to admin
3. ReturnedToGuide     — Admin sent back to guide
4. Resolved            — Guide resolved the problem
```

Verify that events are immutable and ordered by sequence number.
