import { Component, OnInit } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormControl, FormGroupDirective, NgForm, Validators, FormGroup, FormBuilder } from '@angular/forms';
import { ErrorStateMatcher } from '@angular/material/core';
import { UserService } from '../Services/UserServices/user.service';

@Component({
  selector: 'app-sign-up',
  templateUrl: './sign-up.component.html',
  styleUrls: ['./sign-up.component.css']
})

export class MyErrorStateMatcher implements ErrorStateMatcher {
  isErrorState(control: FormControl | null, form: FormGroupDirective | NgForm | null): boolean {
    const invalidCtrl = !!(control && control.invalid && control.parent.dirty);
    const invalidParent = !!(control && control.parent && control.parent.invalid && control.parent.dirty);

    return (invalidCtrl || invalidParent);
  }
}

export class SignUpComponent implements OnInit {

  profileForm: FormGroup;
  matcher = new MyErrorStateMatcher();

  constructor(
     private userService: UserService,
     private formBuilder: FormBuilder
  ) {
    this.profileForm = this.formBuilder.group({
      name: ['', [Validators.required, Validators.pattern(/^[a-zA-Z0-9_ -]+$/)]],
      email: ['', [Validators.required, Validators.pattern(/^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/)]],
      password: ['', [Validators.required, Validators.pattern(/^(?=.*[A-Z])(?=.*\d)(.{8,100})$/)]],
      passwordConfirm: ['', [Validators.required]]
    },
    { validator: this.checkPasswords }
    );
  }

  checkPasswords(group: FormGroup) {
    let pass = group.get('password').value;
    let confirmPass = group.get('confirmPass').value;
  
    return pass === confirmPass ? null : { mismatch: true }     
  }   

  ngOnInit() {
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

  onSubmit() {
    console.warn(this.profileForm.value);
    this.userService.createUser(this.profileForm.value);    
  }

}
