import { Component, OnInit } from '@angular/core';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material';
import { SignUpComponent } from '../sign-up/sign-up.component';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {

  todayDate = new Date();
  
  constructor(
    private dialog: MatDialog
  ) { }

  ngOnInit() {
  }

  createDialog(){
    this.dialog.open(SignUpComponent, { autoFocus: false });

  }
}
