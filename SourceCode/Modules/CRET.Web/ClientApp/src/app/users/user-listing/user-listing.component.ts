import { Component, OnInit, ViewChild, HostListener  } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { SelectionModel } from '@angular/cdk/collections';

import { RestfulService } from 'src/app/shared/services/restful.service';
import { MicrosoftUserList } from 'src/app/shared/models/user-list-model';
import { MicrosoftUser } from 'src/app/shared/models/user-model';
import { UserService } from 'src/app/shared/services/user.service';
import { AllowedUser } from 'src/app/shared/models/allowed-user-model';
import { HttpErrorResponse } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-user-listing',
  templateUrl: './user-listing.component.html',
  styleUrls: ['./user-listing.component.css'],
})
export class UserListingComponent {
  constructor(
    private restfulService: RestfulService,
    private userService: UserService,
    private toastr: ToastrService
  ) { }
  @ViewChild(MatPaginator) paginator: MatPaginator;

  displayedColumns: string[] = ['displayName', 'userPrincipalName', 'select'];
  dataSource = new MatTableDataSource<MicrosoftUser>();
  userList: MicrosoftUser[] = [];
  msalUsers?: MicrosoftUserList;

  selection = new SelectionModel<MicrosoftUser>(true, []);

  selectedRows: MicrosoftUser[] = [];

  allowedUserIds;
  ngOnInit() {
    this.getAllowedUsersFromDb();
    this.getUsersList();
    this.dataSource.paginator = this.paginator;
  }

  /** Whether the number of selected elements matches the total number of rows. */
  isAllSelected() {
    const numSelected = this.selection.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  /** Selects all rows if they are not all selected; otherwise clear selection. */
  masterToggle() {

    this.isAllSelected()
      ? this.selection.clear()
      : this.dataSource.data.forEach((row) => this.selection.select(row));
  }

  logSelection() {
    this.selectedRows = this.selection.selected;
    const selectedUserId = this.selection.selected.map((row) => row.id);
    const userIdString = selectedUserId.join(',');
    if (userIdString == '' && this.allowedUserIds?.userIdList == "") {
      this.toastr.error('Please select user', 'Warning', {
        positionClass: 'toast-bottom-right',
      });
    } else {
      this.sendUserList(userIdString);
    }
  }

  sendUserList(userIdString) {
    let data = new AllowedUser();
    data.createdAt = new Date();
    data.createdBy = 'Admin';
    data.userIdList = userIdString;
    this.userService.sendAllowedUsers(data).subscribe({
      next: (data) => {
        // this.dialogRef.close(true);
        this.getAllowedUsersFromDb();
        this.toastr.success('Successfully Updated', 'Success', {
          positionClass: 'toast-bottom-right',
        });
      },
      error: (e: HttpErrorResponse) => {
        this.toastr.error('Something went wrong', 'Error', {
          positionClass: 'toast-bottom-right',
        });
      },
    });
  }
  convertToMicrosoftUserList(users: MicrosoftUser[]): MicrosoftUserList {
  return { value: users };
}
  getUsersList() {
    this.restfulService.getAllUsersList().subscribe((users: MicrosoftUser[]) => {
      // Access the array of MicrosoftUser objects

      this.msalUsers = this.convertToMicrosoftUserList(users);;
      this.userList = this.msalUsers.value;
      // Parse allowedUserIds string to an array of numbers
      if (this.allowedUserIds != null) {
        let ids = this.allowedUserIds.userIdList;
        const allowedIdsArray = ids.split(',');

        // Iterate through the user list and mark selected users based on allowedUserIds
        this.userList.forEach((user) => {
          if (allowedIdsArray.includes(user.id)) {
            this.selection.select(user); // Select users whose IDs are in allowedUserIds
          }
        });
      }

      this.dataSource = new MatTableDataSource<MicrosoftUser>(this.userList);
      this.dataSource.paginator = this.paginator;
    });
  }


  
  applyFilter(filterValue: string) {
    this.dataSource.filter = filterValue.trim().toLowerCase();
}
  getAllowedUsersFromDb() {
    this.userService.GetAllowedUsers().subscribe({
      next: (data) => {
        this.allowedUserIds = data;
      },
      error: (e: HttpErrorResponse) => {
        // this.toast.error('Something went wrong', 'Error', {
        //   positionClass: 'toast-bottom-right',
        // });
      },
    });
  }

  scrollToTop() {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }
  @HostListener('window:scroll', [])
  onWindowScroll() {
    const scrollPosition = window.pageYOffset || document.documentElement.scrollTop || document.body.scrollTop || 0;
    const scrollButton = document.querySelector('.scroll-top-btn');

    if (scrollButton) {
      if (scrollPosition > 200) { // Adjust the scroll position as needed
        scrollButton.classList.add('show');
      } else {
        scrollButton.classList.remove('show');
      }
    }
  }
}
