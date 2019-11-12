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
         MatToolbarModule,
         MatSidenavModule, 
         MatListModule
         } from '@angular/material';

import {MatDialogModule, MatDialogRef, MAT_DIALOG_DATA} from '@angular/material/dialog';

import { AppComponent } from './app.component';   
import { SignUpComponent } from './sign-up/sign-up.component';
import { HomeComponent } from './home/home.component';
import { HeaderComponent } from './header/header.component';
import { ConfirmEmailComponent } from './confirm-email/confirm-email.component';
import { ConfirmEmailPopupComponent } from './confirm-email-popup/confirm-email-popup.component';
import { ToastrModule } from 'ngx-toastr';
import { NavBarComponent } from './nav-bar/nav-bar.component';
import { LayoutModule } from '@angular/cdk/layout';

@NgModule({
  declarations: [
    AppComponent,
    SignUpComponent,
    HomeComponent,
    HeaderComponent,
    ConfirmEmailComponent,
    ConfirmEmailPopupComponent,
    NavBarComponent
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
    ToastrModule.forRoot(),
    LayoutModule,
    MatToolbarModule,
    MatSidenavModule,
    MatListModule
  ],
  providers: [
    { provide: MatDialogRef, useValue: {} },
    { provide: MAT_DIALOG_DATA, useValue: [] }
  ],
  bootstrap: [AppComponent],
  entryComponents: [ConfirmEmailPopupComponent]
})
export class AppModule { }
