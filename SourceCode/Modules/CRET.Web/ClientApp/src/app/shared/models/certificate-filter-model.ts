
export class CertificateFilterModel {
  /**
   *
   */
  constructor() {
  this.dateFrom = null;
    this.dateTo = null;
    this.organizationName = 'All';
    this.certificateLevel = 'All';
    this.certificateStatus = 'All';
  }
  organizationName: string;
  certificateLevel:string;
  certificateStatus: string;
  dateFrom: Date;
  dateTo: Date;
  startDate: string;
  endDate: string;
}
