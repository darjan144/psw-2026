import { Component, inject, OnInit, signal } from '@angular/core';

import { AdminService } from '../../../core/services/admin.service';
import { ToastService } from '../../../core/services/toast.service';
import { BlockedUser } from '../../../core/models/admin.model';

@Component({
  selector: 'app-blocked-users',
  standalone: true,
  templateUrl: './blocked-users.component.html',
})
export class BlockedUsersComponent implements OnInit {
  private readonly adminService = inject(AdminService);
  private readonly toast = inject(ToastService);

  readonly users = signal<BlockedUser[]>([]);
  readonly loading = signal(true);

  ngOnInit(): void {
    this.adminService.getBlockedUsers().subscribe({
      next: (users) => {
        this.users.set(users);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  unblock(userId: number): void {
    this.adminService.unblockUser(userId).subscribe({
      next: () => {
        this.users.update((list) => list.filter((u) => u.id !== userId));
        this.toast.success('User unblocked');
      },
    });
  }
}
