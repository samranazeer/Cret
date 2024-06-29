import { Component, OnInit } from '@angular/core';
import { SettingModel } from '../shared/models/setting-model';
import { SettingService } from '../shared/services/setting.service';
import { HttpErrorResponse } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.css'],
})
export class SettingsComponent implements OnInit {
  constructor(
    private settingService: SettingService,
    private toastr: ToastrService
  ) {}
  model = new SettingModel();
   selected = "90";

  ngOnInit(): void {
    this.getCertificateValiditySetting();
  }

  getCertificateValiditySetting() {
    this.settingService.GetCertificateValidity().subscribe((response) => {
        this.model = response;
    });
  }

  saveASetting() {
    this.model.createdBy = 'Admin';
    this.model.createdAt = new Date();
    this.settingService.saveSettings(this.model).subscribe({
      next: (data) => {
        this.toastr.success('Settings updated', '', {
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
}
