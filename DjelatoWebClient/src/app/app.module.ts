import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';

import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { MatIconModule,
         MatMenuModule,
         MatInputModule,
         MatButtonModule,
         MatDialogModule
         } from '@angular/material';

import { AppComponent } from './app.component';   
import { SignUpComponent } from './sign-up/sign-up.component';
import { HomeComponent } from './home/home.component';
import { HeaderComponent } from './header/header.component';
import { ConfirmEmailComponent } from './confirm-email/confirm-email.component';
import { ConfirmEmailPopupComponent } from './confirm-email-popup/confirm-email-popup.component';
import { ToastrModule } from 'ngx-toastr';

@NgModule({
  declarations: [
    AppComponent,
    SignUpComponent,
    HomeComponent,
    HeaderComponent,
    ConfirmEmailComponent,
    ConfirmEmailPopupComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    FormsModule,
    AppRoutingModule,    
    MatMenuModule,
    MatIconModule,
    MatInputModule,
    MatButtonModule,
    MatDialogModule,
    ReactiveFormsModule,    
    HttpClientModule,
    ToastrModule.forRoot()
  ],
  providers: [],
  bootstrap: [AppComponent],
  entryComponents: [ConfirmEmailPopupComponent]
})
export class AppModule { }
