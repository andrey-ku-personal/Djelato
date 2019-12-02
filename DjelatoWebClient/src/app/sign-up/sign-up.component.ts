import { Component, OnInit } from '@angular/core';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, ValidationErrors } from '@angular/forms';

import { UserService } from '../services/User/user.service';

import { UserModel } from '../models/user-model';

import { IResponseContent } from '../shared/models/response-content';

import { RegexExpressions } from '../shared/regex-expressions';

import { ToastrService } from 'ngx-toastr';
 
@Component({
  selector: 'app-sign-up',
  templateUrl: './sign-up.component.html',
  styleUrls: ['./sign-up.component.css']
})

export class SignUpComponent implements OnInit {
  stringUrl: string;
  profileForm: FormGroup;
  model: UserModel;
  message: string;
  fileToUpload: File = null;
  isLoading = false;

  constructor(    
    private formBuilder: FormBuilder,
    private toastr: ToastrService,
    private userService: UserService
  ){
    this.initProfileForm();
  }

  ngOnInit() {
    this.profileForm.get("password").valueChanges.subscribe(() => {
      this.profileForm.get("passwordConfirm").updateValueAndValidity();
    });
  }

  initProfileForm(): void {  
    this.profileForm = this.formBuilder.group({
      avatar: [
        ''
      ],
      name: ['', [
        Validators.required, 
        Validators.pattern(RegexExpressions.nameRgx)]
      ],
      email: ['', [
        Validators.required, 
        Validators.pattern(RegexExpressions.emailRgx)]
      ],

      phoneNumber: ['', [
        Validators.required, 
        Validators.pattern(RegexExpressions.phoneNumber)]
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

  //password validation
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

  //from form property
  get imgAvatar(){
    return this.profileForm.get('avatar')
  }

  get name(){
    return this.profileForm.get('name');
  }

  get email(){
    return this.profileForm.get('email');
  }

  get phoneNumber(){
    return this.profileForm.get('phoneNumber');
  }

  get password(){
    return this.profileForm.get('password')
  }

  get passwordConfirm(){
    return this.profileForm.get('passwordConfirm')
  } 

  //Image preview
  onSelectFile(event) {
    const file = (event.target as HTMLInputElement).files[0];
    this.fileToUpload = event.target.files[0];
    console.log( " this.fileToUpload",this.fileToUpload)
    this.profileForm.patchValue({
      avatar: file
    });
    this.imgAvatar.updateValueAndValidity;

    // File Preview
    const reader = new FileReader();
    reader.onload = () => {
      this.stringUrl = reader.result as string;
    }
    reader.readAsDataURL(file)
  }

  appendFile(): FormData {
    const form = new FormData();
    let user: UserModel = <UserModel>this.profileForm.value;

    form.append('avatar', user.avatar); 
    form.append('name', user.name);
    form.append('email', user.email);
    form.append('phoneNumber', user.phoneNumber);
    form.append('password', user.password);
    form.append('passwordConfirm', user.passwordConfirm);
    
    return form;
  }

  onSubmit(): void {
    let profile: FormData = this.appendFile();

    this.isLoading = true;

    this.userService.createUser(profile).subscribe((data: IResponseContent) => {
      if (data.isSucceeded) {
        this.message = '';
        this.isLoading = false;

        this.toastr.success(
          'Your profile create successfully', 
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
          positionClass: 'toast-top-right'
        });        
      }      
    });
  }
}
