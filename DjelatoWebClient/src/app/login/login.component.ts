import { Component, OnInit } from '@angular/core';
import { Validators, FormGroup, FormBuilder, AbstractControl } from '@angular/forms';

import { ToastrService } from 'ngx-toastr';

import { LoginModel } from '../models/login-model';

import { RegexExpressions } from '../shared/regex-expressions';

import { LoginService } from '../services/Login/login.service';
import { IResponseContent } from '../shared/models/response-content';


@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})

export class LoginComponent implements OnInit {

  loginForm: FormGroup;
  loginModel: LoginModel;
  message: string;
  isLoading = false;

  constructor(
    private formBuilder: FormBuilder,
    private toastr: ToastrService,
    private loginService: LoginService
  ) { 
    this.initLoginForm();
  }

  ngOnInit() { }

  initLoginForm(): void {
    this.loginForm = this.formBuilder.group({
      email: ['', [
        Validators.required, 
        Validators.pattern(RegexExpressions.emailRgx)]
      ],

      password: ['', [
        Validators.required,
        Validators.pattern(RegexExpressions.passwordRgx)]
      ],
    })
  }

  //from form property
  get email(): AbstractControl {
    return this.loginForm.get('email');    
  }

  get password(): AbstractControl {
    return this.loginForm.get('password');
  }

  onSubmit(): void {
    this.loginModel = <LoginModel>this.loginForm.value;

    this.isLoading = true;

    this.loginService.login(this.loginModel).subscribe((data: IResponseContent) =>{
      if (data.isSucceeded) {
        this.message = '';
        this.isLoading = false;

        this.toastr.success(
          'You login successfully', 
          'Notification!',
          {
            positionClass: 'toast-top-right',
        });
      }
     }, (error) => {
        let errorContent: IResponseContent = error.error;
        if (!errorContent.isSucceeded) {
          this.isLoading = false;          
          this.message = errorContent.errorMessage;
          
          this.toastr.error(
            this.message,
            'Sorry!',
            {
              positionClass: 'toastr-top-right'
            }
          )
        }

      });
  }



}
