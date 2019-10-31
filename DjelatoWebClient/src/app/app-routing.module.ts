import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { SignUpComponent } from './sign-up/sign-up.component';
import { ConfirmEmailComponent } from './confirm-email/confirm-email.component';

const routes: Routes = [
  { path: 'signup', component: SignUpComponent },
  { path: 'confirmemail', component: ConfirmEmailComponent },  
  { path: 'index', component: HomeComponent },
  { path: '', component: HomeComponent },  
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes)
  ],
  exports: [RouterModule]
})
export class AppRoutingModule { }
