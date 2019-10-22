import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';

@Component({
  selector: 'app-email-confirmed',
  templateUrl: './email-confirmed.component.html',
  styleUrls: ['./email-confirmed.component.css']
})
export class EmailConfirmedComponent implements OnInit {


  constructor(
    @Inject(MAT_DIALOG_DATA) public confirmMessage: any,
    public dialogRef: MatDialogRef<EmailConfirmedComponent>
  ) {
   }

  ngOnInit() {
  }

  onClose(){
    this.dialogRef.close();
  }

}
