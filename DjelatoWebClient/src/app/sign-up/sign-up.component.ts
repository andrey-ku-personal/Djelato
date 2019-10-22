import { Component, OnInit } from '@angular/core';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, ValidationErrors } from '@angular/forms';
import { UserService } from '../Services/UserServices/user.service';
import { MatDialogRef } from '@angular/material';
import { UserModel } from '../sign-up//models/user-model';
import { IResponseContent } from '../shared/models/response-content';
import { RegexExpressions } from '../shared/regex-expressions';
 
@Component({
  selector: 'app-sign-up',
  templateUrl: './sign-up.component.html',
  styleUrls: ['./sign-up.component.css']
})

export class SignUpComponent implements OnInit {

  profileForm: FormGroup;
  model: UserModel;
  message: string;

  constructor(
    private userService: UserService,
    private formBuilder: FormBuilder,
    public dialogRef: MatDialogRef<SignUpComponent>
  ){
    this.profileForm = this.formBuilder.group({
      name: ['', [
        Validators.required, 
        Validators.pattern(RegexExpressions.nameRgx)]
      ],
      email: ['', [
        Validators.required, 
        Validators.pattern(RegexExpressions.emailRgx)]
      ],
  
      password: ['', [
        Validators.required,
        Validators.pattern(RegexExpressions.passwordRgx)]
      ],
  
      passwordConfirm: ['', [
        Validators.required,
        this.checkPasswords()
      ]]
    });
  }

  ngOnInit() {
    this.profileForm.get("password").valueChanges.subscribe(() => {
      this.profileForm.get("passwordConfirm").updateValueAndValidity();
    });
  }

  checkPasswords(): ValidatorFn {
    return (control: FormControl): ValidationErrors => {
      const group = control.parent as FormGroup;

      if (!group) {
        return null;
      }

      let pass = group.get("password").value;
      let confirmPass = group.get("passwordConfirm").value;
 
      return confirmPass ? pass === confirmPass ? null : { mismatch: true } : null;
    };
  }

  get name(){
    return this.profileForm.get('name');
  }

  get email(){
    return this.profileForm.get('email');
  }

  get password(){
    return this.profileForm.get('password')
  }

  get passwordConfirm(){
    return this.profileForm.get('passwordConfirm')
  }

  onClose(){
    this.dialogRef.close();
  }

  onSubmit() {
    this.model = <UserModel>this.profileForm.value;

    this.userService.createUser(this.model).subscribe((data: IResponseContent) => {
      console.log(data);

      if (data.isSucceeded){
        this.dialogRef.close();
      }

    }, (error) => {
      let errorContent: IResponseContent = error.error;        
      if (!errorContent.isSucceeded){
        this.message = errorContent.errorMessage;
      }      
    });
  }
}
