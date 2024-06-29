import { CertificateEnum } from "./certificate-enum";

export class CsrModel {
  numberOfCertificate: number;
  organizartionId: string;
  organizationName: string;
  level: CertificateEnum;
  info:string;
  createdBy: string;
  createdAt: Date;
}
