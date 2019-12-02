import { Component, OnInit, Inject } from '@angular/core';
import { Validators, FormBuilder, FormGroup } from '@angular/forms';

import { MatDialogRef } from '@angular/material';

import { UserService } from '../services/User/user.service';

import { RegexExpressions } from '../shared/regex-expressions';
import { IResponseContent } from '../shared/models/response-content';

@Component({
  selector: 'app-confirm-email-popup',
  templateUrl: './confirm-email-popup.component.html',
  styleUrls: ['./confirm-email-popup.component.css']
})
export class ConfirmEmailPopupComponent implements OnInit {

  keyForm: FormGroup;
  message: string;

  constructor(
    private userServices: UserService,
    private formBuilder: FormBuilder,
    public dialogRef: MatDialogRef<ConfirmEmailPopupComponent>,
  ) {
    this.message = "(check your email)";
    this.keyInit();
   }

   onSubmit(): any{
    let keyNumber: number = this.key.value;
    return this.userServices.sentConfirmKey(keyNumber).subscribe((data: IResponseContent) =>{

      console.log(data);
      this.dialogRef.close(data);

    }, error => {

      let errorContent: IResponseContent = error.error;  
      if (!errorContent.isSucceeded){
        this.message = errorContent.errorMessage;
      }

    });
  }

  ngOnInit() {
  }

  keyInit(){
    this.keyForm = this.formBuilder.group({
      key: ['', [
        Validators.required,
          Validators.pattern(RegexExpressions.keyRgx)
      ]]
    });
  }

  get key(){
    return this.keyForm.get('key');
  }

  onClose(){
    this.dialogRef.close();
  }
}
