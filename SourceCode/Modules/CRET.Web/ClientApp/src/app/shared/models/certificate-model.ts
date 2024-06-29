import { CertificateEnum } from "./certificate-enum";
import { CertificateStatus } from "./certificate-status";

export class CertificateModel {
  id: string;
  incidentNo: string;
  organizartionId: string;
  organizationName: string;
  certificateName: string;
  level: CertificateEnum;
  certificateStatus:CertificateStatus;
  info:string;
  createdAt: Date;
  createdBy: string;
  tag: string;
  csrContent: string;
  certificateContent: string;
  importCSRError: string;
  activationDate: Date;
  expirationDate: Date;
}
