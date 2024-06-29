import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';

//.ts
import { OrganizationModel } from '../../shared/models/organization-model';
import { OrganizationService } from '../../shared/services/organization.service';
import { CreateDialogComponent } from '../create-dialog/create-dialog.component';
import { EditDialogComponent } from '../edit-dialog/edit-dialog.component';
import { DeleteDialogComponent } from '../delete-dialog/delete-dialog.component';
import { ToastrService } from 'ngx-toastr';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort';
import { catchError, map, startWith, switchMap } from 'rxjs/operators';
import { merge, of as observableOf, pipe } from 'rxjs';
import { FormControl } from '@angular/forms';
import { LocalStorage } from 'src/app/shared/services/local-storage.service';

@Component({
  selector: 'app-organization-listing',
  templateUrl: './organization-listing.component.html',
  styleUrls: ['./organization-listing.component.css'],
})
export class OrganizationListingComponent implements OnInit, AfterViewInit {
  sort: Sort = {
    active: '',
    direction: '',
  };

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) matSort: MatSort;

  searchKeywordFilter = new FormControl();

  pageSizes = [5, 10, 20];
  totalData: number;
  isLoading = false;

  constructor(
    private localStorageService: LocalStorage,
    private organizationService: OrganizationService,
    public dialog: MatDialog,
    private toastr: ToastrService
  ) { }

  displayedColumns = [
    'customerNumber',
    'customerName',
    'organizationName',
    'email',
    'contact',
    'createdAt',
    'createdBy',
    'action',
  ];
  organizationList: OrganizationModel[] = [];
  dataSource = new MatTableDataSource<OrganizationModel>();

  ngOnInit(): void {
    // this.getOrganizations();
  }

  getTableData(
    pageNumber: number,
    pageSize: number,
    sort: string,
    sortOrder: string,
    filter: string
  ) {
    this.sort.active = sort;
    this.sort.direction = 'asc';
    let sortWithOrder = sort;
    if (sortOrder == 'desc') {
      sortWithOrder = sort + '|' + sortOrder;
      this.sort.direction = 'desc';
    }
    this.localStorageService.setSortingState(this.localStorageService.organizationSortingStateKey,this.sort);

    return this.organizationService.getDatatable(
      pageNumber,
      pageSize,
      sortWithOrder,
      filter
    );
  }

  ngAfterViewInit() {
    // If the user changes the sort order, reset back to the first page.
    this.matSort.sortChange.subscribe(() => (this.paginator.pageIndex = 0));
    this.matSort.disableClear = true;
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.matSort;

    const sortingState = this.localStorageService.getSortingState(this.localStorageService.organizationSortingStateKey);
    if (sortingState) {
      // Apply the sorting state to the table
      this.matSort.active = sortingState.active;
      this.matSort.direction = sortingState.direction;

      this.sort.active = sortingState.active;
      this.sort.direction = sortingState.direction;
    }
    this.LoadData();
  }
  LoadData() {
    merge(
      // this.searchKeywordFilter.valueChanges,
       this.matSort.sortChange, this.paginator.page)
      .pipe(
        startWith({}),
        switchMap(() => {
          this.isLoading = true;
          var filterValue = this.searchKeywordFilter.value == null ? '' : this.searchKeywordFilter.value;
          return this.getTableData(
            this.paginator.pageIndex + 1,
            this.paginator.pageSize,
            this.matSort.active,
            this.matSort.direction,
            filterValue
          ).pipe(
            catchError((error) => {
              console.error('Error occurred:', error); // Logging the error
              return observableOf(null);
            })
          );
        }),
        map((empData) => {
          if (empData == null) return [];
          this.totalData = empData.total;
          this.isLoading = false;
          return empData.data;
        })
      )
      .subscribe((empData) => {
        this.organizationList = empData;
        this.dataSource = new MatTableDataSource(this.organizationList);
      });
  }
  getOrganizations() {
    this.organizationService.getAllOrganizations().subscribe((response) => {
      this.organizationList = response;
      this.dataSource = new MatTableDataSource<OrganizationModel>(
        this.organizationList
      );
      this.dataSource.paginator = this.paginator;
    });
  }

  //Add new organizayion dialog show
  addOrganization() {
    const dialogRef = this.dialog.open(CreateDialogComponent, {
      width: '35%',
      disableClose: true,
    });
    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.toastr.success('Organization created successfully', 'Success', {
          positionClass: 'toast-bottom-right',
        });
        this.LoadData();
      }
    });
  }

  editOrganization(data) {
    const dialogRef = this.dialog.open(EditDialogComponent, {
      disableClose: true,
      width: '35%',
      data: { organizationData: data },
    });
    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.toastr.success('Organization updated successfully', 'Success', {
          positionClass: 'toast-bottom-right',
        });
        this.LoadData();
      }
    });
  }

  deleteOrganization(id) {
    const dialogRef = this.dialog.open(DeleteDialogComponent, {
      disableClose: true,
      width: '25%',
      autoFocus: false,
      data: { Id: id },
    });
    dialogRef.afterClosed().subscribe((result) => {
      console.log('The dialog was closed');
      if (result) {
        this.toastr.success('Organization deleted successfully', 'Success', {
          positionClass: 'toast-bottom-right',
        });
        this.LoadData();
      }
    });
  }

  searchKeywordAction()
  {
    this.LoadData();
  }
}
