import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { LayoutModule } from '@angular/cdk/layout';
import { NgModule } from '@angular/core';

import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';

import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { AppComponent } from './app.component';   
import { ConfirmEmailComponent } from './confirm-email/confirm-email.component';
import { ConfirmEmailPopupComponent } from './confirm-email-popup/confirm-email-popup.component';
import { HomeComponent } from './home/home.component';
import { NavBarComponent } from './nav-bar/nav-bar.component';
import { SignUpComponent } from './sign-up/sign-up.component';

import { ToastrModule } from 'ngx-toastr';

import { MaterialModule } from './shared/material/material.module';
import { LoginComponent } from './login/login.component';

import {MatProgressSpinnerModule} from '@angular/material/progress-spinner';

@NgModule({
  declarations: [
    AppComponent,
    SignUpComponent,
    HomeComponent,
    ConfirmEmailComponent,
    ConfirmEmailPopupComponent,
    NavBarComponent,
    LoginComponent
  ],

  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    FormsModule,
    AppRoutingModule,    
    MaterialModule,
    ReactiveFormsModule,    
    HttpClientModule,
    ToastrModule.forRoot(),
    LayoutModule,
    MatProgressSpinnerModule
  ],
  exports: [
  ],
  providers: [],
  bootstrap: [AppComponent],
  entryComponents: [ConfirmEmailPopupComponent]
})
export class AppModule { }
