import { Component, OnInit } from '@angular/core';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, ValidationErrors } from '@angular/forms';
import { UserService } from '../Services/UserServices/user.service';
import { MatDialogRef } from '@angular/material';
import { UserModel } from '../sign-up//models/user-model'
 
@Component({
  selector: 'app-sign-up',
  templateUrl: './sign-up.component.html',
  styleUrls: ['./sign-up.component.css']
})

export class SignUpComponent implements OnInit {

  profileForm: FormGroup;
  model: UserModel;

  constructor(
    private userService: UserService,
    private formBuilder: FormBuilder,
    public dialogRef: MatDialogRef<SignUpComponent>,
  ){
    this.profileForm = this.formBuilder.group({
      name: ['', [
        Validators.required, 
        Validators.pattern(/^[a-zA-Z0-9_ -]+$/)]
      ],
      email: ['', [
        Validators.required, 
        Validators.pattern(/^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/)]
      ],
  
      password: ['', [
        Validators.required,
        Validators.pattern(/^(?=.*[A-Z])(?=.*\d)(.{8,100})$/)]
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
    this.profileForm.reset();
    this.dialogRef.close();
  }

  onSubmit() {
    this.model = <UserModel>this.profileForm.value;



    console.warn(this.profileForm.value);
    this.userService.createUser(this.profileForm.value); 
    this.dialogRef.close();
  }
}
