import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-profile-image-avatar',
  templateUrl: './profile-image-avatar.component.html',
  styleUrls: ['./profile-image-avatar.component.css']
})
export class ProfileImageAvatarComponent implements OnInit {
  @Input()
  src?: string;
  
  @Input()
  size = 48;
  constructor() { }

  ngOnInit(): void {
  }

}
