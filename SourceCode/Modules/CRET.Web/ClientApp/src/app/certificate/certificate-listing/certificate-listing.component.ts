import {
  AfterViewInit,
  Component,
  ElementRef,
  OnInit,
  HostListener,
  ViewChild,
} from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';

import { CertificateModel } from '../../shared/models/certificate-model';
import { CertificateService } from '../../shared/services/certificate.service';
import { CreateDialogComponent } from '../create-dialog/create-dialog.component';
import { EditDialogComponent } from '../edit-dialog/edit-dialog.component';
import { DeleteDialogComponent } from '../delete-dialog/delete-dialog.component';
import { ToastrService } from 'ngx-toastr';
import { MatPaginator } from '@angular/material/paginator';
import { FormControl } from '@angular/forms';
import { catchError, map, startWith, switchMap } from 'rxjs/operators';
import { merge, of as observableOf, pipe } from 'rxjs';
import { MatSort, Sort } from '@angular/material/sort';
import { OrganizationService } from 'src/app/shared/services/organization.service';
import { OrganizationModel } from 'src/app/shared/models/organization-model';
import { CertificateStatus } from '../../shared/models/certificate-status';
import { CertificateEnum } from 'src/app/shared/models/certificate-enum';
import { SelectionModel } from '@angular/cdk/collections';
import { ImportCsrDialogComponent } from '../import-csr-dialog/import-csr-dialog.component';
import { ImportCertificateDialogComponent } from '../import-certificate-dialog/import-certificate-dialog.component';
import { QrImageModel } from '../../shared/models/qr-Image-model';
import { HttpErrorResponse } from '@angular/common/http';
import { BatchSendDialogComponent } from '../batch-send-dialog/batch-send-dialog.component';
import { LocalStorage } from 'src/app/shared/services/local-storage.service';
import { CertificateFilterModel } from 'src/app/shared/models/certificate-filter-model';
import * as moment from 'moment';
import { DatatableState } from 'src/app/shared/models/datatable-state-model';
import { BatchAlertDialogComponent } from '../batch-alert-dialog/batch-alert-dialog.component';
// import { CsrImportDialogComponent } from '../csr-import-dialog/csr-import-dialog.component';
@Component({
  selector: 'app-certificate-listing',
  templateUrl: './certificate-listing.component.html',
  styleUrls: ['./certificate-listing.component.css'],
})
export class CertificateListingComponent implements OnInit, AfterViewInit {
  sort: Sort = {
    active: '',
    direction: '',
  };

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) matSort: MatSort;
  @ViewChild('csrFileInput') csrFileInput: ElementRef;

  constructor(
    private localStorageService: LocalStorage,
    private certificateService: CertificateService,
    private organizationService: OrganizationService,
    public dialog: MatDialog,
    private toast: ToastrService
  ) {
    this.getOrganizationsList();
  }

  datatableState: DatatableState;
  csrSelectedFile: File;
  fileContent: string;
  fileExtension: string;
  fileType: string;

  filterationModel: CertificateFilterModel = new CertificateFilterModel();
  filterOptions: string = '';
  showCard: boolean = false;
  isDialogOpen: boolean = false;

  isSuperAdmin: boolean = false;
  certificateLevelEnum = [];
  certificateStatusEnum = [];

  searchKeywordFilter = new FormControl();
  StatusDisplayType = {
    [CertificateStatus.Created]: 'Created',
    [CertificateStatus.Assigned]: 'Assigned',
    [CertificateStatus.CSR_Received]: 'CSR Received',
    [CertificateStatus.CertificateCreated]: 'Certificate Created',
    // Add other additional names for enum members as needed
  };
  pageSizes = [50, 100, 500];
  totalData: number;
  isLoading = false;
  public StatusEnum = CertificateStatus;
  public CertificateEnum = CertificateEnum;

  isDragging: boolean = false;

  onDragOver(event: DragEvent) {
    if (this.isFile(event) && !this.isDialogOpen) {
      event.preventDefault();
      event.stopPropagation();
      this.isDragging = true;
    }
  }

  onDrop(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    const files = event.dataTransfer.files;
    if (files.length > 0) {
      const fileName = files[0].name.toLowerCase();
      if (
        !(
          fileName.endsWith('.csr') ||
          fileName.endsWith('.pem') ||
          fileName.endsWith('.txt')
        )
      ) {
        // Display error message here, e.g., using alert or a modal
        this.toast.warning(
          'The dropped file is neither a certificate nor a CSR',
          '',
          {
            positionClass: 'toast-bottom-right',
          }
        );
        this.isDragging = false;
        return; // Exit the function early if no valid files found
      } else {
        this.csrSelectedFile = files[0];
        this.readFileContent();
      }
    }
  }

  onDragLeave(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = false;
  }

  isFile(evt) {
    var dt = evt.dataTransfer;

    for (var i = 0; i < dt.types.length; i++) {
      if (dt.types[i] === 'Files') {
        return true;
      }
    }
    return false;
  }

  @HostListener('document:dragenter', ['$event'])
  onDragEnter(event: DragEvent) {
    if (this.isFile(event) && !this.isDialogOpen) {
      event.preventDefault();
      event.stopPropagation();
      this.isDragging = true;
    }
  }

  selection = new SelectionModel<CertificateModel>(true, []);
  selectedRows: CertificateModel[] = [];

  organizationList: OrganizationModel[] = [];

  displayedColumns = [
    'select',
    // 'incidentNo',
    'organizationName',
    'tag',
    'level',
    'certificateStatus',
    'info',
    'createdAt',
    'createdBy',
    'action',
  ];
  certificateList: CertificateModel[] = [];
  dataSource = new MatTableDataSource<CertificateModel>();

  ngOnInit(): void {
    this.isSuperAdmin = this.localStorageService.IsSuperAdmin();
    this.certificateLevelEnum =
      this.certificateService.getFilteredCertificateEnum(this.isSuperAdmin);
    this.certificateStatusEnum =
      this.certificateService.getCertificateStatusEnum();
  }

  getTableData(
    pageNumber: number,
    pageSize: number,
    sort: string,
    sortOrder: string,
    filter: string,
    filterOptions: string
  ) {
    this.sort.active = sort;
    let sortWithOrder = sort;
    if (sortOrder == 'desc') {
      sortWithOrder = sort + '|' + sortOrder;
      this.sort.direction = 'desc';
    } else if (sortOrder == 'asc') {
      this.sort.direction = 'asc';
    } else {
      this.sort.direction = '';
    }

    const sortingState = this.localStorageService.getSortingState(
      this.localStorageService.certificatesortingKey
    );

    debugger;
    this.datatableState = new DatatableState();
    this.datatableState.sort = this.sort;
    this.datatableState.filter = filter;
    this.datatableState.filterOptions = filterOptions;

    // if (sortingState) {
    //   this.sort.active = "createdAt";
    //   this.sort.direction = "desc";
    //   this.datatableState.sort = this.sort;
    // }

    this.localStorageService.setSortingState(
      this.localStorageService.certificatesortingKey,
      this.datatableState
    );

    return this.certificateService.getDatatable(
      pageNumber,
      pageSize,
      sortWithOrder,
      filter,
      filterOptions
    );
  }

  ngAfterViewInit() {
    debugger;
    // If the user changes the sort order, reset back to the first page.
    this.matSort.sortChange.subscribe(() => (this.paginator.pageIndex = 0));

    this.matSort.disableClear = true;
    this.dataSource.paginator = this.paginator;

    this.dataSource.sort = this.matSort;

    this.GetStateData();

    this.LoadData();
  }
  /** Whether the number of selected elements matches the total number of rows. */
  isAllSelected() {
    const numSelected = this.selection.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  resetSelection(j: number) {
    const checkboxes = document.getElementsByName('ckb');
    for (let i = 0; i < checkboxes.length; i++) {
      if (i != j) {
        (checkboxes[i] as HTMLInputElement).checked = false;
      }
    }
  }

  masterToggle(row: CertificateModel, j: number) {
    const checkboxes = document.getElementsByName('ckb');
    var isChecked = (checkboxes[j] as HTMLInputElement).checked;
    const index = this.selectedRows.indexOf(row);
    if (isChecked) {
      const isValidSelection = this.isSameOrganizationSelected(row);
      if (isValidSelection) {
        const count = this.selectedRows.length;
        if (count >= 15) {
          (checkboxes[j] as HTMLInputElement).checked = false;

          const dialogRef = this.dialog.open(BatchAlertDialogComponent, {
            width: '35%',
            disableClose: true,
          });
          dialogRef.afterClosed().subscribe((result) => {});
        } else {
          if (index == -1) {
            this.selectedRows.push(row);
          }
        }
      } else {
        this.selectedRows = [];
        this.selectedRows.push(row);
        this.resetSelection(j);
      }
    } else {
      if (index !== -1) {
        this.selectedRows.splice(index, 1);
      }
    }
  }

  isSameOrganizationSelected(row: CertificateModel): boolean {
    let isLevelSame = true;
    let isOrganizationSame = true;
    if (this.selectedRows.length > 0) {
      isOrganizationSame =
        this.selectedRows[0].organizationName === row.organizationName;

      if (row.certificateStatus == this.StatusEnum.CertificateCreated) {
        isLevelSame =
          this.selectedRows[0].certificateStatus == row.certificateStatus;
      } else {
        isLevelSame =
          this.selectedRows[0].certificateStatus == this.StatusEnum.Assigned ||
          this.selectedRows[0].certificateStatus == this.StatusEnum.Created;
      }
    }
    return isOrganizationSame && isLevelSame;
  }

  getCertificates() {
    this.certificateService.getAllCertificates().subscribe((response) => {
      this.certificateList = response;
      this.dataSource = new MatTableDataSource<CertificateModel>(
        this.certificateList
      );
    });
  }
  getOrganizationsList() {
    this.getOrganizations().subscribe((response) => {
      if (response && response.length > 0) {
        this.organizationList = response.filter((org) => org.isActive === 1);
      }
    });
  }
  createIncidentNoList(): string[] {
    // Extract incidentNo property from each CertificateModel
    const incidentNoList: string[] = this.selectedRows.map(
      (cert) => cert.incidentNo
    );
    return incidentNoList;
  }
  addCertificate() {
    this.showCard = false;
    // Call getOrganizations and wait for the response
    this.getOrganizations().subscribe((response) => {
      if (response && response.length > 0) {
        this.isDialogOpen = true;
        const dialogRef = this.dialog.open(CreateDialogComponent, {
          width: '35%',
          disableClose: true,
          data: { organizationList: response },
        });
        dialogRef.afterClosed().subscribe((result) => {
          this.isDialogOpen = false;
          if (result) {
            this.toast.success('Certificate created successfully', 'Success', {
              positionClass: 'toast-bottom-right',
            });
            this.LoadData();
          }
        });
      } else {
        this.toast.warning('Organization list is empty', '', {
          positionClass: 'toast-bottom-right',
        });
        // Handle scenario when organizationList is empty or null
        console.error('Organization list is empty or null');
        // Optionally, show an error message or handle the scenario as required
      }
    });
  }
  editCertificate(data) {
    this.showCard = false;
    // Call getOrganizations and wait for the response
    this.getOrganizations().subscribe((response) => {
      if (response && response.length > 0) {
        this.isDialogOpen = true;

        const dialogRef = this.dialog.open(EditDialogComponent, {
          width: '40%',
          disableClose: true,
          data: { organizationList: response, certificateData: data },
        });

        dialogRef.afterClosed().subscribe((result) => {
          this.isDialogOpen = false;

          if (result.resultFor == 'qRCodeEmail') {
            this.updateRowData(result.data);
          } else if (result.resultFor == 'signCSR') {
            this.LoadData();
          }

          // else if(result){
          //   this.LoadData();
          // }
        });
      } else {
        this.toast.warning('Organization list is empty', '', {
          positionClass: 'toast-bottom-right',
        });
        // Handle scenario when organizationList is empty or null
        console.error('Organization list is empty or null');
        // Optionally, show an error message or handle the scenario as required
      }
    });
  }

  deleteCertificate(id) {
    this.showCard = false;
    this.isDialogOpen = true;

    const dialogRef = this.dialog.open(DeleteDialogComponent, {
      disableClose: true,
      width: '25%',
      autoFocus: false,
      data: { Id: id },
    });
    dialogRef.afterClosed().subscribe((result) => {
      this.isDialogOpen = false;
      console.log('The dialog was closed');
      if (result) {
        this.toast.success('Certificate deleted successfully', 'Success', {
          positionClass: 'toast-bottom-right',
        });
        this.LoadData();
      }
    });
  }

  openImportCsrDialog() {
    this.fileExtension = '.csr,.pem';
    // this.fileType = 'csr';
    this.csrFileInput.nativeElement.accept = '.csr,.pem';
    this.csrFileInput.nativeElement.click();
  }

  openImportCertificateDialog() {
    this.fileExtension = '.pem';
    this.csrFileInput.nativeElement.accept = '.pem';

    // this.fileType = 'certificate';
    this.csrFileInput.nativeElement.click();
  }

  openBatchSendDialog() {
    this.showCard = false;
    var organization = this.organizationList.find(
      (org) => org.id === this.selectedRows[0].organizartionId
    );
    if (organization === null || organization === undefined) {
      this.toast.error('Please select certificate', 'Error', {
        positionClass: 'toast-bottom-right',
      });
    } else {
      this.isDialogOpen = true;

      const dialogRef = this.dialog.open(BatchSendDialogComponent, {
        width: '35%',
        disableClose: true,
        data: {
          email: organization.email,
          count: this.selectedRows.length,
          certificateStatus: this.selectedRows[0].certificateStatus,
        },
      });
      dialogRef.afterClosed().subscribe((result) => {
        this.isDialogOpen = false;
        if (result.email) {
          this.sendBatch(result.email);
        }
      });
    }
  }

  getOrganizations() {
    // Return the observable from the organization service
    return this.organizationService.getAllOrganizations();
  }

  sendBatch(email: string) {
    this.showCard = false;
    const qrImageModel = new QrImageModel();
    qrImageModel.toEmail = email;
    qrImageModel.certificateIds = this.createIncidentNoList();

    this.certificateService.sendQrCodeToEmail(qrImageModel).subscribe({
      next: (data) => {
        this.LoadData();
        let successMessage =
          this.selectedRows[0].certificateStatus ==
          CertificateStatus.CertificateCreated
            ? 'Certificate has been sent'
            : 'CSR Token has been sent';
        this.toast.success(successMessage, 'Success', {
          positionClass: 'toast-bottom-right',
        });
      },
      error: (e: HttpErrorResponse) => {
        this.toast.error('Something went wrong', 'Error', {
          positionClass: 'toast-bottom-right',
        });
      },
    });
  }

  applyStatusTextColor(status) {
    let color: string;
    switch (status) {
      case CertificateStatus.Created:
        color = '#7372b2';
        break;
      case CertificateStatus.Assigned:
        color = '#39a6c8';
        break;
      case CertificateStatus.CSR_Received:
        color = '#ffbe15';
        break;
      case CertificateStatus.CertificateCreated:
        color = '#84b520';
        break;
      default:
        // Default styles if the status doesn't match any case
        color = '#7ab10c';
    }
    return color;
  }
  getStatusBackgroundColor(status: CertificateStatus): any {
    let backgroundColor: string;
    switch (status) {
      case CertificateStatus.Created:
        backgroundColor = '#f0f0f7';
        break;
      case CertificateStatus.Assigned:
        backgroundColor = '#e5f3f8';
        break;
      case CertificateStatus.CSR_Received:
        backgroundColor = '#fff8e5';
        break;
      case CertificateStatus.CertificateCreated:
        backgroundColor = '#f3f4e6';
        break;
      default:
    }
    return backgroundColor;
  }

  LoadData() {
    debugger;
    merge(
      // this.searchKeywordFilter.valueChanges,
      this.matSort.sortChange,
      this.paginator.page
    )
      .pipe(
        startWith({}),
        switchMap(() => {
          this.isLoading = true;
          var filterValue =
            this.searchKeywordFilter.value == null
              ? ''
              : this.searchKeywordFilter.value;
          return this.getTableData(
            this.paginator.pageIndex + 1,
            this.paginator.pageSize,
            this.matSort.active,
            this.matSort.direction,
            filterValue,
            this.filterOptions
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
        this.selection.clear();
        this.selectedRows = [];
        this.certificateList = empData;
        this.dataSource = new MatTableDataSource(this.certificateList);
        this.isDragging = false;
      });
  }
  updateRowData(updatedRowData: CertificateModel) {
    // Find the index of the row to be updated in the certificateList
    const rowIndex = this.certificateList.findIndex(
      (row) => row.incidentNo === updatedRowData.incidentNo
    );

    // If the row is found, update the data
    if (rowIndex !== -1) {
      this.certificateList[rowIndex] = updatedRowData;

      // Update the MatTableDataSource with the modified certificateList
      this.dataSource.data = this.certificateList;
    } else {
      // Handle the scenario when the row is not found
      console.error('Row not found for update');
    }
  }

  toggleFilterCardVisibility() {
    this.showCard = !this.showCard; // Toggles the value of showCard
  }

  filterData() {
    if (this.filterationModel.dateFrom !== null) {
      const dateFrom = moment(this.filterationModel.dateFrom);
      if (!dateFrom.isValid()) {
        this.toast.error('Invalid date format', 'Error', {
          positionClass: 'toast-bottom-right',
        });
        return;
      }
      this.filterationModel.startDate = moment(
        this.filterationModel.dateFrom
      ).format('YYYY-MM-DD');
    } else {
      this.filterationModel.startDate = null;
    }

    if (this.filterationModel.dateTo !== null) {
      const dateTo = moment(this.filterationModel.dateTo);
      // Check if dateTo is a valid date
      if (!dateTo.isValid()) {
        this.toast.error('Invalid date format', 'Error', {
          positionClass: 'toast-bottom-right',
        });
        return; // Exit the function early if dateTo is invalid
      }
      this.filterationModel.endDate = moment(
        this.filterationModel.dateTo
      ).format('YYYY-MM-DD');
    } else {
      this.filterationModel.endDate = null;
    }

    // this.filterOptions = JSON.stringify(this.filterationModel);

    const serializedFilterationModel = {
      startDate: this.filterationModel.startDate,
      endDate: this.filterationModel.endDate,
      organizationName:
        this.filterationModel.organizationName === 'All'
          ? null
          : this.filterationModel.organizationName,
      certificateLevel:
        this.filterationModel.certificateLevel === 'All'
          ? null
          : this.filterationModel.certificateLevel,
      certificateStatus:
        this.filterationModel.certificateStatus === 'All'
          ? null
          : this.filterationModel.certificateStatus,
    };

    // Serialize the new object
    this.filterOptions = JSON.stringify(serializedFilterationModel);

    this.LoadData();
  }

  searchKeywordAction() {
    this.LoadData();
  }
  clearFilter() {
    this.filterationModel = new CertificateFilterModel();
  }

  handleFileInput(event: any): void {
    const input = event.target as HTMLInputElement;
    const files: FileList = event.target.files;
    if (files.length > 0) {
      this.csrSelectedFile = files[0];
      // Read file content
      this.readFileContent();
      input.value = '';
    }
  }

  readFileContent(): void {
    const reader = new FileReader();
    reader.onload = (e) => {
      // The result attribute contains the file's content as a data URL
      const fileName = this.csrSelectedFile.name.toLowerCase();
      debugger;
      this.fileContent = reader.result as string;
      // if (this.fileType == 'csr' && !fileName.endsWith('.csr')) {
      //   this.toast.warning('The dropped file is not a CSR', '', {
      //     positionClass: 'toast-bottom-right',
      //   });
      // } else if (this.fileType == 'certificate' && !fileName.endsWith('.pem')) {
      //   this.toast.warning('The dropped file is not a certificate', '', {
      //     positionClass: 'toast-bottom-right',
      //   });
      // }
      if (fileName.endsWith('.csr')) {
        this.uploadCsrFile();
      } else if (fileName.endsWith('.pem')) {
        this.uploadCertificateFile();
      } else if (fileName.endsWith('.txt')) {
        if (
          this.fileContent.includes('-----BEGIN CERTIFICATE-----') &&
          this.fileContent.includes('-----END CERTIFICATE-----')
        ) {
          this.uploadCertificateFile();
        } else if (
          this.fileContent.includes('-----BEGIN CERTIFICATE REQUEST-----') &&
          this.fileContent.includes('-----END CERTIFICATE REQUEST-----')
        ) {
          this.uploadCsrFile();
        } else {
          if (this.fileType == 'csr') {
            this.toast.warning('The dropped file is not a CSR', '', {
              positionClass: 'toast-bottom-right',
            });
          } else if (this.fileType == 'certificate') {
            this.toast.warning('The dropped file is not a certificate', '', {
              positionClass: 'toast-bottom-right',
            });
          } else {
            this.toast.warning(
              'The dropped file is neither a certificate nor a CSR',
              '',
              {
                positionClass: 'toast-bottom-right',
              }
            );
          }
          this.isDragging = false;
        }
      } else {
        this.toast.warning(
          'The dropped file is neither a certificate nor a CSR',
          '',
          {
            positionClass: 'toast-bottom-right',
          }
        );
      }
    };

    // Read the file as text
    reader.readAsText(this.csrSelectedFile);
  }

  uploadCsrFile(): void {
    let res = this.fileContent;
    const csrContent = { content: this.fileContent };

    this.certificateService.importCsr(csrContent).subscribe({
      next: (data) => {
        this.toast.success('Csr imported successfully', 'Success', {
          positionClass: 'toast-bottom-right',
        });
        this.LoadData();
      },
      error: (e: HttpErrorResponse) => {
        console.log(e);
        if (e.status == 404) {
          this.toast.error(e.error, 'Error', {
            positionClass: 'toast-bottom-right',
          });
        } else {
          this.toast.error('Something went wrong', 'Error', {
            positionClass: 'toast-bottom-right',
          });
        }
        this.LoadData();
      },
    });
  }

  uploadCertificateFile(): void {
    const fileContent = { content: this.fileContent };

    this.certificateService.importCertificate(fileContent).subscribe({
      next: (data) => {
        this.toast.success('Certificate imported successfully', 'Success', {
          positionClass: 'toast-bottom-right',
        });
        this.LoadData();
      },
      error: (e: HttpErrorResponse) => {
        if (e.status == 404) {
          this.toast.error(e.error, 'Error', {
            positionClass: 'toast-bottom-right',
          });
        } else {
          this.toast.error('Something went wrong', 'Error', {
            positionClass: 'toast-bottom-right',
          });
        }
        this.LoadData();
      },
    });
  }

  GetStateData() {
    debugger;
    const sortingState = this.localStorageService.getSortingState(
      this.localStorageService.certificatesortingKey
    );
    if (sortingState) {
      if (
        sortingState.sort.active !== '' &&
        sortingState.sort.direction !== ''
      ) {
        this.matSort.active = sortingState.sort.active;
        this.matSort.direction = sortingState.sort.direction;

        this.sort.active = sortingState.sort.active;
        this.sort.direction = sortingState.sort.direction;
      }
      else{
        this.matSort.active = 'createdAt';
        this.matSort.direction = 'desc';
      }
    } else {
      this.matSort.active = 'createdAt';
      this.matSort.direction = 'desc';
    }
  }
}
