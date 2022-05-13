import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {

  constructor() { }

userinfo :any

  ngOnInit(): void {
    this.userinfo =JSON.parse(localStorage.getItem("LUser")).userContext;
  }

}
